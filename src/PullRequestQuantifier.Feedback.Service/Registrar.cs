namespace PullRequestQuantifier.Feedback.Service
{
    using System;
    using Azure.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using PullRequestQuantifier.Common.Azure.BlobStorage;

    public static class Registrar
    {
        internal static void AddSettings(this IConfigurationBuilder builder)
        {
            builder.AddJsonFile("appsettings.json", false);
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
            var pppConfiguration = configuration.GetSection(nameof(AppConfiguration)).Get<AppConfiguration>();

            serviceCollection.AddSingleton<IBlobStorage>(p => new BlobStorage(
                pppConfiguration.BlobStorageAccountName,
                pppConfiguration.BlobStorageKey,
                true));

            return serviceCollection;
        }
    }
}