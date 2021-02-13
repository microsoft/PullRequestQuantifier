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
    using PullRequestQuantifier.Common;
    using PullRequestQuantifier.GitHub.Client.Events;
    using PullRequestQuantifier.GitHub.Client.GitHubClient;
    using PullRequestQuantifier.GitHub.Client.Models;
    using PullRequestQuantifier.GitHub.Client.Telemetry;

    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class GitHubWebhookController : ControllerBase
    {
        private const string PayloadUrlKeyName = "html_url";
        private readonly GitHubAppFlavorSettings gitHubAppFlavorSettings;
        private readonly IAppTelemetry appTelemetry;
        private readonly IEventBus eventBus;

        public GitHubWebhookController(
            IOptions<GitHubAppFlavorSettings> gitHubAppFlavorSettings,
            IAppTelemetry appTelemetry,
            IEventBus eventBus)
        {
            ArgumentCheck.ParameterIsNotNull(gitHubAppFlavorSettings, nameof(gitHubAppFlavorSettings));
            ArgumentCheck.ParameterIsNotNull(gitHubAppFlavorSettings, nameof(gitHubAppFlavorSettings));
            ArgumentCheck.ParameterIsNotNull(appTelemetry, nameof(appTelemetry));

            this.appTelemetry = appTelemetry;
            this.eventBus = eventBus;
            this.gitHubAppFlavorSettings = gitHubAppFlavorSettings.Value;
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ProcessWebhook(
            [FromHeader(Name = "X-GitHub-Event")] string eventType,
            [FromHeader(Name = "X-Github-Delivery")] string deliveryId,
            [FromHeader(Name = "X-Hub-Signature-256")] string signature)
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

            var senderHtmlUrl = payload["sender"]?[PayloadUrlKeyName]?.ToString();
            if (string.IsNullOrEmpty(senderHtmlUrl))
            {
                appTelemetry.RecordMetric(
                    "GitHubWebhook-PayloadUrlKeyName missing",
                    1,
                    dims);
                throw new UnauthorizedAccessException($"Payload url key name ({PayloadUrlKeyName}) is missing.");
            }

            if (!Authenticate(
                signature,
                content,
                new Uri(senderHtmlUrl).DnsSafeHost))
            {
                appTelemetry.RecordMetric(
                    "GitHubWebhook-AuthFailure",
                    1,
                    dims);
                throw new UnauthorizedAccessException("The signature couldn't be authenticated.");
            }

            if (Enum.TryParse(eventType, true, out GitHubEventActions parsedEvent) &&
                Enum.TryParse(action, true, out GitHubEventActions parsedAction) &&
                (parsedEvent & parsedAction) == parsedAction)
            {
                payload["eventType"] = eventType;
                await eventBus.WriteAsync(payload);
            }

            return Ok();
        }

        private bool Authenticate(
            string signature,
            string content,
            string dnsHost)
        {
            var hashValue = ComputeHash(content, dnsHost);
            return $"sha256={hashValue}".Equals(signature);
        }

        private string ComputeHash(
            string content,
            string dnsHost)
        {
            var secretBytes = Encoding.UTF8.GetBytes(gitHubAppFlavorSettings[dnsHost].WebhookSecret);
            var contentBytes = Encoding.UTF8.GetBytes(content);

            using var hmacSha256 = new HMACSHA256(secretBytes);
            var contentHash = hmacSha256.ComputeHash(contentBytes);

            return BitConverter.ToString(contentHash).Replace("-", string.Empty).ToLower();
        }
    }
}
