namespace PullRequestQuantifier.Common.Azure.ServiceBus
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Azure.Messaging.ServiceBus;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json.Linq;
    using PullRequestQuantifier.Common.Azure.Telemetry;

    [ExcludeFromCodeCoverage]
    public class AzureServiceBus : IEventBus
    {
        private readonly AzureServiceBusSettings settings;
        private readonly ServiceBusClient serviceBusClient;
        private readonly ServiceBusSender sender;
        private readonly ServiceBusProcessor processor;
        private readonly IAppTelemetry telemetry;
        private readonly ILogger<AzureServiceBus> logger;

        public AzureServiceBus(
            IOptions<AzureServiceBusSettings> settings,
            IAppTelemetry telemetry,
            ILogger<AzureServiceBus> logger)
        {
            this.telemetry = telemetry;
            this.logger = logger;
            this.settings = settings.Value;

            serviceBusClient = new ServiceBusClient(
                this.settings.ConnectionString,
                new ServiceBusClientOptions
                {
                    TransportType = ServiceBusTransportType.AmqpWebSockets
                });

            // create a sender for the topic
            sender = serviceBusClient.CreateSender(this.settings.TopicName);

            // create a processor that we can use to process the messages
            processor = serviceBusClient.CreateProcessor(
                this.settings.TopicName,
                this.settings.SubscriptionName,
                new ServiceBusProcessorOptions
                {
                    MaxConcurrentCalls = 10,
                    MaxAutoLockRenewalDuration = TimeSpan.FromDays(3) // auto lock renewal max 3 days to allow us to process long running events
                });
        }

        public async Task WriteAsync(JObject payload)
        {
            if (payload == null)
            {
                return;
            }

            var message = new ServiceBusMessage(payload.ToString())
            {
                CorrelationId = telemetry.OperationId
            };
            await sender.SendMessageAsync(message);
        }

        public async Task SubscribeAsync(
            Func<string, DateTimeOffset, Task> messageHandler,
            Func<Exception, Task> errorHandler,
            CancellationToken cancellationToken)
        {
            // add handler to process messages
            processor.ProcessMessageAsync += async args =>
            {
                using var operation = telemetry.StartOperation<RequestTelemetry>(this, args.Message.CorrelationId);

                var messageProcessingStartDelay = DateTimeOffset.UtcNow - args.Message.EnqueuedTime;
                telemetry.RecordMetric(
                    "StartMessageProcessing-Delay",
                    (long)messageProcessingStartDelay.TotalSeconds);

                await messageHandler(args.Message.Body.ToString(), args.Message.EnqueuedTime);

                // delete the message
                await args.CompleteMessageAsync(args.Message, cancellationToken);
            };

            // add handler to process any errors
            processor.ProcessErrorAsync += async args => { await errorHandler(args.Exception); };

            // start processing
            await processor.StartProcessingAsync(cancellationToken);
        }

        public async void Dispose()
        {
            // stop processing
            await processor.StopProcessingAsync();

            await processor.DisposeAsync();
            await sender.DisposeAsync();
            await serviceBusClient.DisposeAsync();
        }
    }
}
