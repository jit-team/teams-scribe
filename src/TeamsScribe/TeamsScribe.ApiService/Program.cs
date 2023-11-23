using Azure;
using Azure.AI.OpenAI;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
var keySecret = builder.Configuration.GetValue<string>("AzOpenAi:Key");
var proxyUrlSecret = builder.Configuration.GetValue<string>("AzOpenAi:Url");
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var proxyUrl = new Uri(proxyUrlSecret + "/v1/api");

var token = new AzureKeyCredential(keySecret + "/JakuSw");

var openAIClient = new OpenAIClient(proxyUrl, token);

app.MapGet("/test", async () =>
{
    ChatCompletionsOptions completionOptions = new()
    {
        MaxTokens = 2048,
        Temperature = 0.7f,
        NucleusSamplingFactor = 0.95f,
        DeploymentName = "gpt-35-turbo"
    };

    completionOptions.Messages.Add(new ChatMessage(ChatRole.System, "you are a helpful tax accountant and want to lower everybody's taxes."));
    completionOptions.Messages.Add(new ChatMessage(ChatRole.User, "hi there"));
    var response = await openAIClient.GetChatCompletionsAsync(completionOptions);

    return Results.Ok(response.Value.Choices);
});

app.MapDefaultEndpoints();

app.Run();
