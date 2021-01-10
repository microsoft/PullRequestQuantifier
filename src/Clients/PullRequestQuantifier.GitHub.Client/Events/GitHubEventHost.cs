namespace PullRequestQuantifier.GitHub.Client.Events
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Octokit;
    using PullRequestQuantifier.GitHub.Client.GitHubClient;
    using PullRequestQuantifier.GitHub.Client.Telemetry;

    public class GitHubEventHost : IHostedService
    {
        private readonly IEventBus eventBus;
        private readonly IAppTelemetry appTelemetry;
        private readonly IGitHubClientAdapterFactory gitHubClientAdapterFactory;

        public GitHubEventHost(
            IEventBus eventBus,
            IAppTelemetry appTelemetry,
            IGitHubClientAdapterFactory gitHubClientAdapterFactory)
        {
            this.eventBus = eventBus;
            this.appTelemetry = appTelemetry;
            this.gitHubClientAdapterFactory = gitHubClientAdapterFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await eventBus.SubscribeAsync(
                QuantifyPullRequest,
                ErrorHandler,
                cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task QuantifyPullRequest(string pullRequestEvent)
        {
            var payload =
                new Octokit.Internal.SimpleJsonSerializer().Deserialize<PullRequestEventPayload>(pullRequestEvent);

            var gitHubClientAdapter =
                await gitHubClientAdapterFactory.GetGitHubClientAdapterAsync(payload.Installation.Id);

            // get pull request files
            var pullRequestFiles = await gitHubClientAdapter.GetPullRequestFiles(
                payload.Repository.Id,
                payload.PullRequest.Number);

            // convert to quantifier input
        }

        private Task ErrorHandler(Exception exception)
        {
            return Task.CompletedTask;
        }
    }
}