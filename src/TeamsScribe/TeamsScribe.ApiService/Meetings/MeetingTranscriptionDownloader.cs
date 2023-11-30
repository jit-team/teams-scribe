using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace TeamsScribe.ApiService.Meetings;

public class MeetingTranscriptionDownloader(GraphServiceClient graphClient, BlobClient blobClient)
{
    public async Task<string> DownloadAsync(User organizer, OnlineMeeting onlineMeeting)
    {
        var transcripts = await graphClient.Users[organizer?.Id].OnlineMeetings[onlineMeeting?.Id].Transcripts.GetAsync();
        var transcript = transcripts.Value.FirstOrDefault();       

        var fileName = $"{organizer.UserPrincipalName}_meetingat_{onlineMeeting.StartDateTime.Value:O}_transcript.vtt";        

        var contentStream = await graphClient.Users[organizer?.Id]
                                    .OnlineMeetings[onlineMeeting?.Id]
                                    .Transcripts[transcript.Id]
                                    .Content.WithUrl($"{transcript.TranscriptContentUrl}?$format=text/vtt").GetAsync();

        await blobClient.UploadTranscriptAsync(fileName, contentStream);

        return fileName;
    }
}
