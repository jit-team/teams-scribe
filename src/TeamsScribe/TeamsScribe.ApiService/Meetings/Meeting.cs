using Microsoft.Graph.Models;

namespace TeamsScribe.ApiService.Meetings;

public record Meeting(User Organizer, OnlineMeeting OnlineMeeting);
