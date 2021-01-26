namespace PullRequestQuantifier.GitHub.Client.Controllers
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json.Linq;
    using PullRequestQuantifier.GitHub.Client.Events;
    using PullRequestQuantifier.GitHub.Client.GitHubClient;
    using PullRequestQuantifier.GitHub.Client.Models;
    using PullRequestQuantifier.GitHub.Client.Telemetry;

    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class GitHubWebhookController : ControllerBase
    {
        private readonly GitHubAppSettings gitHubAppSettings;

        private readonly IAppTelemetry appTelemetry;

        private readonly IEventBus eventBus;

        public GitHubWebhookController(
            IOptions<GitHubAppSettings> gitHubAppSettings,
            IAppTelemetry appTelemetry,
            IEventBus eventBus)
        {
            this.appTelemetry = appTelemetry;
            this.eventBus = eventBus;
            this.gitHubAppSettings = gitHubAppSettings.Value;
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ProcessWebhook(
            [FromHeader(Name = "X-GitHub-Event")] string eventType,
            [FromHeader(Name = "X-Github-Delivery")] string deliveryId,
            [FromHeader(Name = "X-Hub-Signature")] string signature)
        {
            string content;
            using (var reader = new StreamReader(Request.Body))
            {
                content = await reader.ReadToEndAsync();
            }

            JObject payload = JObject.Parse(content);

            var action = (string)payload["action"];
            var dims = new[]
            {
                ("eventType", eventType),
                ("action", action),
                ("deliveryId", deliveryId)
            };

            if (!Authenticate(signature, content))
            {
                appTelemetry.RecordMetric(
                    "GitHubWebhook-AuthFailure",
                    1,
                    dims);
                throw new UnauthorizedAccessException("The signature couldn't be authenticated.");
            }

            if (Enum.TryParse(eventType, true, out AcceptedGitHubEventTypes _) &&
                Enum.TryParse(action, true, out AcceptedGitHubActionTypes _))
            {
                await eventBus.WriteAsync(payload);
            }

            return Ok();
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

#pragma warning disable CA5350 // GitHub sends webhook encoded with sha1
            using var hmacSha1 = new HMACSHA1(secretBytes);
#pragma warning restore CA5350 // GitHub sends webhook encoded with sha1
            var contentHash = hmacSha1.ComputeHash(contentBytes);

            return BitConverter.ToString(contentHash).Replace("-", string.Empty).ToLower();
        }
    }
}
