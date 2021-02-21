namespace PullRequestQuantifier.GitHub.Common.GitHubClient
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GitHubJwt;
    using Microsoft.Extensions.Options;
    using Octokit;
    using Octokit.Internal;
    using PullRequestQuantifier.Common;
    using PullRequestQuantifier.Common.Azure.Telemetry;
    using PullRequestQuantifier.GitHub.Common.GitHubClient.Exceptions;

    /// <inheritdoc cref="IGitHubClientAdapterFactory"/>
    public sealed class GitHubClientAdapterFactory : IGitHubClientAdapterFactory
    {
        private readonly GitHubAppFlavorSettings gitHubAppFlavorSettings;
        private readonly IReadOnlyDictionary<string, IGitHubJwtFactory> gitHubJwtFactories;
        private readonly IAppTelemetry appTelemetry;

        public GitHubClientAdapterFactory(
            IReadOnlyDictionary<string, IGitHubJwtFactory> gitHubJwtFactories,
            IOptions<GitHubAppFlavorSettings> gitHubAppFlavorSettings,
            IAppTelemetry appTelemetry)
        {
            ArgumentCheck.ParameterIsNotNull(gitHubJwtFactories, nameof(gitHubJwtFactories));
            ArgumentCheck.ParameterIsNotNull(gitHubAppFlavorSettings, nameof(gitHubAppFlavorSettings));
            ArgumentCheck.ParameterIsNotNull(appTelemetry, nameof(appTelemetry));

            this.gitHubJwtFactories = gitHubJwtFactories;
            this.appTelemetry = appTelemetry;
            this.gitHubAppFlavorSettings = gitHubAppFlavorSettings.Value;
        }

        /// <inheritdoc />
        public async Task<IGitHubClientAdapter> GetGitHubClientAdapterAsync(
            long installationId,
            string dnsHost)
        {
            var gitHubClient = CreateClient(
                dnsHost,
                new Credentials(gitHubJwtFactories[dnsHost].CreateEncodedJwtToken(), AuthenticationType.Bearer));

            var (token, expirationTime) =
                await CreateInstallationTokenByInstallationId(
                    gitHubClient,
                    installationId,
                    dnsHost);

            gitHubClient.Credentials = new Credentials(token, AuthenticationType.Bearer);
            return new GitHubClientAdapter(gitHubClient, gitHubAppFlavorSettings[dnsHost]);
        }

        private GitHubClient CreateClient(
            string dnsHost,
            Credentials credentials)
        {
            try
            {
                return new GitHubClient(
                    new Connection(
                        new ProductHeaderValue(gitHubAppFlavorSettings[dnsHost].Name),
                        new Uri(gitHubAppFlavorSettings[dnsHost].EnterpriseApiRoot),
                        new InMemoryCredentialStore(credentials),
                        new HttpClientAdapter(() => new GitHubClientMessageHandler(appTelemetry)),
                        new SimpleJsonSerializer()));
            }
            catch (Exception ex)
            {
                throw new CreateGitHubClientException(
                    $"Failed to create client for GitHubApp: {gitHubAppFlavorSettings[dnsHost].Name}",
                    ex);
            }
        }

        private async Task<(string, DateTimeOffset)> CreateInstallationTokenByInstallationId(
            GitHubClient gitHubClient,
            long installationId,
            string dnsHost)
        {
            try
            {
                var installationToken = await gitHubClient.GitHubApps.CreateInstallationToken(installationId);
                return (installationToken.Token, installationToken.ExpiresAt);
            }
            catch (NotFoundException ex)
            {
                throw new CreateGitHubClientException(
                    $"GitHub installation ({installationId}) not found or insufficient access privileges.",
                    ex);
            }
            catch (Exception ex)
            {
                throw new CreateGitHubClientException(
                    $"Failed to get access token for installation: {installationId}, GitHubApp: {gitHubAppFlavorSettings[dnsHost].Name}",
                    ex);
            }
        }
    }
}
