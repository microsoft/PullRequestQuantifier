namespace PullRequestQuantifier.GitHub.Client.GitHubClient
{
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using PullRequestQuantifier.GitHub.Client.Telemetry;

    public class GitHubClientMessageHandler : HttpClientHandler
    {
        private readonly IAppTelemetry metrics;

        public GitHubClientMessageHandler(IAppTelemetry metrics)
        {
            AllowAutoRedirect = false;
            this.metrics = metrics;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.MovedPermanently)
            {
                request.RequestUri = response.Headers.Location;
                response = await base.SendAsync(request, cancellationToken);
            }

            PublishRateLimitMetrics(request, response);

            return response;
        }

        // todo make this async when we go with the IAppTelemetry async.
        private void PublishRateLimitMetrics(HttpRequestMessage request, HttpResponseMessage response)
        {
            request.Headers.TryGetValues("User-Agent", out var userAgent);
            response.Headers.TryGetValues("X-RateLimit-Limit", out var totalRateLimitList);
            response.Headers.TryGetValues("X-RateLimit-Remaining", out var rateLimitRemainingList);

            var totalRateLimit = totalRateLimitList?.FirstOrDefault();
            var remainingRateLimit = rateLimitRemainingList?.FirstOrDefault();
            if (string.IsNullOrEmpty(totalRateLimit))
            {
                return;
            }

            var dims = new[] { ("User-Agent", userAgent?.FirstOrDefault()), ("X-RateLimit-Limit", totalRateLimit) };
            metrics.RecordMetric("GitHub-RateLimit-Remaining", long.Parse(remainingRateLimit), dims);
        }
    }
}
