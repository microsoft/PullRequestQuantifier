namespace PullRequestQuantifier.GitHub.Common.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Moq;
    using PullRequestQuantifier.Common.Azure.ServiceBus;
    using PullRequestQuantifier.Common.Azure.Telemetry;
    using PullRequestQuantifier.GitHub.Common.Events;
    using Xunit;

    public sealed class GitHubEventHostTests
    {
        [Fact]
        public async Task StartAsyncTest()
        {
            // setup
            var mockEventBus = new Mock<IEventBus>();
            mockEventBus.Setup(f =>
                f.SubscribeAsync(
                    It.IsAny<Func<string, DateTimeOffset, Task>>(),
                    It.IsAny<Func<Exception, Task>>(),
                    CancellationToken.None)).Returns(Task.CompletedTask);

            var gitHubEventHost = new GitHubEventHost(
                mockEventBus.Object,
                new Mock<IAppTelemetry>().Object,
                new List<IGitHubEventHandler>(),
                new Mock<ILogger<GitHubEventHost>>().Object);

            // act
            await gitHubEventHost.StartAsync(CancellationToken.None);

            // assert
            mockEventBus.Verify(
                f => f.SubscribeAsync(
                It.IsAny<Func<string, DateTimeOffset, Task>>(),
                It.IsAny<Func<Exception, Task>>(),
                CancellationToken.None),
                Times.Once);
        }

        [Fact]
        public async Task StopAsyncTest()
        {
            // setup
            var mockEventBus = new Mock<IEventBus>();
            mockEventBus.Setup(f =>
                f.SubscribeAsync(
                    It.IsAny<Func<string, DateTimeOffset, Task>>(),
                    It.IsAny<Func<Exception, Task>>(),
                    CancellationToken.None)).Returns(Task.CompletedTask);

            var gitHubEventHost = new GitHubEventHost(
                mockEventBus.Object,
                new Mock<IAppTelemetry>().Object,
                new List<IGitHubEventHandler>(),
                new Mock<ILogger<GitHubEventHost>>().Object);

            // act
            await gitHubEventHost.StopAsync(CancellationToken.None);

            // assert
            mockEventBus.Verify(
                f => f.SubscribeAsync(
                    It.IsAny<Func<string, DateTimeOffset, Task>>(),
                    It.IsAny<Func<Exception, Task>>(),
                    CancellationToken.None),
                Times.Never);
        }
    }
}
