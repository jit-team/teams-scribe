using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;

namespace TeamsScribe.Functions.Registrations;

public static class GraphClientRegistration
{
    private static readonly string[] scopes = ["https://graph.microsoft.com/.default"];

    public static IServiceCollection AddGraphClient(this IServiceCollection services)
    {
        return services.AddSingleton((sp) =>
        {
            var section = sp.GetService<IConfiguration>().GetSection("AzureAd");

            var tenantId = section["TenantId"];
            var clientId = section["ClientId"];
            var clientSecret = section["ClientSecret"];

            var options = new ClientSecretCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
            };

            var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret, options);

            return new GraphServiceClient(clientSecretCredential, scopes);
        });
    }
}
