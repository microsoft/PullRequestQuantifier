namespace PullRequestQuantifier.GitHub.Client.Events
{
    using System.Collections.Concurrent;
    using Newtonsoft.Json.Linq;

    public sealed class InMemoryEventBus : IEventBus
    {
        private readonly ConcurrentQueue<JObject> payloadsStorage =
            new ConcurrentQueue<JObject>();

        public void Write(JObject payload)
        {
            payloadsStorage.Enqueue(payload);
        }

        public JObject ReadNext()
        {
            return payloadsStorage.TryDequeue(out var ret) ? ret : null;
        }
    }
}
