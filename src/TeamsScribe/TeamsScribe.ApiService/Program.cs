using Azure.Identity;
using Microsoft.Graph;
using Microsoft.OpenApi.Models;
using TeamsScribe.ApiService;
using TeamsScribe.ApiService.Clients.AzureOpenAI;
using TeamsScribe.ApiService.Clients.Config;
using TeamsScribe.ApiService.Endpoints;
using TeamsScribe.ApiService.Meetings;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AI API", Version = "v1" });
    c.AddSecurityDefinition("apikey", new OpenApiSecurityScheme
    {
        Description = "apikey must appear in header",
        Type = SecuritySchemeType.ApiKey,
        Name = "apikey",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });
    var key = new OpenApiSecurityScheme()
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "apikey"
        },
        In = ParameterLocation.Header
    };
    var requirement = new OpenApiSecurityRequirement
    {
        { key, new List<string>() }
    };
    c.AddSecurityRequirement(requirement);
});

// Add dependency injection for IAiClient and AiClient.
builder.Services.AddTransient<IAiClient, AiClient>();
builder.Services.AddTransient<BlobClient>();
builder.Services.AddTransient<MeetingMinutesDistributionClient>();

builder.Services.AddSingleton<ScribeBackgroundService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<ScribeBackgroundService>());
builder.Services.AddSingleton<IScribeWorkingQueue>(sp => sp.GetRequiredService<ScribeBackgroundService>());

builder.Services.AddCors();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.Configure<AzureOpenAiSettings>(builder.Configuration.GetSection(AzureOpenAiSettings.SectionName));
builder.Services.Configure<CommunicationServicesSettings>(builder.Configuration.GetSection(CommunicationServicesSettings.SectionName));
builder.Services.Configure<AzureBlobStorageSettings>(builder.Configuration.GetSection(AzureBlobStorageSettings.SectionName));

// Graph transcript download
builder.Services.AddScoped((sp) =>
        {
            string[] scopes = ["https://graph.microsoft.com/.default"];
            var section = sp.GetService<IConfiguration>().GetSection("AzureAd");

            var tenantId = section["TenantId"];
            var clientId = section["ClientId"];
            var clientSecret = section["ClientSecret"];

            var options = new ClientSecretCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
            };

            var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret, options);

            return new GraphServiceClient(clientSecretCredential, scopes);
        });

builder.Services.AddScoped<MeetingFinder>();
builder.Services.AddScoped<MeetingTranscriptionDownloader>();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5220", "https://gray-pebble-0fa116603.4.azurestaticapps.net")
                            .WithMethods(HttpMethod.Post.ToString())
                            .AllowAnyHeader();
                      });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.Use((context, next) =>
{
    var apiKey = builder.Configuration.GetValue<string>("ApiKey");
    var requestApiKey = context.Request.Headers.FirstOrDefault(h => h.Key == "apikey");
    if (apiKey != requestApiKey.Value)
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    }
    return next(context);
});

app.UseExceptionHandler();
app.UseCors(MyAllowSpecificOrigins);

TranscriptEndpoint.Map(app);

app.MapDefaultEndpoints();

app.Run();
