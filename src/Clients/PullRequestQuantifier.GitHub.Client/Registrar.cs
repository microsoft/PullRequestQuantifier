namespace PullRequestQuantifier.GitHub.Client
{
    using GitHubJwt;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using PullRequestQuantifier.GitHub.Client.Events;
    using PullRequestQuantifier.GitHub.Client.GitHubClient;
    using PullRequestQuantifier.GitHub.Client.Telemetry;

    public static class Registrar
    {
        internal static IServiceCollection RegisterServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<GitHubAppSettings>(configuration.GetSection(nameof(GitHubAppSettings)));
            serviceCollection.AddSingleton<IGitHubJwtFactory>(sp =>
            {
                // register a GitHubJwtFactory used to create tokens to access github for a particular org on behalf  of the  app
                var gitHubAppSettings = sp.GetRequiredService<IOptions<GitHubAppSettings>>().Value;
                ArgumentCheck.ParameterIsNotNull(gitHubAppSettings, nameof(gitHubAppSettings));

                // Use GitHubJwt library to create the GitHubApp Jwt Token using our private certificate PEM file
                return new GitHubJwtFactory(
                    new StringPrivateKeySource(gitHubAppSettings.PrivateKey),
                    new GitHubJwtFactoryOptions
                    {
                        AppIntegrationId = int.Parse(gitHubAppSettings.Id), // The GitHub App Id
                        ExpirationSeconds = 600 // 10 minutes is the maximum time allowed
                    });
            });
            serviceCollection.AddApmForWebHost(configuration, typeof(Registrar).Namespace);
            serviceCollection.AddSingleton<IEventBus, InMemoryEventBus>();
            return serviceCollection;
        }
    }
}
