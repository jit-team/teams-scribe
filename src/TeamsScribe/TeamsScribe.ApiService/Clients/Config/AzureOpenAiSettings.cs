using System;

namespace TeamsScribe.ApiService.Clients.Config
{
    public class AzureOpenAiSettings
    {
        public const string SectionName = "AzOpenAi";
        public string Url { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
    }
}
