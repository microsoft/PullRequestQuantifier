namespace PullRequestQuantifier.GitHub.Client.Tests.TestServer
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using PullRequestQuantifier.GitHub.Client.Events;

    public class InMemoryEventBus : IEventBus
    {
        public List<JObject> Events { get; private set; } = new List<JObject>();

        public void Dispose()
        {
            Events = null;
        }

        public Task WriteAsync(JObject payload)
        {
            Events.Add(payload);
            return Task.CompletedTask;
        }

        public Task SubscribeAsync(
            Func<string, Task> messageHandler,
            Func<Exception, Task> errorHandler,
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
