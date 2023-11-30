using TeamsScribe.ApiService.Clients.AzureOpenAI;
using TeamsScribe.ApiService.Dtos;

namespace TeamsScribe.ApiService.Endpoints;

public static class TranscriptEndpoint
{
    public static void Map(this WebApplication app)
    {
        app.MapGroup("/meeting-summaries").AddEndpoints().WithOpenApi();
    }

    private static RouteGroupBuilder AddEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (MeetingSummaryFormDto dto, MeetingTranscriptionDownloader transcriptionDownloader) =>
        {
            await transcriptionDownloader.DownloadAsync(dto.OrganizerEmail, dto.JoinWebUrl);
            return Results.Accepted();
        });

        group.MapPost("/meeting", async (
            MeetingMinutesDistributionClient distributionClient,
            MeetingTranscriptDto meeting,
            BlobClient blobClient,
            IAiClient aiClient
            ) => 
            {
                var transcript = await blobClient.FetchTranscript(meeting.TranscriptionBlob);
                var meetingMinutes = await aiClient.GetMeetingMinutesAsync(transcript);
                var meetingMinutesPayload = new MeetingMinutesEmailPayload(meeting.Organizer, meeting.Participants, meeting.Title, meetingMinutes);
                await distributionClient.SendAsync(meetingMinutesPayload);
            });
        
        return group;
    }
}

