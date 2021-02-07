namespace PullRequestQuantifier.GitHub.Client.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Newtonsoft.Json.Linq;
    using PullRequestQuantifier.GitHub.Client.Models;
    using PullRequestQuantifier.GitHub.Client.Telemetry;

    public class GitHubEventHost : IHostedService
    {
        private readonly IEventBus eventBus;
        private readonly IAppTelemetry appTelemetry;
        private readonly IDictionary<GitHubEventActions, IGitHubEventHandler> gitHubEventHandlers;

        public GitHubEventHost(
            IEventBus eventBus,
            IAppTelemetry appTelemetry,
            IEnumerable<IGitHubEventHandler> gitHubEventHandlers)
        {
            this.eventBus = eventBus;
            this.appTelemetry = appTelemetry;
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

        private async Task HandleGitHubEvent(string gitHubEvent)
        {
            var eventJtoken = JToken.Parse(gitHubEvent);
            if (!Enum.TryParse(
                eventJtoken["eventType"].ToString(),
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
                // TODO: log
            }
        }

        private Task ErrorHandler(Exception exception)
        {
            return Task.CompletedTask;
        }
    }
}
