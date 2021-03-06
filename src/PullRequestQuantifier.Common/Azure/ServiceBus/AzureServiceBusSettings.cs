namespace PullRequestQuantifier.Common.Azure.ServiceBus
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class AzureServiceBusSettings
    {
        public string ConnectionString { get; set; }

        public string TopicName { get; set; }

        public string SubscriptionName { get; set; }
    }
}