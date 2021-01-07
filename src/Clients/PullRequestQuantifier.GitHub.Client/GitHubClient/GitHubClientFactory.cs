namespace PullRequestQuantifier.GitHub.Client.GitHubClient
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Octokit;
    using Octokit.Internal;
    using PullRequestQuantifier.GitHub.Client.GitHubClient.Exceptions;
    using PullRequestQuantifier.GitHub.Client.Telemetry;

    /// <inheritdoc cref="IGitHubClientFactory"/>
    public sealed class GitHubClientFactory : IGitHubClientFactory
    {
        // will keep the gitHubClient and the token expiration datetime, based on which will have to renew the connection,
        // for now don't do any implementation for this
        private readonly Lazy<Task<(IGitHubClient, DateTimeOffset)>> lazyGitHubClient;

        private GitHubClientFactory(Func<Task<(IGitHubClient, DateTimeOffset)>> getGitHubClientAsync)
        {
            lazyGitHubClient = new Lazy<Task<(IGitHubClient, DateTimeOffset)>>(
                getGitHubClientAsync,
                LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public static IGitHubClientFactory Create(
            Credentials credentials,
            GitHubAppSettings gitHubAppSettings,
            IAppTelemetry appTelemetry)
        {
            ArgumentCheck.ParameterIsNotNull(appTelemetry, nameof(appTelemetry));
            ArgumentCheck.ParameterIsNotNull(gitHubAppSettings, nameof(gitHubAppSettings));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(gitHubAppSettings.GitHubAppName, nameof(gitHubAppSettings.GitHubAppName));

            return new GitHubClientFactory(
                () => CreateAuthenticatedClient(credentials, gitHubAppSettings, appTelemetry));
        }

        public static IGitHubClientFactory Create(
            string orgName,
            Credentials credentials,
            GitHubAppSettings gitHubAppSettings,
            IAppTelemetry appTelemetry)
        {
            ArgumentCheck.ParameterIsNotNull(appTelemetry, nameof(appTelemetry));
            ArgumentCheck.ParameterIsNotNull(gitHubAppSettings, nameof(gitHubAppSettings));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(gitHubAppSettings.GitHubAppName, nameof(gitHubAppSettings.GitHubAppName));

            return new GitHubClientFactory(
                () => CreateAuthenticatedClient(orgName, credentials, gitHubAppSettings, appTelemetry));
        }

        /// <inheritdoc />
        public async Task<IGitHubClientAdapter> GetGitHubGitClientAsync()
        {
            var client = await lazyGitHubClient.Value;
            return new GitHubClientAdapter(client.Item1);
        }

        private static async Task<(IGitHubClient, DateTimeOffset)> CreateAuthenticatedClient(
            Credentials credentials,
            GitHubAppSettings gitHubAppSettings,
            IAppTelemetry appTelemetry)
        {
            GitHubClient gitHubClient = null;

            await Task.Run(() =>
            {
                gitHubClient = CreateClient(credentials, gitHubAppSettings, appTelemetry);
            }).ConfigureAwait(false);

            return (gitHubClient, DateTimeOffset.MaxValue);
        }

        private static async Task<(IGitHubClient, DateTimeOffset)> CreateAuthenticatedClient(
            string orgName,
            Credentials credentials,
            GitHubAppSettings gitHubAppSettings,
            IAppTelemetry appTelemetry)
        {
            var gitHubClient = CreateClient(
                credentials,
                gitHubAppSettings,
                appTelemetry);

            var (token, expirationTime) = await (
                string.IsNullOrEmpty(orgName) ?
                CreateAppToken(gitHubAppSettings) : CreateInstallationToken(orgName, gitHubAppSettings, gitHubClient));

            gitHubClient.Credentials = new Credentials(token, AuthenticationType.Bearer);
            return (gitHubClient, expirationTime);
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
                        new ProductHeaderValue(gitHubAppSettings.GitHubAppName),
                        new Uri(gitHubAppSettings.EnterpriseApiRoot),
                        new InMemoryCredentialStore(credentials),
                        new HttpClientAdapter(() => new GitHubClientMessageHandler(appTelemetry)),
                        new SimpleJsonSerializer()));
            }
            catch (Exception ex)
            {
                throw new CreateGitHubClientException(
                    $"Failed to create client for GitHubApp: {gitHubAppSettings.GitHubAppName}", ex);
            }
        }

        private static async Task<(string, DateTimeOffset)> CreateAppToken(GitHubAppSettings gitHubAppSettings)
        {
            try
            {
                // todo add implementation, await Task.Delay(TimeSpan.Zero) to avoid the stylecop warning
                await Task.Delay(TimeSpan.Zero);
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                throw new CreateGitHubClientException(
                    $"Failed to get create token for GitHubApp: {gitHubAppSettings.GitHubAppName}", ex);
            }
        }

        private static async Task<(string, DateTimeOffset)> CreateInstallationToken(
            string orgName,
            GitHubAppSettings gitHubAppSettings,
            GitHubClient gitHubClient)
        {
            try
            {
                Installation installation = await gitHubClient.GitHubApps.GetOrganizationInstallationForCurrent(orgName);
                var installationToken = await gitHubClient.GitHubApps.CreateInstallationToken(installation.Id);
                return (installationToken.Token, installationToken.ExpiresAt);
            }
            catch (NotFoundException ex)
            {
                throw new CreateGitHubClientException(
                    $"GitHub Org ({orgName}) not found or insufficient access privileges.", ex);
            }
            catch (Exception ex)
            {
                throw new CreateGitHubClientException(
                    $"Failed to get access token for Org: {orgName}, GitHubApp: {gitHubAppSettings.GitHubAppName}", ex);
            }
        }
    }
}
