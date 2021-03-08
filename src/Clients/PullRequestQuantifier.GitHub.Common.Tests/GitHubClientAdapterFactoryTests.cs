namespace PullRequestQuantifier.GitHub.Common.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using GitHubJwt;
    using Microsoft.Extensions.Options;
    using Moq;
    using PullRequestQuantifier.Common.Azure.Telemetry;
    using PullRequestQuantifier.GitHub.Common.GitHubClient;
    using PullRequestQuantifier.GitHub.Common.GitHubClient.Exceptions;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public sealed class GitHubClientAdapterFactoryTests
    {
        private readonly Mock<IGitHubJwtFactory> mockGitHubJwtFactory;
        private readonly IOptions<GitHubAppFlavorSettings> gitHubAppFlavorSettings;
        private readonly IMock<IAppTelemetry> mockAppTelemetry;

        public GitHubClientAdapterFactoryTests()
        {
            mockGitHubJwtFactory = new Mock<IGitHubJwtFactory>();
            mockAppTelemetry = new Mock<IAppTelemetry>();
            gitHubAppFlavorSettings = Options.Create(new GitHubAppFlavorSettings());
            gitHubAppFlavorSettings.Value.GitHubAppsSettings = new Dictionary<string, GitHubAppSettings>
            {
                {
                    "test",
                    new GitHubAppSettings
                    {
                        Name = nameof(GitHubClientAdapterFactoryTests),
                        EnterpriseApiRoot = "https://test.com"
                    }
                }
            };

            mockGitHubJwtFactory.Setup(f => f.CreateEncodedJwtToken()).Returns(Guid.NewGuid().ToString);
        }

        [Fact]
        public void GitHubClientAdapterFactory_Success()
        {
            // setup
            IGitHubClientAdapterFactory gitHubClientAdapterFactory = new GitHubClientAdapterFactory(
                new Dictionary<string, IGitHubJwtFactory> { { "test", mockGitHubJwtFactory.Object } },
                gitHubAppFlavorSettings,
                mockAppTelemetry.Object);

            // assert we get to this point
            Assert.True(true);
        }

        [Fact]
        public async Task GetGitHubClientAdapterAsync_CreateGitHubClientException()
        {
            // setup
            IGitHubClientAdapterFactory gitHubClientAdapterFactory = new GitHubClientAdapterFactory(
                new Dictionary<string, IGitHubJwtFactory> { { "test", mockGitHubJwtFactory.Object } },
                gitHubAppFlavorSettings,
                mockAppTelemetry.Object);

            // assert
            await Assert.ThrowsAsync<CreateGitHubClientException>(() => gitHubClientAdapterFactory.GetGitHubClientAdapterAsync(0, "test"));
        }
    }
}
