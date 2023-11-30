namespace TeamsScribe.ApiService;

public record class MeetingTranscriptDto(string Organizer, DateTime MeetingDate, IReadOnlyCollection<string> Participants, string Title, string Description, string TranscriptionBlob);
