namespace PullRequestQuantifier.GitHub.Client.GitHubClient
***REMOVED***
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Octokit;
    using Octokit.Internal;
    using PullRequestQuantifier.GitHub.Client.GitHubClient.Exceptions;
    using PullRequestQuantifier.GitHub.Client.Telemetry;

    /// <inheritdoc cref="IGitHubClientFactory"/>
    public sealed class GitHubClientFactory : IGitHubClientFactory
    ***REMOVED***
        // will keep the gitHubClient and the token expiration datetime, based on which will have to renew the connection,
        // for now don't do any implementation for this
        private readonly Lazy<Task<(IGitHubClient, DateTimeOffset)>> lazyGitHubClient;

        private GitHubClientFactory(Func<Task<(IGitHubClient, DateTimeOffset)>> getGitHubClientAsync)
        ***REMOVED***
            lazyGitHubClient = new Lazy<Task<(IGitHubClient, DateTimeOffset)>>(
                getGitHubClientAsync,
                LazyThreadSafetyMode.ExecutionAndPublication);
***REMOVED***

        public static IGitHubClientFactory Create(
            Credentials credentials,
            GitHubAppSettings gitHubAppSettings,
            IAppTelemetry appTelemetry)
        ***REMOVED***
            ArgumentCheck.ParameterIsNotNull(appTelemetry, nameof(appTelemetry));
            ArgumentCheck.ParameterIsNotNull(gitHubAppSettings, nameof(gitHubAppSettings));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(gitHubAppSettings.GitHubAppName, nameof(gitHubAppSettings.GitHubAppName));

            return new GitHubClientFactory(
                () => CreateAuthenticatedClient(credentials, gitHubAppSettings, appTelemetry));
***REMOVED***

        public static IGitHubClientFactory Create(
            string orgName,
            Credentials credentials,
            GitHubAppSettings gitHubAppSettings,
            IAppTelemetry appTelemetry)
        ***REMOVED***
            ArgumentCheck.ParameterIsNotNull(appTelemetry, nameof(appTelemetry));
            ArgumentCheck.ParameterIsNotNull(gitHubAppSettings, nameof(gitHubAppSettings));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(gitHubAppSettings.GitHubAppName, nameof(gitHubAppSettings.GitHubAppName));

            return new GitHubClientFactory(
                () => CreateAuthenticatedClient(orgName, credentials, gitHubAppSettings, appTelemetry));
***REMOVED***

        /// <inheritdoc />
        public async Task<IGitHubClientAdapter> GetGitHubGitClientAsync()
        ***REMOVED***
            var client = await lazyGitHubClient.Value;
            return new GitHubClientAdapter(client.Item1);
***REMOVED***

        private static async Task<(IGitHubClient, DateTimeOffset)> CreateAuthenticatedClient(
            Credentials credentials,
            GitHubAppSettings gitHubAppSettings,
            IAppTelemetry appTelemetry)
        ***REMOVED***
            GitHubClient gitHubClient = null;

            await Task.Run(() =>
            ***REMOVED***
                gitHubClient = CreateClient(credentials, gitHubAppSettings, appTelemetry);
    ***REMOVED***).ConfigureAwait(false);

            return (gitHubClient, DateTimeOffset.MaxValue);
***REMOVED***

        private static async Task<(IGitHubClient, DateTimeOffset)> CreateAuthenticatedClient(
            string orgName,
            Credentials credentials,
            GitHubAppSettings gitHubAppSettings,
            IAppTelemetry appTelemetry)
        ***REMOVED***
            var gitHubClient = CreateClient(
                credentials,
                gitHubAppSettings,
                appTelemetry);

            var (token, expirationTime) = await (
                string.IsNullOrEmpty(orgName) ?
                CreateAppToken(gitHubAppSettings) : CreateInstallationToken(orgName, gitHubAppSettings, gitHubClient));

            gitHubClient.Credentials = new Credentials(token, AuthenticationType.Bearer);
            return (gitHubClient, expirationTime);
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
                        new ProductHeaderValue(gitHubAppSettings.GitHubAppName),
                        new Uri(gitHubAppSettings.EnterpriseApiRoot),
                        new InMemoryCredentialStore(credentials),
                        new HttpClientAdapter(() => new GitHubClientMessageHandler(appTelemetry)),
                        new SimpleJsonSerializer()));
    ***REMOVED***
            catch (Exception ex)
            ***REMOVED***
                throw new CreateGitHubClientException(
                    $"Failed to create client for GitHubApp: ***REMOVED***gitHubAppSettings.GitHubAppName***REMOVED***", ex);
    ***REMOVED***
***REMOVED***

        private static async Task<(string, DateTimeOffset)> CreateAppToken(GitHubAppSettings gitHubAppSettings)
        ***REMOVED***
            try
            ***REMOVED***
                // todo add implementation, await Task.Delay(TimeSpan.Zero) to avoid the stylecop warning
                await Task.Delay(TimeSpan.Zero);
                throw new NotImplementedException();
    ***REMOVED***
            catch (Exception ex)
            ***REMOVED***
                throw new CreateGitHubClientException(
                    $"Failed to get create token for GitHubApp: ***REMOVED***gitHubAppSettings.GitHubAppName***REMOVED***", ex);
    ***REMOVED***
***REMOVED***

        private static async Task<(string, DateTimeOffset)> CreateInstallationToken(
            string orgName,
            GitHubAppSettings gitHubAppSettings,
            GitHubClient gitHubClient)
        ***REMOVED***
            try
            ***REMOVED***
                Installation installation = await gitHubClient.GitHubApps.GetOrganizationInstallationForCurrent(orgName);
                var installationToken = await gitHubClient.GitHubApps.CreateInstallationToken(installation.Id);
                return (installationToken.Token, installationToken.ExpiresAt);
    ***REMOVED***
            catch (NotFoundException ex)
            ***REMOVED***
                throw new CreateGitHubClientException(
                    $"GitHub Org (***REMOVED***orgName***REMOVED***) not found or insufficient access privileges.", ex);
    ***REMOVED***
            catch (Exception ex)
            ***REMOVED***
                throw new CreateGitHubClientException(
                    $"Failed to get access token for Org: ***REMOVED***orgName***REMOVED***, GitHubApp: ***REMOVED***gitHubAppSettings.GitHubAppName***REMOVED***", ex);
    ***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
