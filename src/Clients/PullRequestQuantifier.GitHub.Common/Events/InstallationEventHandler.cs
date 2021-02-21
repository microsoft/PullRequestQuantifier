namespace PullRequestQuantifier.GitHub.Common.Events
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using PullRequestQuantifier.Common.Azure.Telemetry;
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

            logger.LogInformation(
                "Installation event: {accountId} | {accountLogin} | {accountUrl} | {accountType} | " +
                "{action} | {repositorySelection} | {repositories}",
                payload.Installation.Account.Id,
                payload.Installation.Account.Login,
                payload.Installation.Account.Url,
                payload.Installation.Account.Type.ToString(),
                payload.Action,
                payload.Installation.RepositorySelection,
                payload.Repositories != null ? string.Join(" , ", payload.Repositories.Select(r => r.FullName)) : string.Empty);
            telemetry.RecordMetric(
                "Installation-Event",
                1,
                ("Action", payload.Action));

            return Task.CompletedTask;
        }
    }
}
