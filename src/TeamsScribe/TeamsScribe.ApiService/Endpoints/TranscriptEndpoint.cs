using TeamsScribe.ApiService.Clients.AzureOpenAI;
using TeamsScribe.ApiService.Dtos;

namespace TeamsScribe.ApiService.Endpoints;

public static class TranscriptEndpoint
{
    public static void Map(this WebApplication app)
    {
        app.MapGroup("/transcript").AddEndpoints().WithOpenApi();
    }

    private static RouteGroupBuilder AddEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (TranscriptDto dto, IAiClient aiClient) =>
        {
            var result = await aiClient.GetMeetingMinutesAsync(dto.transcript);
            return Results.Ok(result);
        });
        return group;
    }
}

