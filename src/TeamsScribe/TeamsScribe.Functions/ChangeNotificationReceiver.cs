using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Serialization.Json;
using TeamsScribe.Functions.ChangeNotifications.Handlers.Transcriptions;

namespace TeamsScribe.Functions;

public class ChangeNotificationReceiver
{    
    private readonly TranscriptionNotificationHandler _transcriptionNotificationHandler;
    private readonly ILogger _logger;

    public ChangeNotificationReceiver(
        TranscriptionNotificationHandler transcriptionNotificationHandler,
        ILoggerFactory loggerFactory)
    {        
        _transcriptionNotificationHandler = transcriptionNotificationHandler;
        _logger = loggerFactory.CreateLogger<ChangeNotificationReceiver>();
    }

    [Function("ChangeNotificationReceiver")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        ArgumentNullException.ThrowIfNull(req);

        if (req.Query["validationToken"] is not null)
        {
            return ValidateSubscription(req);
        }

        return await ProcessChangeNotification(req);
    }

    private HttpResponseData ValidateSubscription(HttpRequestData req)
    {
        var validationToken = req.Query["validationToken"]?.ToString();

        ArgumentNullException.ThrowIfNull(validationToken);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        response.WriteString(validationToken);

        _logger.LogInformation("Responding to a subscription validation request");

        return response;
    }

    private async Task<HttpResponseData> ProcessChangeNotification(HttpRequestData req)
    {        
        _logger.LogInformation("Received change notifications from Graph subscription");
        var clientState = Environment.GetEnvironmentVariable("Graph:SecretClientState");

        using var reader = new StreamReader(req.Body);
        var requestBody = await reader.ReadToEndAsync();        
        
        using var document = JsonDocument.Parse(requestBody);
        var jsonParseNode = new JsonParseNode(document.RootElement);
        var collectionResponse = jsonParseNode.GetObjectValue(ChangeNotificationCollectionResponse.CreateFromDiscriminatorValue);

        foreach (var changeNotification in collectionResponse.Value)
        {
            if (changeNotification.ClientState != clientState)
            {
                _logger.LogWarning("Not matching ClientState found in change notification");
                continue;
            }

            await _transcriptionNotificationHandler.HandleAsync(changeNotification);
        }

        return req.CreateResponse(HttpStatusCode.Accepted);
    }
}

