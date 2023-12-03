# teams-scribe

Teams-Scribe is an AI based application designed specifically for Microsoft Teams users, offering two key functions: 
- generating meeting minutes based on transcriptions from the Teams application
- automatically sending the generated summary to participants via email.

[Link to app](https://gray-pebble-0fa116603.4.azurestaticapps.net/)

## Tech stack

- Azure
    - Azure App Service
    - Azure Static Web Apps    
    - Azure Blob Storage
    - Azure OpenAI
    - Azure Communication Services
    - Graph API
- .NET 8, .NET Aspire, Blazor, ASP.NET Core Minimal API
- CI/CD
    - GitHub Actions

![image](https://github.com/jit-team/teams-scribe/assets/37310497/4f7f0fb9-1954-4b30-a1bc-bb270dcf4cc1)

## Setup

### Prerequisites

- Azure subscription (Work or school subscription)
- Microsoft Teams
- Microsoft Entra ID App Registration with below configuration
    - Client secret generated
    - API permissions (Application permissions to Graph API)
        - Calendars.Read        
        - OnlineMeeting.Read.All
        - OnlineMeetingTranscript.Read.All
        - User.Read.All
- Granting application access policy for accessing online meetings with application permissions
    - Teams PowerShell module
    - Article to follow: https://learn.microsoft.com/en-us/graph/cloud-communication-online-meeting-application-access-policy

### Required env variables for backend
```
ApiKey //key used to access backend

AzCommunicationServices__ConnectionString //connection string to Azure comminication services for sending emails

AzOpenAi__Key // Azure Open AI/OpenAI api key

AzOpenAi__Url // Azure Open AI/OpenAI url

AzureAd__ClientId //App registration client Id

AzureAd__ClientSecret //App registration client secret

AzureAd__TenantId //App registration tenant Id

BlobStorage__ConnectionString //Azure blob storage connection string

BlobStorage__Container //Azure blob storage container name 
```

### Required appsettings.json for frontend

```
{
    "ApiUrl": "https://backend-url.com"
}
```
