
namespace TeamsScribe.ApiService.Clients.AzureOpenAI;

public interface IAiClient
{
    Task<string> GetMeetingMinutesAsync(string transcript);
}