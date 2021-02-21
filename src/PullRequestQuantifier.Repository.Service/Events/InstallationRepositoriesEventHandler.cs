namespace PullRequestQuantifier.GitHub.Client.Events
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using PullRequestQuantifier.Common.Azure.Telemetry;
    using PullRequestQuantifier.GitHub.Common.Events;
    using PullRequestQuantifier.GitHub.Common.Models;

    public class InstallationRepositoriesEventHandler : IGitHubEventHandler
    {
        private readonly IAppTelemetry telemetry;
        private readonly ILogger<InstallationRepositoriesEventHandler> logger;

        public InstallationRepositoriesEventHandler(
            IAppTelemetry telemetry,
            ILogger<InstallationRepositoriesEventHandler> logger)
        {
            this.telemetry = telemetry;
            this.logger = logger;
        }

        public GitHubEventActions EventType { get; } = GitHubEventActions.Installation_Repositories;

        public Task HandleEvent(string gitHubEvent)
        {
            var payload =
                new Octokit.Internal.SimpleJsonSerializer().Deserialize<InstallationRepositoriesEventPayload>(gitHubEvent);

            // todo call handle clone,stats
            return Task.CompletedTask;
        }
    }
}
