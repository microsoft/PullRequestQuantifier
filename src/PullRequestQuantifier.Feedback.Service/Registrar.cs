namespace PullRequestQuantifier.Feedback.Service
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using PullRequestQuantifier.Common.Azure.BlobStorage;

    public static class Registrar
    {
        internal static IServiceCollection RegisterServices(
            this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            // todo key vault and get secrets from there
            serviceCollection.AddSingleton<IBlobStorage>(p => new BlobStorage(
                string.Empty,
                string.Empty,
                true));

            return serviceCollection;
        }
    }
}