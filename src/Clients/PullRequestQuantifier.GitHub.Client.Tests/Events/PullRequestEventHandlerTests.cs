namespace PullRequestQuantifier.GitHub.Client.Tests.Events
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Octokit;
    using PullRequestQuantifier.Common.Azure.Telemetry;
    using PullRequestQuantifier.GitHub.Client.Events;
    using PullRequestQuantifier.GitHub.Common.GitHubClient;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public sealed class PullRequestEventHandlerTests
    {
        [Fact]
        public async Task HandleEventTest()
        {
            // setup
            var appTelemetry = new Mock<IAppTelemetry>();
            var gitHubClientAdapter = new Mock<IGitHubClientAdapter>();
            appTelemetry.Setup(f => f.RecordMetric(It.IsAny<string>(), It.IsAny<long>()));
            var gitHubClientAdapterFactory = new Mock<IGitHubClientAdapterFactory>();
            gitHubClientAdapterFactory.Setup(f => f.GetGitHubClientAdapterAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(gitHubClientAdapter.Object);
            gitHubClientAdapter.Setup(f => f.GetPullRequestFilesAsync(It.IsAny<long>(), It.IsAny<int>()))
                .ReturnsAsync(new List<PullRequestFile>());
            gitHubClientAdapter.Setup(f => f.GetIssueLabelsAsync(It.IsAny<long>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Label>());
            gitHubClientAdapter.Setup(f => f.GetIssueCommentsAsync(It.IsAny<long>(), It.IsAny<int>()))
                .ReturnsAsync(new List<IssueComment>());

            var installationEventHandler = new PullRequestEventHandler(
                gitHubClientAdapterFactory.Object,
                appTelemetry.Object,
                new Mock<ILogger<PullRequestEventHandler>>().Object);

            // act
            await installationEventHandler.HandleEvent(await File.ReadAllTextAsync(@"Data/PulRequestPayload.txt"));

            // assert
            appTelemetry.Verify(
                f =>
                    f.RecordMetric(It.IsAny<string>(), It.IsAny<long>()),
                Times.Once);
        }
    }
}
