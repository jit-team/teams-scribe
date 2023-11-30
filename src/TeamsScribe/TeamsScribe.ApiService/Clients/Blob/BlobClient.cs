using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;

namespace TeamsScribe.ApiService;

public class BlobClient
{
    private readonly BlobContainerClient _client;

    public BlobClient(IOptions<AzureBlobStorageSettings> settings)
    {
        _client = new BlobServiceClient(settings.Value.ConnectionString).GetBlobContainerClient(settings.Value.Container); 
    }

    public async Task<string> FetchTranscript(string blobPath, CancellationToken cancellationToken)
    {
        var result = await _client.GetBlobClient(blobPath).DownloadContentAsync(cancellationToken);
        return result.Value.Content.ToString();
    }
}
