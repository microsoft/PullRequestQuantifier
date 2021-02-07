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
        Added = 1 << 4,
        Deleted = 1 << 5,
        Removed = 1 << 6,
        Pull_Request = Opened | Reopened | Synchronize,
        Installation = Created | Added | Deleted | Removed
    }
}
