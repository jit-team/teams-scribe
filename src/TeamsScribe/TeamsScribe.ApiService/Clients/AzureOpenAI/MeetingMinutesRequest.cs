namespace TeamsScribe.ApiService;

public record MeetingMinutesRequest(DateTimeOffset MeetingDate, string Title, string Description, string Transcript);
