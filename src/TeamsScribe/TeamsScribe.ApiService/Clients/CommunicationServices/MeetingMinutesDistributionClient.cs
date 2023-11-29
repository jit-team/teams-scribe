using Azure.Communication.Email;
using Microsoft.Extensions.Options;

namespace TeamsScribe.ApiService;

public class MeetingMinutesDistributionClient
{
    private const string SENDER_EMAIL_ADDRESS = "DoNotReply@d0720e39-e7a6-46c1-9e15-f39d5e482ac4.azurecomm.net";

    private readonly EmailClient _emailClient;
    private readonly ILogger<MeetingMinutesDistributionClient> _logger;

    public MeetingMinutesDistributionClient(
        IOptions<CommunicationServicesSettings> options,
        ILogger<MeetingMinutesDistributionClient> logger)
    {
        _emailClient = new EmailClient(options.Value!.ConnectionString);
    }

    public async Task SendAsync(MeetingMinutesEmailPayload payload)
    {
        try
        {
            var content = new EmailContent($"Meeting minutes from \"{payload.Title}\"")
            {
                PlainText = payload.Minutes
            };

            var recipients = new EmailRecipients(new[] { new EmailAddress(payload.Organizer)}, payload.Recipients.Select(r => new EmailAddress(r)));

            var message = new EmailMessage(
                senderAddress: SENDER_EMAIL_ADDRESS,
                recipients,
                content
            );

            var sendingOperation = await _emailClient.SendAsync(Azure.WaitUntil.Completed, message);

            if (sendingOperation.Value.Status == EmailSendStatus.Failed)
            {
                var errorMessage = $"Last resposne payload: {sendingOperation.GetRawResponse().Content}";
                _logger.LogWarning("Failed to send meeting minutes email for \"{MeetintTitle}\" with error: {Error}", payload.Title, errorMessage);
                await SendErrorAsync(payload, errorMessage);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Exception occured during meeting notes email sending for \"{MeetingTitle}\"", payload.Title);
            await SendErrorAsync(payload, exception.ToString());
        }
    }

    private async Task SendErrorAsync(MeetingMinutesEmailPayload payload, string error)
    {
        try
        {
            var recipients = new EmailRecipients(new[] { new EmailAddress(payload.Organizer) });

            var content = new EmailContent($"Failed to send meeting minutes from \"{payload.Title}\"")
            {
                PlainText = error
            };

            var message = new EmailMessage(
                senderAddress: SENDER_EMAIL_ADDRESS,
                recipients,
                content
            );


            var sendingOperation = await _emailClient.SendAsync(Azure.WaitUntil.Completed, message);

            if (sendingOperation.Value.Status == EmailSendStatus.Failed)
            {
                var errorMessage = $"Last resposne payload: {sendingOperation.GetRawResponse().Content}";
                _logger.LogWarning("Failed to send error email to \"{Owner}\" with error: {Error}", payload.Organizer, errorMessage);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Excpetion occured while sending error email to {Owner}", payload.Organizer);
        }
    }
}
