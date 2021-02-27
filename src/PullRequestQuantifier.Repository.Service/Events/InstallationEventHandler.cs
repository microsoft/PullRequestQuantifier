namespace PullRequestQuantifier.Repository.Service.Events
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Abstractions;
    using System.Threading.Tasks;
    using GitHubJwt;
    using LibGit2Sharp;
    using Microsoft.Extensions.Logging;
    using PullRequestQuantifier.Common.Azure.Telemetry;
    using PullRequestQuantifier.Common.Extensions;
    using PullRequestQuantifier.GitHub.Common.Events;
    using PullRequestQuantifier.GitHub.Common.GitHubClient;
    using PullRequestQuantifier.GitHub.Common.Models;

    public class InstallationEventHandler : IGitHubEventHandler
    {
        private readonly IAppTelemetry telemetry;
        private readonly ILogger<InstallationEventHandler> logger;
        private readonly IGitHubClientAdapterFactory gitHubClientAdapterFactory;
        private readonly IFileSystem fileSystem;

        public InstallationEventHandler(
            IAppTelemetry telemetry,
            ILogger<InstallationEventHandler> logger,
            IGitHubClientAdapterFactory gitHubClientAdapterFactory,
            IFileSystem fileSystem)
        {
            this.telemetry = telemetry;
            this.logger = logger;
            this.gitHubClientAdapterFactory = gitHubClientAdapterFactory;
            this.fileSystem = fileSystem;
        }

        public GitHubEventActions EventType { get; } = GitHubEventActions.Installation;

        public async Task HandleEvent(string gitHubEvent)
        {
            var payload =
                new Octokit.Internal.SimpleJsonSerializer().Deserialize<InstallationEventPayload>(gitHubEvent);

            foreach (var payloadRepository in payload.Repositories)
            {
                var clonePath = fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), Guid.NewGuid().ToString());

                try
                {
                    var installationToken = await GetInstallationToken(payload);
                    var dnsSafeHost = new Uri(payload.Installation.HtmlUrl).DnsSafeHost;
                    Repository.Clone(
                        $"https://x-access-token:{installationToken}@{dnsSafeHost}/{payload.Installation.Account.Login}/{payloadRepository.Name}.git",
                        clonePath);
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
