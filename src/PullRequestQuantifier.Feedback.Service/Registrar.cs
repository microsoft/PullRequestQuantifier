namespace PullRequestQuantifier.Feedback.Service
{
    using System;
    using Azure.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using PullRequestQuantifier.Common.Azure.BlobStorage;
    using PullRequestQuantifier.Feedback.Service.Models;

    public static class Registrar
    {
        internal static void AddSettings(this IConfigurationBuilder builder)
        {
            string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            builder
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{environmentName}.json", true);
            string endPoint = builder.Build().GetValue<string>("ConfigEndpoint");

            var managedIdentityCredential = new DefaultAzureCredential();
            builder.AddAzureAppConfiguration(
                    options =>
                        options.Connect(
                                new Uri(endPoint), managedIdentityCredential)
                            .ConfigureKeyVault(kv => kv.SetCredential(managedIdentityCredential)))
                .Build();
        }

        internal static IServiceCollection RegisterServices(
            this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            serviceCollection.AddLogging(b => b.AddConsole());
            var appConfiguration = configuration.GetSection(nameof(AppConfiguration)).Get<AppConfiguration>();

            serviceCollection.Configure<FeedbackForm>(configuration.GetSection(nameof(FeedbackForm)));
            serviceCollection.AddSingleton<IBlobStorage>(p => new BlobStorage(
                appConfiguration.BlobStorageAccountName,
                appConfiguration.BlobStorageKey,
                true));

            return serviceCollection;
        }
    }
}