namespace PullRequestQuantifier.GitHub.Client.Events
{
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
    using PullRequestQuantifier.GitHub.Client.GitHubClient;
    using PullRequestQuantifier.GitHub.Client.Models;
    using PullRequestQuantifier.GitHub.Client.Telemetry;

    public class PullRequestEventHandler : IGitHubEventHandler
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
                await gitHubClientAdapterFactory.GetGitHubClientAdapterForInstallationAsync(payload.Installation.Id);

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

            // get context if present
            string context = null;
            try
            {
                var rawContext = await gitHubClientAdapter.GetRawFileAsync(
                    payload.Repository.Owner.Login,
                    payload.Repository.Name,
                    "/prquantifier.yaml");
                context = Encoding.UTF8.GetString(rawContext);
            }
            catch (NotFoundException)
            {
            }
            catch
            {
                // ignored
            }

            var quantifyClient = new QuantifyClient(context);
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

            // get existing comments created by us
            var existingComments = await gitHubClientAdapter.GetIssueCommentsAsync(payload.Repository.Id, payload.PullRequest.Number);
            var existingCommentsCreatedByUs = existingComments.Where(ec => ec.User.Login.Equals($"{gitHubClientAdapter.GitHubAppSettings.Name}[bot]"));
            foreach (var existingComment in existingCommentsCreatedByUs)
            {
                await gitHubClientAdapter.DeleteIssueCommentAsync(payload.Repository.Id, existingComment.Id);
            }

            // create a comment on the issue
            var defaultBranch = payload.Repository.DefaultBranch;
            var quantifierContextLink = $"{payload.Repository.HtmlUrl}/blob/{defaultBranch}/prquantifier.yaml";
            var comment = await quantifierClientResult.ToMarkdownCommentAsync(
                payload.Repository.HtmlUrl,
                quantifierContextLink,
                payload.PullRequest.HtmlUrl,
                payload.PullRequest.User.Login);
            await gitHubClientAdapter.CreateIssueCommentAsync(
                payload.Repository.Id,
                payload.PullRequest.Number,
                comment);
            return quantifierClientResult;
        }

        private string ConvertToHex(Color c)
        {
            return c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
    }
}
