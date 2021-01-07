namespace PullRequestQuantifier.GitHub.Client.GitHubClient
{
    public sealed class GitHubAppSettings
    {
        public string EnterpriseUrl { get; set; }

        public string EnterpriseApiRoot { get; set; }

        // todo this  should be renamed to ClientID
        public string GitHubAppId { get; set; }

        public int AppIntegrationId { get; set; }

        public string GitHubAppName { get; set; }

        public string GitHubAppSecret { get; set; }

        public string GitHubAppPrivateKeySecretName { get; set; }
    }
}
