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
        
        return group;
    }
}

