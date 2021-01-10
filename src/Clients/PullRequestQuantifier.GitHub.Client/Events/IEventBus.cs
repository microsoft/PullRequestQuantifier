namespace PullRequestQuantifier.GitHub.Client.Events
***REMOVED***
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    public interface IEventBus : IDisposable
    ***REMOVED***
        Task WriteAsync(JObject payload);

        Task SubscribeAsync(
            Func<string, Task> messageHandler,
            Func<Exception, Task> errorHandler,
            CancellationToken cancellationToken);
***REMOVED***
***REMOVED***