namespace PullRequestQuantifier.GitHub.Common.GitHubClient
{
    using System.Collections.Generic;

    public sealed class GitHubAppFlavorSettings
    {
        public Dictionary<string, GitHubAppSettings> GitHubAppsSettings { get; set; }

        public GitHubAppSettings this[string dnsHost] => GitHubAppsSettings[dnsHost];
    }
}
