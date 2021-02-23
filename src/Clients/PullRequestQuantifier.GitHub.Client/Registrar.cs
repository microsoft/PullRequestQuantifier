namespace PullRequestQuantifier.GitHub.Client
{
    using System.Collections.Generic;
    using GitHubJwt;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;
    using PullRequestQuantifier.Common;
    using PullRequestQuantifier.Common.Azure.ServiceBus;
    using PullRequestQuantifier.Common.Azure.Telemetry;
    using PullRequestQuantifier.GitHub.Client.Events;
    using PullRequestQuantifier.GitHub.Common.Events;
    using PullRequestQuantifier.GitHub.Common.GitHubClient;

    public static class Registrar
    {
        public static IServiceCollection RegisterServices(
            this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            serviceCollection.Configure<GitHubAppFlavorSettings>(configuration.GetSection(nameof(GitHubAppFlavorSettings)));
            serviceCollection.Configure<AzureServiceBusSettings>(
                configuration.GetSection(nameof(AzureServiceBusSettings)));
            serviceCollection.AddSingleton<IReadOnlyDictionary<string, IGitHubJwtFactory>>(
                sp =>
                {
                    // register a GitHubJwtFactory used to create tokens to access github for a particular org on behalf  of the  app
                    var gitHubAppFlavorSettings = sp.GetRequiredService<IOptions<GitHubAppFlavorSettings>>().Value;
                    ArgumentCheck.ParameterIsNotNull(gitHubAppFlavorSettings, nameof(gitHubAppFlavorSettings));

                    var ret = new Dictionary<string, IGitHubJwtFactory>();
                    foreach (var gitHubAppSettings in gitHubAppFlavorSettings.GitHubAppsSettings)
                    {
                        // Use GitHubJwt library to create the GitHubApp Jwt Token using our private certificate PEM file
                        ret[gitHubAppSettings.Key] = new GitHubJwtFactory(
                            new StringPrivateKeySource(gitHubAppSettings.Value.PrivateKey),
                            new GitHubJwtFactoryOptions
                            {
                                AppIntegrationId = int.Parse(gitHubAppSettings.Value.Id), // The GitHub App Id
                                ExpirationSeconds = 590 // 10 minutes is the maximum time allowed
                            });
                    }

                    return ret;
                });
            serviceCollection.AddSingleton<IGitHubClientAdapterFactory, GitHubClientAdapterFactory>();
            serviceCollection.TryAddSingleton<IEventBus, AzureServiceBus>();
            serviceCollection.TryAddEnumerable(
                new[]
                {
                    ServiceDescriptor.Singleton<IGitHubEventHandler, PullRequestEventHandler>(),
                    ServiceDescriptor.Singleton<IGitHubEventHandler, InstallationEventHandler>(),
                    ServiceDescriptor.Singleton<IGitHubEventHandler, InstallationRepositoriesEventHandler>()
                });
            serviceCollection.AddHostedService<GitHubEventHost>();

            serviceCollection.AddApmForWebHost(configuration, typeof(Registrar).Namespace);
            return serviceCollection;
        }
    }
}
