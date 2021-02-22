namespace PullRequestQuantifier.GitHub.Common.GitHubClient
{
    public sealed class GitHubAppSettings
    {
        public string EnterpriseUrl { get; set; }

        public string EnterpriseApiRoot { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string PrivateKey { get; set; }

        public string WebhookSecret { get; set; }
    }
}
