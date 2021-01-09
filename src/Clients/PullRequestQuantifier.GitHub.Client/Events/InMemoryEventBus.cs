namespace PullRequestQuantifier.GitHub.Client.Events
***REMOVED***
    using System.Collections.Concurrent;
    using Newtonsoft.Json.Linq;

    public sealed class InMemoryEventBus : IEventBus
    ***REMOVED***
        private readonly ConcurrentQueue<JObject> payloadsStorage =
            new ConcurrentQueue<JObject>();

        public void Write(JObject payload)
        ***REMOVED***
            payloadsStorage.Enqueue(payload);
***REMOVED***

        public JObject ReadNext()
        ***REMOVED***
            return payloadsStorage.TryDequeue(out var ret) ? ret : null;
***REMOVED***
***REMOVED***
***REMOVED***
