using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TeamsScribe.Functions.ChangeNotifications.Handlers.Transcriptions;
using TeamsScribe.Functions.Registrations;
using System.Net.Http.Headers;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddGraphClient();
        services.AddScoped<MeetingTranscriptionDownloader>();        

        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(hostContext.Configuration.GetSection("BlobStorage")["ConnectionString"]).WithName("teamsScribeBlob");
        });

        services.AddHttpClient("TeamsScribeApi", httpClient => 
        {
            httpClient.BaseAddress = new Uri(hostContext.Configuration.GetSection("TeamsScribe")["ApiUrl"]);
            
            var apiKey = hostContext.Configuration.GetSection("TeamsScribe")["ApiKey"];
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("ApiKey", apiKey);
        });
    })
    .Build();

host.Run();
