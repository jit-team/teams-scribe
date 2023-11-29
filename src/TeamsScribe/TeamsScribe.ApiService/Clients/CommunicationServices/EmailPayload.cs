namespace TeamsScribe.ApiService;

public record MeetingMinutesEmailPayload(string Organizer, IReadOnlyCollection<string> Recipients, string Title, string Minutes);