namespace PullRequestQuantifier.GitHub.Client.GitHubClient
***REMOVED***
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using PullRequestQuantifier.GitHub.Client.Telemetry;

    public class GitHubClientMessageHandler : HttpClientHandler
    ***REMOVED***
        private readonly IAppTelemetry metrics;

        public GitHubClientMessageHandler(IAppTelemetry metrics)
        ***REMOVED***
            AllowAutoRedirect = false;
            this.metrics = metrics;
***REMOVED***

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        ***REMOVED***
            var response = await base.SendAsync(request, cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.MovedPermanently)
            ***REMOVED***
                request.RequestUri = response.Headers.Location;
                response = await base.SendAsync(request, cancellationToken);
    ***REMOVED***

            PublishRateLimitMetrics(request, response);

            return response;
***REMOVED***

        // todo make this async when we go with the IAppTelemetry async.
        private void PublishRateLimitMetrics(HttpRequestMessage request, HttpResponseMessage response)
        ***REMOVED***
            request.Headers.TryGetValues("User-Agent", out var userAgent);
            response.Headers.TryGetValues("X-RateLimit-Limit", out var totalRateLimitList);
            response.Headers.TryGetValues("X-RateLimit-Remaining", out var rateLimitRemainingList);

            var totalRateLimit = totalRateLimitList?.FirstOrDefault();
            var remainingRateLimit = rateLimitRemainingList?.FirstOrDefault();
            if (string.IsNullOrEmpty(totalRateLimit))
            ***REMOVED***
                return;
    ***REMOVED***

            var dims = new[] ***REMOVED*** ("User-Agent", userAgent?.FirstOrDefault()), ("X-RateLimit-Limit", totalRateLimit) ***REMOVED***;
            metrics.RecordMetric("GitHub-RateLimit-Remaining", long.Parse(remainingRateLimit), dims);
***REMOVED***
***REMOVED***
***REMOVED***
