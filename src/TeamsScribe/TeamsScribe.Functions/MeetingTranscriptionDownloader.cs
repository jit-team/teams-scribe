using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace TeamsScribe.Functions.ChangeNotifications.Handlers.Transcriptions;

public class MeetingTranscriptionDownloader
{
    private readonly GraphServiceClient _graphClient;    
    private readonly HttpClient _httpClient;
    private readonly BlobContainerClient _blobContainerClient;

    public MeetingTranscriptionDownloader(GraphServiceClient graphClient,        
        IHttpClientFactory httpClientFactory,
        IAzureClientFactory<BlobServiceClient> blobClientFactory)
    {
        _graphClient = graphClient;        
        _httpClient = httpClientFactory.CreateClient("TeamsScribeApi");
        _blobContainerClient = blobClientFactory.CreateClient("teamsScribeBlob").GetBlobContainerClient("transcripts");
        _blobContainerClient.CreateIfNotExists();
    }

    public async Task DownloadAsync(string organizerEmail, string joinWebUrl)
    {
        var organizer = await GetOrganizer(organizerEmail);
        var onlineMeeting = await GetOnlineMeeting(organizer, joinWebUrl);

        var transcripts = await _graphClient.Users[organizer?.Id].OnlineMeetings[onlineMeeting?.Id].Transcripts.GetAsync();

        foreach (var transcript in transcripts.Value)
        {
            var contentUrl = $"{transcript.TranscriptContentUrl}?$format=text/vtt";
            
            //
            //
            //
        }
    }

    private async Task<User> GetOrganizer(string organizerEmail)
    {
        var user = await _graphClient.Users[organizerEmail].GetAsync();
        ArgumentNullException.ThrowIfNull(user, $"There is no user with email {organizerEmail}");

        return user;
    }

    private async Task<OnlineMeeting?> GetOnlineMeeting(User organizer, string joinWebUrl)
    {
        var meetings = await _graphClient.Users[organizer?.Id].OnlineMeetings
            .GetAsync(r =>
            {
                r.QueryParameters.Filter = $"JoinWebUrl eq '{joinWebUrl}'";
            });

        var meeting = meetings?.Value?.FirstOrDefault();
        ArgumentNullException.ThrowIfNull(meeting, $"There is no meeting for url '{joinWebUrl}'");

        return meeting;
    }
}
