namespace PullRequestQuantifier.GitHub.Client.Events
{
    using System;
    using System.Drawing;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Octokit;
    using PullRequestQuantifier.Abstractions.Context;
    using PullRequestQuantifier.Abstractions.Core;
    using PullRequestQuantifier.Abstractions.Git;
    using PullRequestQuantifier.Client.QuantifyClient;
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

            // get pull request
            var pullRequest = await gitHubClientAdapter.GetPullRequestAsync(
                payload.Repository.Id,
                payload.PullRequest.Number);

            // get pull request files
            var pullRequestFiles = await gitHubClientAdapter.GetPullRequestFilesAsync(
                payload.Repository.Id,
                payload.PullRequest.Number);

            // convert to quantifier input
            var quantifierInput = new QuantifierInput();
            foreach (var pullRequestFile in pullRequestFiles)
            {
                if (pullRequestFile.Patch == null)
                {
                    continue;
                }

                var changeType = GitChangeType.Modified;
                switch (pullRequestFile.Status)
                {
                    case "modified":
                        break;
                    case "added":
                        changeType = GitChangeType.Added;
                        break;
                    case "deleted":
                        changeType = GitChangeType.Deleted;
                        break;
                }

                var gitFilePatch = new GitFilePatch
                {
                    ChangeType = changeType,
                    AbsoluteLinesAdded = pullRequestFile.Additions,
                    AbsoluteLinesDeleted = pullRequestFile.Deletions,
                    DiffContent = pullRequestFile.Patch,
                    FilePath = pullRequestFile.FileName,
                    DiffContentLines = pullRequestFile.Patch.Split("\n")
                };
                quantifierInput.Changes.Add(gitFilePatch);
            }

            // get context if present
            string context = null;
            try
            {
                var rawContext = await gitHubClientAdapter.GetRawFileAsync(
                    payload.Repository.Owner.Login,
                    payload.Repository.Name,
                    "/.prquantifier");
                context = Encoding.UTF8.GetString(rawContext);
            }
            catch (Octokit.NotFoundException)
            {
            }
            catch (Exception)
            {
                // ignored
            }

            var quantifyClient = new QuantifyClient(
                context,
                QuantifyClientOutput.SummaryByExt);
            var quantifierClientResult = await quantifyClient.Compute(quantifierInput);

            // create a new label in the repository if doesn't exist
            try
            {
                var existingLabel = await gitHubClientAdapter.GetLabelAsync(
                    payload.Repository.Id,
                    quantifierClientResult.Label);
            }
            catch (NotFoundException)
            {
                // create new label
                var color = Color.FromName(quantifierClientResult.Color);
                await gitHubClientAdapter.CreateLabelAsync(
                    payload.Repository.Id,
                    new NewLabel(quantifierClientResult.Label, ConvertToHex(color)));
            }

            // apply label to pull request
            await gitHubClientAdapter.ApplyLabelAsync(
                payload.Repository.Id,
                payload.PullRequest.Number,
                new[] { quantifierClientResult.Label });
        }

        private Task ErrorHandler(Exception exception)
        {
            return Task.CompletedTask;
        }

        private string ConvertToHex(Color c)
        {
            return c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
    }
}