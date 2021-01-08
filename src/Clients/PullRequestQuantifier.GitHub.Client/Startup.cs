using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using PullRequestQuantifier.GitHub.Client;

[assembly: FunctionsStartup(typeof(Startup))]

namespace PullRequestQuantifier.GitHub.Client
***REMOVED***
    using System.IO;
    using GitHubJwt;
    using Microsoft.ApplicationInsights;
    using Microsoft.Azure.Functions.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using PullRequestQuantifier.GitHub.Client.GitHubClient;
    using PullRequestQuantifier.GitHub.Client.Telemetry;

    public class Startup : FunctionsStartup
    ***REMOVED***
        public override void Configure(IFunctionsHostBuilder builder)
        ***REMOVED***
            builder.Services.AddOptions<GitHubAppSettings>()
                .Configure<IConfiguration>((gitHubAppSettings, configuration) =>
                ***REMOVED***
                    configuration.GetSection(nameof(GitHubAppSettings)).Bind(gitHubAppSettings);
        ***REMOVED***);
            builder.Services.AddSingleton<IGitHubJwtFactory>(sp =>
            ***REMOVED***
                // register a GitHubJwtFactory used to create tokens to access github for a particular org on behalf  of the  app
                var gitHubAppSettings = sp.GetRequiredService<IOptions<GitHubAppSettings>>().Value;
                ArgumentCheck.ParameterIsNotNull(gitHubAppSettings, nameof(gitHubAppSettings));

                // Use GitHubJwt library to create the GitHubApp Jwt Token using our private certificate PEM file
                return new GitHubJwtFactory(
                    new StringPrivateKeySource(gitHubAppSettings.PrivateKey),
                    new GitHubJwtFactoryOptions
                    ***REMOVED***
                        AppIntegrationId = int.Parse(gitHubAppSettings.Id), // The GitHub App Id
                        ExpirationSeconds = 600 // 10 minutes is the maximum time allowed
            ***REMOVED***);
    ***REMOVED***);
            builder.Services.AddSingleton<IAppTelemetry>(sp =>
            ***REMOVED***
                // use the default TelemetryClient provided by Azure Function
                var telemetryClient = sp.GetRequiredService<TelemetryClient>();
                return new AppTelemetry(typeof(Startup).Namespace, telemetryClient);
    ***REMOVED***);
***REMOVED***
***REMOVED***
***REMOVED***
