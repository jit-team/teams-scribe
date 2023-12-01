using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace TeamsScribe.ApiService.Events;

public class EventFinder(GraphServiceClient graphClient)
{
    public async Task<CalendarEvent> FindAsync(string organizerEmail, DateOnly date, string joinWebUrl)
    {
        var events = await graphClient.Users[organizerEmail].Events
                        .GetAsync(rc =>
                        {
                            rc.QueryParameters.Filter = $"start/dateTime ge '{date.AddDays(-1):yyyy-MM-dd}T00:00' and start/dateTime lt '{date.AddDays(1):yyyy-MM-dd}T00:00'";
                        });

        return new(events.Value.FirstOrDefault(e => e.OnlineMeeting.JoinUrl == joinWebUrl));
    }
}
