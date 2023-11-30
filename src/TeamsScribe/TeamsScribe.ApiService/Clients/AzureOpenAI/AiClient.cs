
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using TeamsScribe.ApiService.Clients.Config;

namespace TeamsScribe.ApiService.Clients.AzureOpenAI;

public class AiClient : IAiClient
{
    private readonly OpenAIClient _aiClient;

    public AiClient(IOptions<AzureOpenAiSettings> options)
    {
        var proxyUrl = new Uri(options.Value.Url + "/v1/api");
        var token = new AzureKeyCredential(options.Value.Key + "/JakuSw");
        _aiClient = new OpenAIClient(proxyUrl, token);
    }

    public async Task<string> GetMeetingMinutesAsync(MeetingMinutesRequest request, CancellationToken cancellationToken)
    {
        ChatCompletionsOptions completionOptions = new()
        {
            MaxTokens = 5000,
            Temperature = 0.7f,
            NucleusSamplingFactor = 0.95f,
            DeploymentName = "gpt-35-turbo"
        };

        completionOptions.Messages.Add(new ChatMessage(ChatRole.System, TranscriptPrompt.SetupBaseTranscript));
        completionOptions.Messages.Add(new ChatMessage(ChatRole.User, TranscriptPrompt.SetupExampleTranscriptQuestion));
        completionOptions.Messages.Add(new ChatMessage(ChatRole.Assistant, TranscriptPrompt.SetupExampleTranscriptResponse));

        var questionPrompt = string.Format(TranscriptPrompt.TranscriptTemplate,
            request.MeetingDate,
            string.IsNullOrEmpty(request.Description) ? "Not provided." : request.Description,
            request.Transcript);

        completionOptions.Messages.Add(new ChatMessage(ChatRole.User, questionPrompt));
        var response = await _aiClient.GetChatCompletionsAsync(completionOptions, cancellationToken);
        return response.Value.Choices[0].Message.Content;
    }
}

