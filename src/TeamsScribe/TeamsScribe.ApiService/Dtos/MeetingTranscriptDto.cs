namespace TeamsScribe.ApiService;

public record class MeetingTranscriptDto(string Organizer, IReadOnlyCollection<string> Participants, string Title, string Description, string TranscriptionBlob);
