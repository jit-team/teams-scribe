
namespace TeamsScribe.ApiService.Clients.AzureOpenAI;

public interface IAiClient
{
    Task<string> GetMeetingMinutesAsync(MeetingMinutesRequest request, CancellationToken cancellationToken);
}