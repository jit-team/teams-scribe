using TeamsScribe.ApiService.Clients.AzureOpenAI;
using TeamsScribe.ApiService.Dtos;
using TeamsScribe.ApiService.Meetings;

namespace TeamsScribe.ApiService.Endpoints;

public static class TranscriptEndpoint
{
    public static void Map(this WebApplication app)
    {
        app.MapGroup("/meeting-summaries").AddEndpoints().WithOpenApi();
    }

    private static RouteGroupBuilder AddEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (MeetingSummaryFormDto dto, 
            MeetingFinder meetingFinder,
            MeetingTranscriptionDownloader transcriptionDownloader) =>
        {
            var meeting = await meetingFinder.FindAsync(dto.OrganizerEmail, dto.JoinWebUrl);
            var transcriptionPath = await transcriptionDownloader.DownloadAsync(meeting.Organizer, meeting.OnlineMeeting);

            return Results.Accepted();
        });
        
        return group;
    }
}

