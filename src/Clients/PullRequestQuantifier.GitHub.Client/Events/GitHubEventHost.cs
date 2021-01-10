namespace PullRequestQuantifier.GitHub.Client.Events
***REMOVED***
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Octokit;
    using PullRequestQuantifier.GitHub.Client.GitHubClient;
    using PullRequestQuantifier.GitHub.Client.Telemetry;

    public class GitHubEventHost : IHostedService
    ***REMOVED***
        private readonly IEventBus eventBus;
        private readonly IAppTelemetry appTelemetry;
        private readonly IGitHubClientAdapterFactory gitHubClientAdapterFactory;

        public GitHubEventHost(
            IEventBus eventBus,
            IAppTelemetry appTelemetry,
            IGitHubClientAdapterFactory gitHubClientAdapterFactory)
        ***REMOVED***
            this.eventBus = eventBus;
            this.appTelemetry = appTelemetry;
            this.gitHubClientAdapterFactory = gitHubClientAdapterFactory;
***REMOVED***

        public async Task StartAsync(CancellationToken cancellationToken)
        ***REMOVED***
            await eventBus.SubscribeAsync(
                QuantifyPullRequest,
                ErrorHandler,
                cancellationToken);
***REMOVED***

        public Task StopAsync(CancellationToken cancellationToken)
        ***REMOVED***
            return Task.CompletedTask;
***REMOVED***

        private async Task QuantifyPullRequest(string pullRequestEvent)
        ***REMOVED***
            var payload =
                new Octokit.Internal.SimpleJsonSerializer().Deserialize<PullRequestEventPayload>(pullRequestEvent);

            var gitHubClientAdapter =
                await gitHubClientAdapterFactory.GetGitHubClientAdapterAsync(payload.Installation.Id);

            // get pull request files
            var pullRequestFiles = await gitHubClientAdapter.GetPullRequestFiles(
                payload.Repository.Id,
                payload.PullRequest.Number);

            // convert to quantifier input
***REMOVED***

        private Task ErrorHandler(Exception exception)
        ***REMOVED***
            return Task.CompletedTask;
***REMOVED***
***REMOVED***
***REMOVED***