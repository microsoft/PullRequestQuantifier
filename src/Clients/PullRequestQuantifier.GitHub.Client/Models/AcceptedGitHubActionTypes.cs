namespace PullRequestQuantifier.GitHub.Client.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AcceptedGitHubActionTypes
    {
        Opened = 1,
        Synchronize,
        Reopened
    }
}
