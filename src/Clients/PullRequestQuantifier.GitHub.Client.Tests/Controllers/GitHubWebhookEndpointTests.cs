namespace PullRequestQuantifier.GitHub.Client.Tests.Controllers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using PullRequestQuantifier.GitHub.Client.Models;
    using PullRequestQuantifier.GitHub.Client.Tests.TestServer;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class GitHubWebhookEndpointTests : IDisposable
    {
        private readonly HttpClient httpClient;
        private readonly GitHubClientTestServer testServer;
        private readonly JToken testWebhookData;

        public GitHubWebhookEndpointTests()
        {
            testServer = new GitHubClientTestServer();
            httpClient = testServer.CreateClient();
            testWebhookData = JToken.Parse(File.ReadAllTextAsync("Controllers/Data/TestGitHubWebhook1.json").Result);
        }

        [Fact]
        public async Task Accepts_PullRequestOpened()
        {
            var response = await GetServerResponseAsync(GitHubEventActions.Pull_Request, GitHubEventActions.Opened);

            response.EnsureSuccessStatusCode();
            Assert.Single(testServer.InMemoryEventBus.Events);
        }

        [Fact]
        public async Task Accepts_PullRequestSynchronize()
        {
            var response = await GetServerResponseAsync(GitHubEventActions.Pull_Request, GitHubEventActions.Synchronize);

            response.EnsureSuccessStatusCode();
            Assert.Single(testServer.InMemoryEventBus.Events);
        }

        [Fact]
        public async Task Accepts_InstallationCreated()
        {
            var response = await GetServerResponseAsync(GitHubEventActions.Installation, GitHubEventActions.Created);

            response.EnsureSuccessStatusCode();
            Assert.Single(testServer.InMemoryEventBus.Events);
        }

        [Fact]
        public async Task Rejects_PullRequestCreated()
        {
            var response = await GetServerResponseAsync(GitHubEventActions.Pull_Request, GitHubEventActions.Created);

            response.EnsureSuccessStatusCode();
            Assert.Empty(testServer.InMemoryEventBus.Events);
        }

        [Fact]
        public async Task Rejects_InstallationOpened()
        {
            var response = await GetServerResponseAsync(GitHubEventActions.Installation, GitHubEventActions.Opened);

            response.EnsureSuccessStatusCode();
            Assert.Empty(testServer.InMemoryEventBus.Events);
        }

        public void Dispose()
        {
            httpClient?.Dispose();
            testServer?.Dispose();
        }

        private async Task<HttpResponseMessage> GetServerResponseAsync(GitHubEventActions eventActionToSend, GitHubEventActions actionActionToSend)
        {
            testWebhookData["action"] = actionActionToSend.ToString();

            var content = new StringContent(
                testWebhookData.ToString(Formatting.None),
                Encoding.UTF8);

            var webhookSignature = ComputeHash(
                testWebhookData.ToString(Formatting.None),
                testServer.TestGitHubAppSettings[GitHubClientTestServer.TestDomain].WebhookSecret);
            httpClient.DefaultRequestHeaders.Add("X-GitHub-Event", eventActionToSend.ToString().ToLower());
            httpClient.DefaultRequestHeaders.Add("X-GitHub-Delivery", Guid.NewGuid().ToString());
            httpClient.DefaultRequestHeaders.Add("X-Hub-Signature-256", webhookSignature);
            return await httpClient.PostAsync("/githubWebhook", content);
        }

        private string ComputeHash(string request, string secretValue)
        {
            var secretBytes = Encoding.ASCII.GetBytes(secretValue);
            var requestBytes = Encoding.ASCII.GetBytes(request);
            var hmacSha256 = new HMACSHA256(secretBytes);
            var hashBytes = hmacSha256.ComputeHash(requestBytes);
            return $"sha256={BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower()}";
        }
    }
}
