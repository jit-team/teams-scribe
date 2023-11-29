namespace TeamsScribe.ApiService;

public class AzureBlobStorageSettings
{
    public const string SectionName = "BlobStorage";

    public string ConnectionString { get; set; }
    public string Container { get; set; }
}
