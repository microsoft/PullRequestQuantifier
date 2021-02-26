namespace PullRequestQuantifier.Repository.Service.Events
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using GitHubJwt;
    using LibGit2Sharp;
    using Microsoft.Extensions.Logging;
    using PullRequestQuantifier.Common.Azure.Telemetry;
    using PullRequestQuantifier.GitHub.Common.Events;
    using PullRequestQuantifier.GitHub.Common.Models;

    public class InstallationEventHandler : IGitHubEventHandler
    {
        private readonly IReadOnlyDictionary<string, IGitHubJwtFactory> gitHubJwtFactories;
        private readonly IAppTelemetry telemetry;
        private readonly ILogger<InstallationEventHandler> logger;

        public InstallationEventHandler(
            IReadOnlyDictionary<string, IGitHubJwtFactory> gitHubJwtFactories,
            IAppTelemetry telemetry,
            ILogger<InstallationEventHandler> logger)
        {
            this.gitHubJwtFactories = gitHubJwtFactories;
            this.telemetry = telemetry;
            this.logger = logger;
        }

        public GitHubEventActions EventType { get; } = GitHubEventActions.Installation;

        public Task HandleEvent(string gitHubEvent)
        {
            var payload =
                new Octokit.Internal.SimpleJsonSerializer().Deserialize<InstallationEventPayload>(gitHubEvent);
            var cloneOptions = GetCloneOptions(payload);

            foreach (var payloadRepository in payload.Repositories)
            {
                var clonePath = Path.Combine(Environment.CurrentDirectory, Guid.NewGuid().ToString());

                try
                {
                    Repository.Clone(
                        $"{payload.Sender.HtmlUrl}/{payloadRepository.Name}.git",
                        clonePath,
                        cloneOptions);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                Directory.Delete(clonePath);
            }

            return Task.CompletedTask;
        }

        private CloneOptions GetCloneOptions(InstallationEventPayload payload)
        {
            var token = gitHubJwtFactories[new Uri(payload.Sender.HtmlUrl).DnsSafeHost].CreateEncodedJwtToken();
            return new CloneOptions
            {
                CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials
                {
                    Username = "PAT",
                    Password = token
                }, BranchName = "master"
            };
        }
    }
}
