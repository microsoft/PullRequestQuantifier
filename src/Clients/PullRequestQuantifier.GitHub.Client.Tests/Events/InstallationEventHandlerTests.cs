namespace PullRequestQuantifier.GitHub.Client.Tests.Events
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Moq;
    using PullRequestQuantifier.Common.Azure.Telemetry;
    using PullRequestQuantifier.GitHub.Client.Events;
    using PullRequestQuantifier.GitHub.Common.GitHubClient;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public sealed class InstallationEventHandlerTests
    {
        [Fact]
        public async Task HandleEventTest()
        {
            // setup
            var appTelemetry = new Mock<IAppTelemetry>();
            appTelemetry.Setup(f => f.RecordMetric(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<(string, string)[]>()));

            var installationEventHandler = new InstallationEventHandler(
                new Mock<IGitHubClientAdapterFactory>().Object,
                appTelemetry.Object,
                new Mock<ILogger<PullRequestEventHandler>>().Object);

            // act
            await installationEventHandler.HandleEvent(await File.ReadAllTextAsync(@"Data/InstallationPayload.txt"));

            // assert
            appTelemetry.Verify(
                f =>
                f.RecordMetric(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<(string, string)[]>()),
                Times.Once);
        }
    }
}
