
using System.Collections.Concurrent;
using TeamsScribe.ApiService.Clients.AzureOpenAI;

namespace TeamsScribe.ApiService;

public interface IScribeWorkingQueue
{
    public void Enqueue(MeetingTranscriptDto request);
}

public class ScribeBackgroundService : BackgroundService, IScribeWorkingQueue
{
    private readonly MeetingMinutesDistributionClient _distributionClient;
    private readonly BlobClient _blobClient;
    private readonly IAiClient _aiClient;

    private readonly ConcurrentQueue<MeetingTranscriptDto> _queue = new();

    public ScribeBackgroundService(
        MeetingMinutesDistributionClient distributionClient,
        BlobClient blobClient,
        IAiClient aiClient)
    {
        _distributionClient = distributionClient;
        _blobClient = blobClient;
        _aiClient = aiClient;
    }

    public void Enqueue(MeetingTranscriptDto request)
    {
        _queue.Enqueue(request);
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            while (_queue.TryDequeue(out var request))
            {
                await ProcessAsync(request, stoppingToken);
            }

            await Task.Delay(500, stoppingToken);
        }
    }

    private async Task ProcessAsync(MeetingTranscriptDto meeting, CancellationToken cancellationToken)
    {
        var transcript = await _blobClient.FetchTranscript(meeting.TranscriptionBlob, cancellationToken);
        var meetingMinutes = await _aiClient.GetMeetingMinutesAsync(transcript, cancellationToken);
        var meetingMinutesPayload = new MeetingMinutesEmailPayload(meeting.Organizer, meeting.Participants, meeting.Title, meetingMinutes);
        await _distributionClient.SendAsync(meetingMinutesPayload, cancellationToken);
    }
}
