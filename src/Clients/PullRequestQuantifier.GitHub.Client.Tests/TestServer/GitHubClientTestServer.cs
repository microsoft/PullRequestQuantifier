namespace PullRequestQuantifier.GitHub.Client.Tests.TestServer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using System.Text;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Moq;
    using PullRequestQuantifier.Common.Azure.ServiceBus;
    using PullRequestQuantifier.Common.Azure.Telemetry;
    using PullRequestQuantifier.GitHub.Client.GitHubClient;

    [ExcludeFromCodeCoverage]
    public class GitHubClientTestServer : IDisposable
    {
        public const string TestDomain = "github.com";
        private readonly IWebHostBuilder hostBuilder;
        private readonly AzureServiceBusSettings azureServiceBusSettings;
        private readonly TestServer testServer;

        public GitHubClientTestServer()
        {
            var webhookSecret = Convert.ToBase64String(Encoding.UTF8.GetBytes("github-webhook-secret-value"));
            TestGitHubAppSettings = new GitHubAppFlavorSettings
            {
                GitHubAppsSettings = new Dictionary<string, GitHubAppSettings>
                {
                    [TestDomain] = new GitHubAppSettings
                    {
                        Id = "23",
                        Name = "fakeName",
                        EnterpriseApiRoot = "https://fakeApiRoot",
                        EnterpriseUrl = $"https://{TestDomain}",
                        PrivateKey = "fakeKey",
                        WebhookSecret = webhookSecret
                    },
                    ["github.com"] = new GitHubAppSettings
                    {
                        Id = "23",
                        Name = "fakeName",
                        EnterpriseApiRoot = "https://fakeApiRoot",
                        EnterpriseUrl = $"https://{TestDomain}",
                        PrivateKey = "fakeKey",
                        WebhookSecret = webhookSecret
                    }
                }
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
                            .AddInMemoryObject(
                                TestGitHubAppSettings.GitHubAppsSettings[TestDomain],
                                $"{nameof(GitHubAppFlavorSettings)}:GitHubAppsSettings:{TestDomain}")
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

        public GitHubAppFlavorSettings TestGitHubAppSettings { get; }

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
    }
}
