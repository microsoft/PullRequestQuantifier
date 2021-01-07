using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using PullRequestQuantifier.GitHub.Client;

[assembly: FunctionsStartup(typeof(Startup))]

namespace PullRequestQuantifier.GitHub.Client
{
    using GitHubJwt;
    using Microsoft.ApplicationInsights;
    using Microsoft.Azure.Functions.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using PullRequestQuantifier.GitHub.Client.GitHubClient;
    using PullRequestQuantifier.GitHub.Client.Telemetry;

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<GitHubAppSettings>()
                .Configure<IConfiguration>((gitHubAppSettings, configuration) =>
                {
                    configuration.GetSection(nameof(GitHubAppSettings)).Bind(gitHubAppSettings);
                });
            builder.Services.AddSingleton<IGitHubJwtFactory>(sp =>
            {
                // register a GitHubJwtFactory used to create tokens to access github for a particular org on behalf  of the  app
                var gitHubAppSettings = sp.GetRequiredService<GitHubAppSettings>();
                ArgumentCheck.ParameterIsNotNull(gitHubAppSettings, nameof(gitHubAppSettings));

                // Use GitHubJwt library to create the GitHubApp Jwt Token using our private certificate PEM file
                return new GitHubJwtFactory(
                    new StringPrivateKeySource(gitHubAppSettings.GitHubAppPrivateKeySecretName),
                    new GitHubJwtFactoryOptions
                    {
                        AppIntegrationId = gitHubAppSettings.AppIntegrationId, // The GitHub App Id
                        ExpirationSeconds = 600 // 10 minutes is the maximum time allowed
                    });
            });
            builder.Services.AddSingleton<IAppTelemetry>(sp =>
            {
                // use the default TelemetryClient provided by Azure Function
                var telemetryClient = sp.GetRequiredService<TelemetryClient>();
                return new AppTelemetry(typeof(Startup).Namespace, telemetryClient);
            });
        }
    }
}
