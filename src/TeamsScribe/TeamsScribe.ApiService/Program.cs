using TeamsScribe.ApiService.Clients.AzureOpenAI;
using TeamsScribe.ApiService.Clients.Config;
using TeamsScribe.ApiService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

TranscriptEndpoint.Map(app);

app.MapDefaultEndpoints();

app.Run();
