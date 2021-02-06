namespace PullRequestQuantifier.GitHub.Client.Tests.TestServer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Moq;
    using PullRequestQuantifier.GitHub.Client.Events;
    using PullRequestQuantifier.GitHub.Client.GitHubClient;
    using PullRequestQuantifier.GitHub.Client.Models;
    using PullRequestQuantifier.GitHub.Client.Telemetry;
    using Xunit;

    public class GitHubClientTestServer : IDisposable
    {
        private readonly IWebHostBuilder hostBuilder;
        private readonly AzureServiceBusSettings azureServiceBusSettings;
        private readonly TestServer testServer;

        public GitHubClientTestServer()
        {
            var webhookSecret = Convert.ToBase64String(Encoding.UTF8.GetBytes("github-webhook-secret-value"));
            TestGitHubAppSettings = new GitHubAppSettings
            {
                Id = "23",
                Name = "fakeName",
                EnterpriseApiRoot = "https://fakeApiRoot",
                EnterpriseUrl = "https://fakeUrl",
                PrivateKey = "fakeKey",
                WebhookSecret = webhookSecret
            };
            azureServiceBusSettings = new AzureServiceBusSettings
            {
                ConnectionString = "fakeString",
                SubscriptionName = "fakeSub",
                TopicName = "fakeTopic"
            };
            hostBuilder = new WebHostBuilder()
                .ConfigureServices(
                    (context, services) =>
                    {
                        context.Configuration = new ConfigurationBuilder()
                            .AddInMemoryObject(TestGitHubAppSettings, nameof(GitHubAppSettings))
                            .AddInMemoryObject(azureServiceBusSettings, nameof(AzureServiceBusSettings))
                            .AddInMemoryCollection(
                                new List<KeyValuePair<string, string>>
                                {
                                    new KeyValuePair<string, string>("ApplicationInsights:InstrumentationKey", Guid.NewGuid().ToString())
                                })
                            .Build();
                        AddMockTelemetry(services);
                        AddInMemoryEventBus(services);
                        services.RegisterServices(context.Configuration);
                    })
                .Configure(
                    (context, app) => { })
                .UseStartup<Startup>();
            testServer = new TestServer(hostBuilder);
        }

        public GitHubAppSettings TestGitHubAppSettings { get; }

        public InMemoryEventBus InMemoryEventBus { get; private set; }

        public HttpClient CreateClient()
        {
            return testServer.CreateClient();
        }

        public void Dispose()
        {
            testServer?.Dispose();
        }

        private void AddInMemoryEventBus(IServiceCollection services)
        {
            InMemoryEventBus = new InMemoryEventBus();
            services.TryAddSingleton<IEventBus>(InMemoryEventBus);
        }

        private void AddMockTelemetry(IServiceCollection services)
        {
            var mockTelemetry = new Mock<IAppTelemetry>();
            services.TryAddSingleton(_ => mockTelemetry.Object);
        }

        private string ComputeHash(string request, string secretValue)
        {
            var secretBytes = Encoding.ASCII.GetBytes(secretValue);
            var requestBytes = Encoding.ASCII.GetBytes(request);

#pragma warning disable CA5350 // GitHub sends webhook encoded with sha1
            var hmacSha1 = new HMACSHA1(secretBytes);
#pragma warning restore CA5350 // GitHub sends webhook encoded with sha1
            var hashBytes = hmacSha1.ComputeHash(requestBytes);

            return $"sha1={BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower()}";
        }
    }
}
