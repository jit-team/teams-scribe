namespace TeamsScribe.ApiService;

public record MeetingMinutesRequest(DateTimeOffset MeetingDate, string Description, string Transcript);
