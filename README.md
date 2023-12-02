# teams-scribe

Teams-Scribe is an AI based application designed specifically for Microsoft Teams users, offering two key functions: 
- generating meeting minutes based on transcriptions from the Teams application
- automatically sending the generated summary to participants via email.

[Link to app](https://gray-pebble-0fa116603.4.azurestaticapps.net/)
## Setup

### Required env variables for backend
```
ApiKey //key used to access backend

AzCommunicationServices__ConnectionString //connection string to Azure comminication services for sending emails

AzOpenAi__Key // Azure Open AI/OpenAI api key

AzOpenAi__Url // Azure Open AI/OpenAI url

AzureAd__ClientId //App registration client Id

AzureAd__ClientSecret //App registration client secret

AzureAd__TenantId //App registration tennant Id

BlobStorage__ConnectionString //Azure blob storage connection string

BlobStorage__Container //Azure blob storage container name 
```

### Required appsettings.json for frontend

```
{
    "ApiUrl": "https://backend-url.com"
}
```
