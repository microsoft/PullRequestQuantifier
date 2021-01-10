namespace PullRequestQuantifier.GitHub.Client.GitHubClient
***REMOVED***
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GitHubJwt;
    using Microsoft.Extensions.Options;
    using Octokit;
    using Octokit.Internal;
    using PullRequestQuantifier.GitHub.Client.GitHubClient.Exceptions;
    using PullRequestQuantifier.GitHub.Client.Telemetry;

    /// <inheritdoc cref="IGitHubClientAdapterFactory"/>
    public sealed class GitHubClientAdapterFactory : IGitHubClientAdapterFactory
    ***REMOVED***
        private readonly GitHubAppSettings gitHubAppSettings;
        private readonly IGitHubJwtFactory gitHubJwtFactory;
        private readonly IAppTelemetry appTelemetry;
        private GitHubClient gitHubClient;

        public GitHubClientAdapterFactory(
            IGitHubJwtFactory gitHubJwtFactory,
            IOptions<GitHubAppSettings> gitHubAppSettings,
            IAppTelemetry appTelemetry)
        ***REMOVED***
            this.gitHubJwtFactory = gitHubJwtFactory;
            this.appTelemetry = appTelemetry;
            this.gitHubAppSettings = gitHubAppSettings.Value;
***REMOVED***

        /// <inheritdoc />
        public async Task<IGitHubClientAdapter> GetGitHubClientAdapterAsync(long installationId)
        ***REMOVED***
            gitHubClient = CreateClient(
                new Credentials(gitHubJwtFactory.CreateEncodedJwtToken(), AuthenticationType.Bearer),
                this.gitHubAppSettings,
                appTelemetry);
            var (token, expirationTime) =
                await CreateInstallationTokenByInstallationId(installationId, gitHubAppSettings);

            gitHubClient.Credentials = new Credentials(token, AuthenticationType.Bearer);
            return new GitHubClientAdapter(gitHubClient);
***REMOVED***

        private static GitHubClient CreateClient(
            Credentials credentials,
            GitHubAppSettings gitHubAppSettings,
            IAppTelemetry appTelemetry)
        ***REMOVED***
            try
            ***REMOVED***
                return new GitHubClient(
                    new Connection(
                        new ProductHeaderValue(gitHubAppSettings.Name),
                        new Uri(gitHubAppSettings.EnterpriseApiRoot),
                        new InMemoryCredentialStore(credentials),
                        new HttpClientAdapter(() => new GitHubClientMessageHandler(appTelemetry)),
                        new SimpleJsonSerializer()));
    ***REMOVED***
            catch (Exception ex)
            ***REMOVED***
                throw new CreateGitHubClientException(
                    $"Failed to create client for GitHubApp: ***REMOVED***gitHubAppSettings.Name***REMOVED***", ex);
    ***REMOVED***
***REMOVED***

        private async Task<(string, DateTimeOffset)> CreateInstallationTokenByInstallationId(
            long installationId,
            GitHubAppSettings gitHubAppSettings)
        ***REMOVED***
            try
            ***REMOVED***
                var installationToken = await gitHubClient.GitHubApps.CreateInstallationToken(installationId);
                return (installationToken.Token, installationToken.ExpiresAt);
    ***REMOVED***
            catch (NotFoundException ex)
            ***REMOVED***
                throw new CreateGitHubClientException(
                    $"GitHub installation (***REMOVED***installationId***REMOVED***) not found or insufficient access privileges.", ex);
    ***REMOVED***
            catch (Exception ex)
            ***REMOVED***
                throw new CreateGitHubClientException(
                    $"Failed to get access token for installation: ***REMOVED***installationId***REMOVED***, GitHubApp: ***REMOVED***gitHubAppSettings.Name***REMOVED***",
                    ex);
    ***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***