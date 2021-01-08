namespace PullRequestQuantifier.GitHub.Client
***REMOVED***
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
    ***REMOVED***
        private readonly IGitHubJwtFactory gitHubJwtFactory;

        private readonly GitHubAppSettings gitHubAppSettings;

        private readonly IAppTelemetry appTelemetry;

        public QuantifierFunction(
            IGitHubJwtFactory gitHubJwtFactory,
            IOptions<GitHubAppSettings> gitHubAppSettings,
            IAppTelemetry appTelemetry)
        ***REMOVED***
            this.gitHubJwtFactory = gitHubJwtFactory;
            this.appTelemetry = appTelemetry;
            this.gitHubAppSettings = gitHubAppSettings.Value;
***REMOVED***

        [FunctionName("quantify")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        ***REMOVED***
            req.Headers.TryGetValue("X-GitHub-Event", out var eventType);
            req.Headers.TryGetValue("X-GitHub-Delivery", out var deliveryId);
            req.Headers.TryGetValue("X-Hub-Signature", out var signature);

            string content;
            using (var reader = new StreamReader(req.Body))
            ***REMOVED***
                content = await reader.ReadToEndAsync();
    ***REMOVED***

            JObject payload = JObject.Parse(content);

            var action = (string)payload["action"];
            var dims = new[]
            ***REMOVED***
                ("eventType", eventType.ToString()),
                ("action", action),
                ("deliveryId", deliveryId.ToString())
    ***REMOVED***;

            if (!Authenticate(signature, content))
            ***REMOVED***
                appTelemetry.RecordMetric(
                    "GitHubWebhook-AuthFailure",
                    1,
                    dims);
                throw new UnauthorizedAccessException("The signature couldn't be authenticated.");
    ***REMOVED***

            var gitHubClientFactory = GitHubClientFactory.Create(
                "microsoft",
                new Credentials(gitHubJwtFactory.CreateEncodedJwtToken(), AuthenticationType.Bearer),
                gitHubAppSettings,
                appTelemetry);
            await gitHubClientFactory.GetGitHubGitClientAsync();
            return new OkObjectResult("Response from function with injected dependencies.");
***REMOVED***

        private bool Authenticate(string signature, string content)
        ***REMOVED***
            var hashValue = ComputeHash(content);

            if (("sha1=" + hashValue).Equals(signature))
            ***REMOVED***
                return true;
    ***REMOVED***

            return false;
***REMOVED***

        private string ComputeHash(string content)
        ***REMOVED***
            var secretBytes = Encoding.UTF8.GetBytes(gitHubAppSettings.WebhookSecret);
            var contentBytes = Encoding.UTF8.GetBytes(content);

            using var hmacSha1 = new HMACSHA1(secretBytes);
            var contentHash = hmacSha1.ComputeHash(contentBytes);

            return BitConverter.ToString(contentHash).Replace("-", string.Empty).ToLower();
***REMOVED***
***REMOVED***
***REMOVED***
