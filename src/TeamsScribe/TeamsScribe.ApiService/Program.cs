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
builder.Services.AddCors();
// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.Configure<AzureOpenAiSettings>(builder.Configuration.GetSection(AzureOpenAiSettings.SectionName));
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5220").AllowAnyMethod().AllowAnyHeader();
                      });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseExceptionHandler();
app.UseCors(MyAllowSpecificOrigins);
app.Use((context, next) =>
{
    var apiKey = builder.Configuration.GetValue<string>("ApiKey");
    var requestApiKey = context.Request.Headers.FirstOrDefault(h => h.Key == "ApiKey");
    if (apiKey != requestApiKey.Value)
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    }
    return next(context);
});

TranscriptEndpoint.Map(app);

app.MapDefaultEndpoints();

app.Run();
