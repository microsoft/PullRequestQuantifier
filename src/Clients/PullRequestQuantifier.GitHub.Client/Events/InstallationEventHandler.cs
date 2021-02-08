namespace PullRequestQuantifier.GitHub.Client.Events
{
    using System.Threading.Tasks;
    using PullRequestQuantifier.GitHub.Client.GitHubClient;
    using PullRequestQuantifier.GitHub.Client.Models;

    public class InstallationEventHandler : IGitHubEventHandler
    {
        private readonly IGitHubClientAdapterFactory gitHubClientAdapterFactory;

        public InstallationEventHandler(IGitHubClientAdapterFactory gitHubClientAdapterFactory)
        {
            this.gitHubClientAdapterFactory = gitHubClientAdapterFactory;
        }

        public GitHubEventActions EventType { get; } = GitHubEventActions.Installation;

        public Task HandleEvent(string gitHubEvent)
        {
            // TODO: handle installation events
            return Task.CompletedTask;
        }
    }
}
