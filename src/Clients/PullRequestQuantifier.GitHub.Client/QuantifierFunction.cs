namespace PullRequestQuantifier.GitHub.Client
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using GitHubJwt;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json.Linq;
    using Octokit;
    using PullRequestQuantifier.GitHub.Client.GitHubClient;
    using PullRequestQuantifier.GitHub.Client.Telemetry;

    public class QuantifierFunction
    {
        private readonly IGitHubJwtFactory gitHubJwtFactory;

        private readonly GitHubAppSettings gitHubAppSettings;

        private readonly IAppTelemetry appTelemetry;

        public QuantifierFunction(
            IGitHubJwtFactory gitHubJwtFactory,
            IOptions<GitHubAppSettings> gitHubAppSettings,
            IAppTelemetry appTelemetry)
        {
            this.gitHubJwtFactory = gitHubJwtFactory;
            this.appTelemetry = appTelemetry;
            this.gitHubAppSettings = gitHubAppSettings.Value;
        }

        [FunctionName("quantify")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            req.Headers.TryGetValue("X-GitHub-Event", out var eventType);
            req.Headers.TryGetValue("X-GitHub-Delivery", out var deliveryId);
            req.Headers.TryGetValue("X-Hub-Signature", out var signature);

            string content;
            using (var reader = new StreamReader(req.Body))
            {
                content = await reader.ReadToEndAsync();
            }

            JObject payload = JObject.Parse(content);

            var action = (string)payload["action"];
            var dims = new[]
            {
                ("eventType", eventType.ToString()),
                ("action", action),
                ("deliveryId", deliveryId.ToString())
            };

            if (!Authenticate(signature, content))
            {
                appTelemetry.RecordMetric(
                    "GitHubWebhook-AuthFailure",
                    1,
                    dims);
                throw new UnauthorizedAccessException("The signature couldn't be authenticated.");
            }

            var gitHubClientFactory = GitHubClientFactory.Create(
                "microsoft",
                new Credentials(gitHubJwtFactory.CreateEncodedJwtToken(), AuthenticationType.Bearer),
                gitHubAppSettings,
                appTelemetry);
            await gitHubClientFactory.GetGitHubGitClientAsync();
            return new OkObjectResult("Response from function with injected dependencies.");
        }

        private bool Authenticate(string signature, string content)
        {
            var hashValue = ComputeHash(content);

            if (("sha1=" + hashValue).Equals(signature))
            {
                return true;
            }

            return false;
        }

        private string ComputeHash(string content)
        {
            var secretBytes = Encoding.UTF8.GetBytes(gitHubAppSettings.WebhookSecret);
            var contentBytes = Encoding.UTF8.GetBytes(content);

            using var hmacSha1 = new HMACSHA1(secretBytes);
            var contentHash = hmacSha1.ComputeHash(contentBytes);

            return BitConverter.ToString(contentHash).Replace("-", string.Empty).ToLower();
        }
    }
}
