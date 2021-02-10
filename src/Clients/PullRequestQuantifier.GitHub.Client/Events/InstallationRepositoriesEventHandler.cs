namespace PullRequestQuantifier.GitHub.Client.Events
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using PullRequestQuantifier.GitHub.Client.GitHubClient;
    using PullRequestQuantifier.GitHub.Client.Models;
    using PullRequestQuantifier.GitHub.Client.Telemetry;

    public class InstallationRepositoriesEventHandler : IGitHubEventHandler
    {
        private readonly IGitHubClientAdapterFactory gitHubClientAdapterFactory;
        private readonly IAppTelemetry telemetry;
        private readonly ILogger<PullRequestEventHandler> logger;

        public InstallationRepositoriesEventHandler(
            IGitHubClientAdapterFactory gitHubClientAdapterFactory,
            IAppTelemetry telemetry,
            ILogger<PullRequestEventHandler> logger)
        {
            this.gitHubClientAdapterFactory = gitHubClientAdapterFactory;
            this.telemetry = telemetry;
            this.logger = logger;
        }

        public GitHubEventActions EventType { get; } = GitHubEventActions.Installation_Repositories;

        public Task HandleEvent(string gitHubEvent)
        {
            var payload =
                new Octokit.Internal.SimpleJsonSerializer().Deserialize<InstallationRepositoriesEventPayload>(gitHubEvent);

            logger.LogInformation(
                "Installation_Repositories event: {accountId} | {accountLogin} | {accountUrl} | {accountType} | " +
                "{action} | {repositorySelection} | {newRepositorySelection} | {repositoriesAdded} | {repositoriesRemoved}",
                payload.Installation.Account.Id,
                payload.Installation.Account.Login,
                payload.Installation.Account.Url,
                payload.Installation.Account.Type.ToString(),
                payload.Action,
                payload.Installation.RepositorySelection,
                payload.RepositorySelection,
                string.Join(" | ", payload.RepositoriesAdded.Select(r => r.FullName)),
                string.Join(" | ", payload.RepositoriesRemoved.Select(r => r.FullName)));
            telemetry.RecordMetric(
                "Installation-Event",
                1,
                ("Action", payload.Action));

            return Task.CompletedTask;
        }
    }
}
