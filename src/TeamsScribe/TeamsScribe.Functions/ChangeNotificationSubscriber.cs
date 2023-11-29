using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace TeamsScribe.Functions;

public class ChangeNotificationSubscriber
{
    private readonly GraphServiceClient _graphClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;    

    public ChangeNotificationSubscriber(
        GraphServiceClient graphClient,
        IConfiguration configuration,
        ILoggerFactory loggerFactory)
    {
        _graphClient = graphClient;
        _configuration = configuration;
        _logger = loggerFactory.CreateLogger<ChangeNotificationSubscriber>();        
    }
    
    [Function("ChangeNotificationSubscriber")]
    public async Task Run([TimerTrigger("0 */30 * * * *")] TimerInfo timerInfo)
    {
        var hostName = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");        
        var section = _configuration.GetSection("Graph");

        var clientState = section["SecretClientState"];
        var encryptionCertificate = section["EncryptionCertificate"];
        var encryptionCertificateId = section["EncryptionCertificateId"];

        _logger.LogInformation("Hostname: {hostName}", hostName);

        var newSubscription = new Subscription
        {
            ChangeType = ChangeType.Created.ToString(),
            NotificationUrl = $"https://{hostName}/api/ChangeNotificationReceiver",
            Resource = "communications/onlineMeetings/getAllRecordings",
            IncludeResourceData = false,
            EncryptionCertificate = encryptionCertificate,
            EncryptionCertificateId = encryptionCertificateId,
            ExpirationDateTime = DateTime.UtcNow.AddHours(1),
            ClientState = clientState
        };

        var result = await _graphClient.Subscriptions.PostAsync(newSubscription);

        _logger.LogInformation("Subscription with Id '{subscriptionId}' for graph change notifications created", result.Id);
    }
}

