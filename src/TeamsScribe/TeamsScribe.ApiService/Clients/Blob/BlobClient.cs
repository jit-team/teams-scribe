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

    public async Task<string> FetchTranscript(string blobPath)
    {
        var result = await _client.GetBlobClient(blobPath).DownloadContentAsync();
        return result.Value.Content.ToString();
    }
}
