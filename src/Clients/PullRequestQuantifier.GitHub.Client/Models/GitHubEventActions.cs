namespace PullRequestQuantifier.GitHub.Client.Models
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GitHubEventActions
    {
        Opened = 1 << 0,
        Synchronize = 1 << 1,
        Reopened = 1 << 2,
        Created = 1 << 3,
        Pull_Request = Opened | Reopened | Synchronize,
        Installation = Created
    }
}
