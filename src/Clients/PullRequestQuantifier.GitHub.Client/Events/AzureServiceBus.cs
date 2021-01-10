namespace PullRequestQuantifier.GitHub.Client.Events
***REMOVED***
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json.Linq;

    public class AzureServiceBus : IEventBus
    ***REMOVED***
        private readonly AzureServiceBusSettings settings;
        private readonly ServiceBusClient serviceBusClient;
        private readonly ServiceBusSender sender;
        private readonly ServiceBusProcessor processor;

        public AzureServiceBus(IOptions<AzureServiceBusSettings> settings)
        ***REMOVED***
            this.settings = settings.Value;
            serviceBusClient = new ServiceBusClient(this.settings.ConnectionString);

            // create a sender for the topic
            sender = serviceBusClient.CreateSender(this.settings.TopicName);

            // create a processor that we can use to process the messages
            processor = serviceBusClient.CreateProcessor(
                this.settings.TopicName,
                this.settings.SubscriptionName,
                new ServiceBusProcessorOptions
                ***REMOVED***
                    MaxConcurrentCalls = 10
        ***REMOVED***);
***REMOVED***

        public async Task WriteAsync(JObject payload)
        ***REMOVED***
            if (payload == null)
            ***REMOVED***
                return;
    ***REMOVED***

            await sender.SendMessageAsync(new ServiceBusMessage(payload.ToString()));
***REMOVED***

        public async Task SubscribeAsync(
            Func<string, Task> messageHandler,
            Func<Exception, Task> errorHandler,
            CancellationToken cancellationToken)
        ***REMOVED***
            // add handler to process messages
            processor.ProcessMessageAsync += async args =>
            ***REMOVED***
                await messageHandler(args.Message.Body.ToString());

                // delete the message
                await args.CompleteMessageAsync(args.Message, cancellationToken);
    ***REMOVED***;

            // add handler to process any errors
            processor.ProcessErrorAsync += async args => await errorHandler(args.Exception);

            // start processing
            await processor.StartProcessingAsync(cancellationToken);
***REMOVED***

        public async void Dispose()
        ***REMOVED***
            // stop processing
            await processor.StopProcessingAsync();

            await processor.DisposeAsync();
            await sender.DisposeAsync();
            await serviceBusClient.DisposeAsync();
***REMOVED***
***REMOVED***
***REMOVED***