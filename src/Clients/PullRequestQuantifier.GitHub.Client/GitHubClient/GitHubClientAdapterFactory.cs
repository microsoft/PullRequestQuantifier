namespace PullRequestQuantifier.GitHub.Client.GitHubClient
{
    using System;
    using System.Threading.Tasks;
    using GitHubJwt;
    using Microsoft.Extensions.Options;
    using Octokit;
    using Octokit.Internal;
    using PullRequestQuantifier.GitHub.Client.GitHubClient.Exceptions;
    using PullRequestQuantifier.GitHub.Client.Telemetry;

    /// <inheritdoc cref="IGitHubClientAdapterFactory"/>
    public sealed class GitHubClientAdapterFactory : IGitHubClientAdapterFactory
    {
        private readonly GitHubAppSettings gitHubAppSettings;
        private readonly IGitHubJwtFactory gitHubJwtFactory;
        private readonly IAppTelemetry appTelemetry;
        private GitHubClient gitHubClient;

        public GitHubClientAdapterFactory(
            IGitHubJwtFactory gitHubJwtFactory,
            IOptions<GitHubAppSettings> gitHubAppSettings,
            IAppTelemetry appTelemetry)
        {
            this.gitHubJwtFactory = gitHubJwtFactory;
            this.appTelemetry = appTelemetry;
            this.gitHubAppSettings = gitHubAppSettings.Value;
        }

        /// <inheritdoc />
        public async Task<IGitHubClientAdapter> GetGitHubClientAdapterAsync(long installationId)
        {
            gitHubClient = CreateClient(
                new Credentials(gitHubJwtFactory.CreateEncodedJwtToken(), AuthenticationType.Bearer),
                this.gitHubAppSettings,
                appTelemetry);
            var (token, expirationTime) =
                await CreateInstallationTokenByInstallationId(installationId, gitHubAppSettings);

            gitHubClient.Credentials = new Credentials(token, AuthenticationType.Bearer);
            return new GitHubClientAdapter(gitHubClient);
        }

        private static GitHubClient CreateClient(
            Credentials credentials,
            GitHubAppSettings gitHubAppSettings,
            IAppTelemetry appTelemetry)
        {
            try
            {
                return new GitHubClient(
                    new Connection(
                        new ProductHeaderValue(gitHubAppSettings.Name),
                        new Uri(gitHubAppSettings.EnterpriseApiRoot),
                        new InMemoryCredentialStore(credentials),
                        new HttpClientAdapter(() => new GitHubClientMessageHandler(appTelemetry)),
                        new SimpleJsonSerializer()));
            }
            catch (Exception ex)
            {
                throw new CreateGitHubClientException(
                    $"Failed to create client for GitHubApp: {gitHubAppSettings.Name}", ex);
            }
        }

        private async Task<(string, DateTimeOffset)> CreateInstallationTokenByInstallationId(
            long installationId,
            GitHubAppSettings gitHubAppSettings)
        {
            try
            {
                var installationToken = await gitHubClient.GitHubApps.CreateInstallationToken(installationId);
                return (installationToken.Token, installationToken.ExpiresAt);
            }
            catch (NotFoundException ex)
            {
                throw new CreateGitHubClientException(
                    $"GitHub installation ({installationId}) not found or insufficient access privileges.", ex);
            }
            catch (Exception ex)
            {
                throw new CreateGitHubClientException(
                    $"Failed to get access token for installation: {installationId}, GitHubApp: {gitHubAppSettings.Name}",
                    ex);
            }
        }
    }
}