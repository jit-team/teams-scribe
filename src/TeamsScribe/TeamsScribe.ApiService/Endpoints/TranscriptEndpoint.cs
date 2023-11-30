using System.Text.Encodings.Web;
using System.Web;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.Routing;
using TeamsScribe.ApiService.Clients.AzureOpenAI;
using TeamsScribe.ApiService.Dtos;
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
            MeetingTranscriptionDownloader transcriptionDownloader,
            IScribeWorkingQueue queue) =>
        {
            var meeting = await meetingFinder.FindAsync(dto.OrganizerEmail, dto.JoinWebUrl);
            var transcriptionPath = await transcriptionDownloader.DownloadAsync(meeting.Organizer, meeting.OnlineMeeting);

            var transcriptDto = FormScribeRequest(meeting, transcriptionPath);
            //queue.Enqueue(transcriptDto);    

            return Results.Accepted();
        });
        
        return group;
    }

    private static MeetingTranscriptDto FormScribeRequest(Meeting meeting, string transcriptionPath)
    {
        var organizer = meeting.Organizer;
        var onlineMeeting = meeting.OnlineMeeting;
        var participants = onlineMeeting.Participants.Attendees.Select(a => a.Upn).ToList();
        
        var html = new HtmlDocument();
        html.LoadHtml(HttpUtility.UrlDecode(onlineMeeting.JoinInformation.Content.Replace("data:text/html,", "")));
        var description = html.DocumentNode.FirstChild.InnerHtml;

        return new MeetingTranscriptDto(
            organizer.UserPrincipalName,
            onlineMeeting.StartDateTime.Value, 
            participants,
            onlineMeeting.Subject, 
            description, 
            transcriptionPath);
    }
}

