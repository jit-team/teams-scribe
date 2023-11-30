using Azure.Identity;
using Microsoft.Graph;
using Microsoft.OpenApi.Models;
using TeamsScribe.ApiService;
using TeamsScribe.ApiService.Clients.AzureOpenAI;
using TeamsScribe.ApiService.Clients.Config;
using TeamsScribe.ApiService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AI API", Version = "v1" });
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "ApiKey must appear in header",
        Type = SecuritySchemeType.ApiKey,
        Name = "ApiKey",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });
    var key = new OpenApiSecurityScheme()
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "ApiKey"
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

builder.Services.AddScoped<MeetingTranscriptionDownloader>();    

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5220")
                            .WithMethods(HttpMethod.Post.ToString())
                            .AllowAnyHeader();
                      });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseExceptionHandler();
app.UseCors(MyAllowSpecificOrigins);

TranscriptEndpoint.Map(app);

app.MapDefaultEndpoints();

app.Run();
