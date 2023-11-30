using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using System.Web;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Serialization.Json;
using TeamsScribe.Functions.ChangeNotifications.Handlers.Transcriptions;

namespace TeamsScribe.Functions;

public class MeetingTranscriptionDownloaderFunction
{
    private readonly MeetingTranscriptionDownloader _meetingTranscriptionDownloader;    

    public MeetingTranscriptionDownloaderFunction(MeetingTranscriptionDownloader meetingTranscriptionDownloader)
    {
        _meetingTranscriptionDownloader = meetingTranscriptionDownloader;        
    }

    [Function("MeetingTranscriptionDownloader")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        return await ProcessRequest(req);
    }       

    private async Task<HttpResponseData> ProcessRequest(HttpRequestData req)
    {
        var json = await req.ReadFromJsonAsync<RequestDto>();
        await _meetingTranscriptionDownloader.DownloadAsync(json.OrganizerEmail, json.JoinWebUrl);

        return req.CreateResponse(HttpStatusCode.Created);
    }

    public record RequestDto(string OrganizerEmail, string JoinWebUrl);
}

