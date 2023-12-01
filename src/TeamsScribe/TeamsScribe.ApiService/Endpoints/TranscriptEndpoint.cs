using System.Web;
using HtmlAgilityPack;
using TeamsScribe.ApiService.Dtos;
using TeamsScribe.ApiService.Events;
using TeamsScribe.ApiService.Meetings;

namespace TeamsScribe.ApiService.Endpoints;

public static class TranscriptEndpoint
{
    public static void Map(this WebApplication app)
    {
        app.MapGroup("/api/meeting-summaries").AddEndpoints().WithOpenApi();
    }

    private static RouteGroupBuilder AddEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (MeetingSummaryFormDto dto, 
            MeetingFinder meetingFinder,
            EventFinder eventFinder,
            MeetingTranscriptionDownloader transcriptionDownloader,
            IScribeWorkingQueue queue) =>
        {
            var meeting = await meetingFinder.FindAsync(dto.OrganizerEmail, dto.JoinWebUrl);
            var calendarEvent = await eventFinder.FindAsync(dto.OrganizerEmail, DateOnly.FromDateTime(meeting.OnlineMeeting.StartDateTime.Value.DateTime), dto.JoinWebUrl);
            var transcriptionPath = await transcriptionDownloader.DownloadAsync(meeting.Organizer, meeting.OnlineMeeting);

            var transcriptDto = FormScribeRequest(meeting, calendarEvent, transcriptionPath);
            queue.Enqueue(transcriptDto);    

            return Results.Accepted();
        });
        
        return group;
    }

    private static MeetingTranscriptDto FormScribeRequest(Meeting meeting, CalendarEvent calendarEvent, string transcriptionPath)
    {
        var organizer = meeting.Organizer;
        var onlineMeeting = meeting.OnlineMeeting;
        var participants = onlineMeeting.Participants.Attendees.Select(a => a.Upn).ToList();
        
        var html = new HtmlDocument();
        html.LoadHtml(HttpUtility.UrlDecode(calendarEvent.@event.Body.Content));
        var description = html.DocumentNode.SelectSingleNode("//body").InnerText;

        return new MeetingTranscriptDto(
            organizer.UserPrincipalName,
            onlineMeeting.StartDateTime.Value, 
            participants,
            onlineMeeting.Subject, 
            description[..(description.IndexOf('_') + 1)], 
            transcriptionPath);
    }
}

