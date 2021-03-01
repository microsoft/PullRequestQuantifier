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

            foreach (var payloadRepository in payload.Repositories)
            {
                var repoDirectory = Guid.NewGuid().ToString();
                var clonePath = fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), repoDirectory);

                try
                {
                    var installationToken = await GetInstallationToken(payload);
                    var dnsSafeHost = new Uri(payload.Installation.HtmlUrl).DnsSafeHost;
                    Repository.Clone(
                        $"https://x-access-token:{installationToken}@{dnsSafeHost}/{payload.Installation.Account.Login}/{payloadRepository.Name}.git",
                        clonePath);
                    await Tools.QuantifyRepositories.Program.Main(new[] { "-repoPath", clonePath });
                    using var streamReader = new StreamReader(fileSystem.Path.Combine(clonePath, $"{repoDirectory}_QuantifierResults.csv"));
                    using var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture);
                    csv.Context.RegisterClassMap<CommitStatsMap>();
                    var repositoryStats = csv.GetRecords<CommitStats>();
                    repositoryStats = repositoryStats.Select(
                        r =>
                        {
                            r.PartitionKey = $"{payload.Installation.Account.Id}-{payloadRepository.Id}";
                            r.RowKey = r.CommitSha1;
                            return r;
                        }).ToList();

                    await blobStorage.CreateTableAsync(nameof(CommitStats));
                    await blobStorage.InsertOrReplaceTableEntitiesAsync(nameof(CommitStats), repositoryStats);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                fileSystem.DeleteDirectory(clonePath);
            }
        }

        private async Task<string> GetInstallationToken(InstallationEventPayload payload)
        {
            var gitHubClientAdapter =
                await gitHubClientAdapterFactory.GetGitHubClientAdapterAsync(
                    payload.Installation.Id,
                    new Uri(payload.Sender.HtmlUrl).DnsSafeHost);
            return gitHubClientAdapter.Credentials.Password;
        }
    }
}
