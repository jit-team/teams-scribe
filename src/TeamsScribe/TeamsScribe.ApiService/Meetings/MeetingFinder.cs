using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace TeamsScribe.ApiService.Meetings;

public class MeetingFinder(GraphServiceClient graphClient)
{
    public async Task<Meeting> FindAsync(string organizerEmail, string joinWebUrl)
    {
        var organizer = await GetOrganizer(organizerEmail);
        var onlineMeeting = await GetOnlineMeeting(organizer, joinWebUrl);

        return new Meeting(organizer, onlineMeeting);
    }

    private async Task<User> GetOrganizer(string organizerEmail)
    {
        var user = await graphClient.Users[organizerEmail].GetAsync();
        ArgumentNullException.ThrowIfNull(user, $"There is no user with email {organizerEmail}");

        return user;
    }

    private async Task<OnlineMeeting> GetOnlineMeeting(User organizer, string joinWebUrl)
    {
        var meetings = await graphClient.Users[organizer?.Id].OnlineMeetings
            .GetAsync(r =>
            {
                r.QueryParameters.Filter = $"JoinWebUrl eq '{joinWebUrl}'";
            });

        var meeting = meetings?.Value?.FirstOrDefault();
        ArgumentNullException.ThrowIfNull(meeting, $"There is no meeting for url '{joinWebUrl}'");

        return meeting;
    }
}
