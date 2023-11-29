using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TeamsScribe.Functions.ChangeNotifications.Handlers.Transcriptions;
using TeamsScribe.Functions.Registrations;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddGraphClient();
        services.AddScoped<TranscriptionNotificationHandler>();

        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(hostContext.Configuration.GetSection("BlobStorage")["ConnectionString"]).WithName("teamsScribeBlob");
        });
    })
    .Build();

host.Run();
