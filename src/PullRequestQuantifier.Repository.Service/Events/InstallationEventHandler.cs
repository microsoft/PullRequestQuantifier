namespace PullRequestQuantifier.GitHub.Client.Events
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using PullRequestQuantifier.Common.Azure.Telemetry;
    using PullRequestQuantifier.GitHub.Common.Events;
    using PullRequestQuantifier.GitHub.Common.GitHubClient;
    using PullRequestQuantifier.GitHub.Common.Models;

    public class InstallationEventHandler : IGitHubEventHandler
    {
        private readonly IGitHubClientAdapterFactory gitHubClientAdapterFactory;
        private readonly IAppTelemetry telemetry;
        private readonly ILogger<InstallationEventHandler> logger;

        public InstallationEventHandler(
            IGitHubClientAdapterFactory gitHubClientAdapterFactory,
            IAppTelemetry telemetry,
            ILogger<InstallationEventHandler> logger)
        {
            this.gitHubClientAdapterFactory = gitHubClientAdapterFactory;
            this.telemetry = telemetry;
            this.logger = logger;
        }

        public GitHubEventActions EventType { get; } = GitHubEventActions.Installation;

        public Task HandleEvent(string gitHubEvent)
        {
            var payload =
                new Octokit.Internal.SimpleJsonSerializer().Deserialize<InstallationEventPayload>(gitHubEvent);

            // todo call handle clone,stats
            return Task.CompletedTask;
        }
    }
}
