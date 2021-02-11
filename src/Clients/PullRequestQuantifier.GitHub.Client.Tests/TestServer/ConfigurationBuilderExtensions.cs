namespace PullRequestQuantifier.GitHub.Client.Tests.TestServer
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Extensions.Configuration;

    [ExcludeFromCodeCoverage]
    public static class ConfigurationBuilderExtensions
    {
        public static ConfigurationBuilder AddInMemoryObject(
            this ConfigurationBuilder configurationBuilder,
            object settings,
            string settingsRoot)
        {
            configurationBuilder.AddInMemoryCollection(settings.ToKeyValuePairs(settingsRoot));
            return configurationBuilder;
        }
    }
}
