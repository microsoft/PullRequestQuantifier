namespace PullRequestQuantifier.GitHub.Client.Events
***REMOVED***
    using Newtonsoft.Json.Linq;

    public interface IEventBus
    ***REMOVED***
        void Write(JObject payload);

        JObject ReadNext();
***REMOVED***
***REMOVED***
