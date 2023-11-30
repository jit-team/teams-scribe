using Azure.Storage.Blobs;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace TeamsScribe.ApiService.Meetings;

public class MeetingTranscriptionDownloader(GraphServiceClient graphClient, BlobClient blobClient)
{
    public async Task<string> DownloadAsync(User organizer, OnlineMeeting onlineMeeting)
    {
        var transcripts = await graphClient.Users[organizer?.Id].OnlineMeetings[onlineMeeting?.Id].Transcripts.GetAsync();
        var transcript = transcripts.Value.FirstOrDefault();       

        var fileName = $"{organizer.UserPrincipalName}_{onlineMeeting.StartDateTime}_transcript.vtt";
        var contentUrl = $"{transcript.TranscriptContentUrl}?$format=text/vtt";

        // var transcript = await graphClient.Users[organizer?.Id].OnlineMeetings[onlineMeeting?.Id].Transcripts[transcript.Id].GetAsync();
        // var stream = transcript.Content

        // blobClient.UploadTranscriptAsync(fileName, );

        return string.Empty;
    }
}
