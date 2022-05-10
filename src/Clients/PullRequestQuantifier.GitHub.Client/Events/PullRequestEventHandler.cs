namespace PullRequestQuantifier.GitHub.Client.Events
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Octokit;
    using PullRequestQuantifier.Abstractions.Core;
    using PullRequestQuantifier.Abstractions.Git;
    using PullRequestQuantifier.Client.Extensions;
    using PullRequestQuantifier.Client.QuantifyClient;
    using PullRequestQuantifier.Common;
    using PullRequestQuantifier.Common.Azure.Telemetry;
    using PullRequestQuantifier.GitHub.Common.Events;
    using PullRequestQuantifier.GitHub.Common.GitHubClient;
    using PullRequestQuantifier.GitHub.Common.Models;

    public sealed class PullRequestEventHandler : IGitHubEventHandler
    {
        private readonly IGitHubClientAdapterFactory gitHubClientAdapterFactory;
        private readonly IAppTelemetry telemetry;
        private readonly ILogger<PullRequestEventHandler> logger;

        public PullRequestEventHandler(
            IGitHubClientAdapterFactory gitHubClientAdapterFactory,
            IAppTelemetry telemetry,
            ILogger<PullRequestEventHandler> logger)
        {
            this.gitHubClientAdapterFactory = gitHubClientAdapterFactory;
            this.telemetry = telemetry;
            this.logger = logger;
        }

        public GitHubEventActions EventType { get; } = GitHubEventActions.Pull_Request;

        public async Task HandleEvent(string gitHubEvent)
        {
            var payload =
                new Octokit.Internal.SimpleJsonSerializer().Deserialize<PullRequestEventPayload>(gitHubEvent);

            logger.LogInformation(
                "Quantifying pull request: {pullRequestUrl}|{pullRequestId}|{sha}",
                payload.PullRequest.HtmlUrl,
                payload.PullRequest.Id,
                payload.PullRequest.Head.Sha);
            telemetry.RecordMetric(
                "PullRequest-QuantifyRequest",
                1);

            var quantifierResult = await QuantifyPullRequest(payload);

            logger.LogInformation(
                "Quantified pull request: {pullRequestUrl}|{pullRequestId}|{sha}|" +
                "{label}|{formula}|{absoluteLinesAdded}|{absoluteLinesDeleted}|" +
                "{quantifiedLinesAdded}|{quantifiedLinesDeleted}|" +
                "{percentileAddition}|{percentileDeletion}|{formulaPercentile}",
                payload.PullRequest.HtmlUrl,
                payload.PullRequest.Id,
                payload.PullRequest.Head.Sha,
                quantifierResult.Label,
                quantifierResult.Formula,
                quantifierResult.QuantifierInput.Changes.Sum(c => c.AbsoluteLinesAdded),
                quantifierResult.QuantifierInput.Changes.Sum(c => c.AbsoluteLinesDeleted),
                quantifierResult.QuantifiedLinesAdded,
                quantifierResult.QuantifiedLinesDeleted,
                quantifierResult.PercentileAddition,
                quantifierResult.PercentileDeletion,
                quantifierResult.FormulaPercentile);
        }

        private async Task<QuantifierResult> QuantifyPullRequest(PullRequestEventPayload payload)
        {
            var gitHubClientAdapter =
                await gitHubClientAdapterFactory.GetGitHubClientAdapterAsync(
                    payload.Installation.Id,
                    new Uri(payload.PullRequest.HtmlUrl).DnsSafeHost);

            var quantifierInput = await GetQuantifierInputFromPullRequest(payload, gitHubClientAdapter);

            var contextResult = await GetContextFromRepoIfPresent(payload, gitHubClientAdapter);

            var quantifyClient = new QuantifyClient(contextResult.context);
            var quantifierClientResult = await quantifyClient.Compute(quantifierInput);
            var labels = quantifyClient.Context.Thresholds.Select(t => t.Label).Union(new[] { Constants.NoChangesLabelName });

            await ApplyLabelToPullRequest(
                payload,
                gitHubClientAdapter,
                quantifierClientResult,
                labels);

            var quantifierContextLink = !string.IsNullOrWhiteSpace(contextResult.context)
                ? $"{payload.Repository.HtmlUrl}/blob/{payload.Repository.DefaultBranch}/{contextResult.contextPath}"
                : string.Empty;
            await UpdateCommentOnPullRequest(
                payload,
                gitHubClientAdapter,
                quantifierClientResult,
                quantifierContextLink);
            return quantifierClientResult;
        }

        private async Task UpdateCommentOnPullRequest(
            PullRequestEventPayload payload,
            IGitHubClientAdapter gitHubClientAdapter,
            QuantifierResult quantifierClientResult,
            string quantifierContextLink)
        {
            // delete existing comments created by us
            var existingComments =
                await gitHubClientAdapter.GetIssueCommentsAsync(payload.Repository.Id, payload.PullRequest.Number);
            var existingCommentsCreatedByUs = existingComments.Where(
                ec =>
                    ec.User.Login.Equals($"{gitHubClientAdapter.GitHubAppSettings.Name}[bot]"));
            foreach (var existingComment in existingCommentsCreatedByUs)
            {
                await gitHubClientAdapter.DeleteIssueCommentAsync(payload.Repository.Id, existingComment.Id);
            }

            // create a new comment on the issue
            var comment = await quantifierClientResult.ToMarkdownCommentAsync(
                payload.Repository.HtmlUrl,
                quantifierContextLink,
                payload.PullRequest.HtmlUrl,
                payload.PullRequest.User.Login,
                ShouldPostAnonymousFeedbackLink(payload),
                new MarkdownCommentOptions { CollapsePullRequestQuantifiedSection = true });
            await gitHubClientAdapter.CreateIssueCommentAsync(
                payload.Repository.Id,
                payload.PullRequest.Number,
                comment);
        }

        // only post anonymous feedback link in case of github.com flavor
        private bool ShouldPostAnonymousFeedbackLink(PullRequestEventPayload payload)
        {
            return new Uri(payload.PullRequest.HtmlUrl).DnsSafeHost.Equals("github.com");
        }

        private async Task ApplyLabelToPullRequest(
            PullRequestEventPayload payload,
            IGitHubClientAdapter gitHubClientAdapter,
            QuantifierResult quantifierClientResult,
            IEnumerable<string> labelOptionsFromContext)
        {
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

            // remove any previous labels applied if it's different
            // labels do not have the property of who applied them
            // so we use string matching against the label options present in the context
            // if label strings have changed in context since last PR update, this will break
            var existingLabels =
                await gitHubClientAdapter.GetIssueLabelsAsync(payload.Repository.Id, payload.PullRequest.Number);
            var existingLabelsByUs = existingLabels.Select(el => el.Name).Intersect(labelOptionsFromContext).ToList();
            foreach (var existingLabel in existingLabelsByUs)
            {
                if (existingLabel == quantifierClientResult.Label)
                {
                    continue;
                }

                await gitHubClientAdapter.RemoveLabelFromIssueAsync(
                    payload.Repository.Id,
                    payload.PullRequest.Number,
                    existingLabel);
            }

            // apply new label to pull request
            await gitHubClientAdapter.ApplyLabelAsync(
                payload.Repository.Id,
                payload.PullRequest.Number,
                new[] { quantifierClientResult.Label });
        }

        private async Task<(string context, string contextPath)> GetContextFromRepoIfPresent(PullRequestEventPayload payload, IGitHubClientAdapter gitHubClientAdapter)
        {
            // get context if present
            byte[] rawContext = { };
            string contextPath = string.Empty;
            try
            {
                rawContext = await gitHubClientAdapter.GetRawFileAsync(
                    payload.Repository.Owner.Login,
                    payload.Repository.Name,
                    $"/{Constants.RootContextFilePath}");
                contextPath = Constants.RootContextFilePath;
            }
            catch (NotFoundException)
            {
                try
                {
                    // try reading from .github folder as a fallback
                    rawContext = await gitHubClientAdapter.GetRawFileAsync(
                        payload.Repository.Owner.Login,
                        payload.Repository.Name,
                        $"/{Constants.GitHubFolderContextFilePath}");
                    contextPath = Constants.GitHubFolderContextFilePath;
                }
                catch
                {
                    // ignored
                }
            }
            catch
            {
                // ignored
            }

            var context = Encoding.UTF8.GetString(rawContext);
            return (context, contextPath);
        }

        private async Task<QuantifierInput> GetQuantifierInputFromPullRequest(PullRequestEventPayload payload, IGitHubClientAdapter gitHubClientAdapter)
        {
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

                var fileExtension = !string.IsNullOrWhiteSpace(pullRequestFile.FileName)
                    ? new FileInfo(pullRequestFile.FileName).Extension
                    : string.Empty;
                var gitFilePatch = new GitFilePatch(
                    pullRequestFile.FileName,
                    fileExtension)
                {
                    ChangeType = changeType,
                    AbsoluteLinesAdded = pullRequestFile.Additions,
                    AbsoluteLinesDeleted = pullRequestFile.Deletions,
                    DiffContent = pullRequestFile.Patch,
                };
                quantifierInput.Changes.Add(gitFilePatch);
            }

            return quantifierInput;
        }

        private string ConvertToHex(Color c)
        {
            return c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
    }
}
