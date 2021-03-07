namespace PullRequestQuantifier.GitHub.Client.Tests.Events
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Moq;
    using PullRequestQuantifier.Common.Azure.Telemetry;
    using PullRequestQuantifier.GitHub.Client.Events;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public sealed class InstallationRepositoriesEventHandlerTests
    {
        [Fact]
        public async Task HandleEventTest()
        {
            // setup
            var appTelemetry = new Mock<IAppTelemetry>();
            appTelemetry.Setup(f => f.RecordMetric(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<(string, string)[]>()));

            var installationRepositoriesEventHandler = new InstallationRepositoriesEventHandler(
                appTelemetry.Object,
                new Mock<ILogger<PullRequestEventHandler>>().Object);

            // act
            await installationRepositoriesEventHandler.HandleEvent(await File.ReadAllTextAsync(@"Data/InstallationRepositories.txt"));

            // assert
            appTelemetry.Verify(
                f =>
                f.RecordMetric(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<(string, string)[]>()),
                Times.Once);
        }
    }
}
