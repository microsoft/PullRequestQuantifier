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
        Suspend = 1 << 7,
        Unsuspend = 1 << 8,
        New_Permissions_Accepted = 1 << 9,
        Pull_Request = Opened | Reopened | Synchronize,
        Installation = Created | Deleted | Suspend | Unsuspend | New_Permissions_Accepted,
        Installation_Repositories = Added | Removed
    }
}
