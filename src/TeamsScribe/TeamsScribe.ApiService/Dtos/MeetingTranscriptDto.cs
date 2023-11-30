namespace TeamsScribe.ApiService;

public record class MeetingTranscriptDto(string Organizer, DateTimeOffset MeetingDate, IReadOnlyCollection<string> Participants, string Title, string Description, string TranscriptionBlob);
