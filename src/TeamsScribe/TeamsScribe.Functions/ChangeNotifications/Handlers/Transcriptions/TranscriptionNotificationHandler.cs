using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace TeamsScribe.Functions.ChangeNotifications.Handlers.Transcriptions;

public class TranscriptionNotificationHandler
{
    private readonly GraphServiceClient _graphClient;
    private readonly IConfiguration _configuration;
    private readonly BlobContainerClient _blobContainerClient;

    public TranscriptionNotificationHandler(GraphServiceClient graphClient,
        IConfiguration configuration,
        IAzureClientFactory<BlobServiceClient> blobClientFactory)
    {
        _graphClient = graphClient;
        _configuration = configuration;
        _blobContainerClient = blobClientFactory.CreateClient("teamsScribeBlob").GetBlobContainerClient("transcripts");
        _blobContainerClient.CreateIfNotExists();
    }

    public async Task HandleAsync(ChangeNotification changeNotification)
    {
        var chatId = await GetChatId(changeNotification);
        var chat = await _graphClient.Chats[chatId].GetAsync();

        var organizer = await GetOrganizer(chat);
        var onlineMeeting = await GetOnlineMeeting(chat, organizer);

        var transcripts = await _graphClient.Users[organizer?.Id].OnlineMeetings[onlineMeeting?.Id].Transcripts.GetAsync();

        foreach (var transcript in transcripts.Value)
        {

        }
    }

    private Task<string> GetChatId(ChangeNotification changeNotification)
    {
        var encryptionCertificate = _configuration.GetSection("GraphSubscription")["EncryptionCertificate"];

        return Task.FromResult("");
    }

    private async Task<User?> GetOrganizer(Chat? chat)
    {
        var organizerId = chat?.OnlineMeetingInfo?.Organizer?.Id;
        return await _graphClient.Users[organizerId].GetAsync();
    }

    private async Task<OnlineMeeting?> GetOnlineMeeting(Chat? chat, User? organizer)
    {
        var meetingUrl = chat?.OnlineMeetingInfo?.JoinWebUrl ?? await ExtractJoinWebUrlFromCalendarEventId(organizer?.Id, chat?.OnlineMeetingInfo?.CalendarEventId);

        var meetings = await _graphClient.Users[organizer?.Id].OnlineMeetings
            .GetAsync(r =>
            {
                r.QueryParameters.Filter = $"JoinWebUrl eq '{meetingUrl}'";
            });

        return meetings?.Value?.FirstOrDefault();
    }

    private async Task<string?> ExtractJoinWebUrlFromCalendarEventId(string? organizerId, string? calendarEventId)
    {
        var calendarEvent = await _graphClient.Users[organizerId].Events[calendarEventId].GetAsync();
        return calendarEvent?.OnlineMeeting?.JoinUrl;
    }
}
