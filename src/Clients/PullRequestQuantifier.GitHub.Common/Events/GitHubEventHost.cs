namespace PullRequestQuantifier.GitHub.Common.Events
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Linq;
    using PullRequestQuantifier.Common.Azure.ServiceBus;
    using PullRequestQuantifier.Common.Azure.Telemetry;
    using PullRequestQuantifier.GitHub.Common.Models;

    public class GitHubEventHost : IHostedService
    {
        private readonly IEventBus eventBus;
        private readonly IAppTelemetry telemetry;
        private readonly ILogger<GitHubEventHost> logger;
        private readonly IDictionary<GitHubEventActions, IGitHubEventHandler> gitHubEventHandlers;

        public GitHubEventHost(
            IEventBus eventBus,
            IAppTelemetry telemetry,
            IEnumerable<IGitHubEventHandler> gitHubEventHandlers,
            ILogger<GitHubEventHost> logger)
        {
            this.eventBus = eventBus;
            this.telemetry = telemetry;
            this.logger = logger;
            this.gitHubEventHandlers = gitHubEventHandlers.ToDictionary(h => h.EventType);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await eventBus.SubscribeAsync(
                HandleGitHubEvent,
                ErrorHandler,
                cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        [ExcludeFromCodeCoverage]
        private async Task HandleGitHubEvent(string gitHubEvent, DateTimeOffset messageEnqueueTime)
        {
            var eventJtoken = JToken.Parse(gitHubEvent);
            var eventType = eventJtoken["eventType"].ToString();
            if (!Enum.TryParse(
                eventType,
                true,
                out GitHubEventActions parsedEvent))
            {
                return;
            }

            if (gitHubEventHandlers.TryGetValue(parsedEvent, out var selectedEventHandler))
            {
                await selectedEventHandler.HandleEvent(gitHubEvent);
            }
            else
            {
                // this should never happen
                // there must always be a handler for an accepted eventType
                logger.LogError("Received unknown event in GitHubEventHost. {eventType}", eventType);
                telemetry.RecordMetric("GitHubEventHost-UnknownEvent", 1);
            }

            var messageProcessingCompleteDelay = DateTimeOffset.UtcNow - messageEnqueueTime;
            telemetry.RecordMetric(
                "EndMessageProcessing-Delay",
                (long)messageProcessingCompleteDelay.TotalSeconds,
                ("EventType", eventType));
        }

        [ExcludeFromCodeCoverage]
        private Task ErrorHandler(Exception exception)
        {
            logger.LogError(exception, "Error during event processing.");
            telemetry.RecordMetric("MessageProcessing-Error", 1);
            return Task.CompletedTask;
        }
    }
}
