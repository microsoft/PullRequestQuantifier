namespace PullRequestQuantifier.Repository.Service.Events
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;
    using System.Threading.Tasks;
    using CsvHelper;
    using LibGit2Sharp;
    using Microsoft.Extensions.Logging;
    using PullRequestQuantifier.Common.Azure.BlobStorage;
    using PullRequestQuantifier.Common.Azure.Telemetry;
    using PullRequestQuantifier.Common.Extensions;
    using PullRequestQuantifier.GitHub.Common.Events;
    using PullRequestQuantifier.GitHub.Common.GitHubClient;
    using PullRequestQuantifier.GitHub.Common.Models;
    using PullRequestQuantifier.Repository.Service.Models;

    public class InstallationEventHandler : IGitHubEventHandler
    {
        private readonly IAppTelemetry telemetry;
        private readonly ILogger<InstallationEventHandler> logger;
        private readonly IGitHubClientAdapterFactory gitHubClientAdapterFactory;
        private readonly IFileSystem fileSystem;
        private readonly IBlobStorage blobStorage;

        public InstallationEventHandler(
            IAppTelemetry telemetry,
            ILogger<InstallationEventHandler> logger,
            IGitHubClientAdapterFactory gitHubClientAdapterFactory,
            IFileSystem fileSystem,
            IBlobStorage blobStorage)
        {
            this.telemetry = telemetry;
            this.logger = logger;
            this.gitHubClientAdapterFactory = gitHubClientAdapterFactory;
            this.fileSystem = fileSystem;
            this.blobStorage = blobStorage;
        }

        public GitHubEventActions EventType { get; } = GitHubEventActions.Installation;

        public async Task HandleEvent(string gitHubEvent)
        {
            var payload =
                new Octokit.Internal.SimpleJsonSerializer().Deserialize<InstallationEventPayload>(gitHubEvent);
            if (Enum.TryParse(
                payload.Action,
                true,
                out GitHubEventActions parsedAction) && parsedAction != GitHubEventActions.Created)
            {
                logger.LogInformation("Ignoring installation event with {action} action", payload.Action);
                return;
            }

            logger.LogInformation("Handling installation event for {account}", payload.Installation.Account.Login);
            foreach (var payloadRepository in payload.Repositories)
            {
                if (payloadRepository.Fork)
                {
                    logger.LogInformation(
                        "Ignoring forked repo {account}/{repository}",
                        payload.Installation.Account.Login,
                        payloadRepository.Name);
                    continue;
                }

                var repoDirectory = Guid.NewGuid().ToString();
                var clonePath = fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), repoDirectory);

                try
                {
                    var dnsSafeHost = new Uri(payload.Installation.HtmlUrl).DnsSafeHost;
                    var gitHubClientAdapter =
                        await gitHubClientAdapterFactory.GetGitHubClientAdapterAsync(
                            payload.Installation.Id,
                            dnsSafeHost);
                    var installationToken = gitHubClientAdapter.Credentials.Password;
                    logger.LogInformation(
                        "Cloning repository {account}/{repository}",
                        payload.Installation.Account.Login,
                        payloadRepository.Name);
                    Repository.Clone(
                        $"https://x-access-token:{installationToken}@{dnsSafeHost}/{payload.Installation.Account.Login}/{payloadRepository.Name}.git",
                        clonePath);

                    await Tools.QuantifyRepositories.Program.Main(new[] { "-repoPath", clonePath });

                    using var streamReader = new StreamReader(fileSystem.Path.Combine(clonePath, $"{repoDirectory}_QuantifierResults.csv"));
                    using var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture);
                    csv.Context.RegisterClassMap<CommitStatsMap>();
                    var commitStats = csv.GetRecords<CommitStats>();
                    commitStats = commitStats.Select(
                        r =>
                        {
                            r.PartitionKey = $"{payload.Installation.Account.Id}-{payloadRepository.Id}";
                            r.RowKey = r.CommitSha1;
                            return r;
                        }).ToList();
                    var commitStatsMap = commitStats.ToDictionary(c => c.CommitSha1);

                    // get all closed pull requests
                    var closedPrs = await gitHubClientAdapter.GetClosedPullRequestsAsync(payloadRepository.Id);
                    foreach (var pr in closedPrs)
                    {
                        var prLeadTime = pr.MergedAt?.Subtract(pr.CreatedAt);
                        if (prLeadTime != null && commitStatsMap.TryGetValue(pr.MergeCommitSha, out var commitStat))
                        {
                            commitStat.PullRequestLeadTime = (TimeSpan)prLeadTime;
                            commitStat.PullRequestId = pr.Id;
                            commitStat.PullRequestAuthor = pr.User.Login;
                        }
                    }

                    // upload only the commits for which there was a PR
                    var commitStatsToUpload = commitStatsMap.Values.Where(c => c.PullRequestId != 0).ToList();

                    logger.LogInformation(
                        "Calculated {commitCount} commits to upload for {account}/{repository}",
                        commitStatsToUpload.Count,
                        payload.Installation.Account.Login,
                        payloadRepository.Name);
                    await blobStorage.CreateTableAsync(nameof(CommitStats));
                    await blobStorage.InsertOrReplaceTableEntitiesAsync(nameof(CommitStats), commitStatsToUpload);
                }
                catch (Exception e)
                {
                    logger.LogError(
                        e,
                        "Error during processing installation event for {account}/{repository}",
                        payload.Installation.Account.Login,
                        payloadRepository.Name);
                    throw;
                }
                finally
                {
                    fileSystem.DeleteDirectory(clonePath);
                }
            }
        }
    }
}
