using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Sitecore.Services.Core.Diagnostics;
using Newtonsoft.Json;

namespace Ori.DataExchange.Providers.ServiceBus.ServiceBus
{
    class ServiceBusReceiver : IServiceBusReceiver
    {
        private readonly SubscriptionClient _subscriptionClient;
        private readonly ILogger _logger;


        public ServiceBusReceiver(MessagingFactory messagingFactory, string topic, string subscription, ILogger logger)
        {
            if (topic == null) throw new ArgumentNullException(nameof(topic));
            if (subscription == null) throw new ArgumentNullException(nameof(subscription));
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            _logger = logger;

            _subscriptionClient = messagingFactory?.CreateSubscriptionClient(topic, subscription) ?? throw new ArgumentNullException(nameof(messagingFactory));
        }

        public async Task<IServiceBusMessage[]> ReceiveMessageBatchAsync(int messageCount, TimeSpan serverWaitTime)
        {
            IEnumerable<BrokeredMessage> messages = await ReceiveBatchAsync(messageCount, serverWaitTime);

            var tasks = new List<Task<ServiceBusMessage>>();

            foreach (BrokeredMessage brokeredMessage in messages)
            {
                tasks.Add(CreateMessageAsync(brokeredMessage));
            }
            return await Task.WhenAll(tasks);
        }


        private Task<IEnumerable<BrokeredMessage>> ReceiveBatchAsync(int messageCount, TimeSpan serverWaitTime)
        {
            return _subscriptionClient.ReceiveBatchAsync(messageCount, serverWaitTime);
        }


        private async Task<ServiceBusMessage> CreateMessageAsync(BrokeredMessage brokeredMessage)
        {
            var message = await Deserialize(brokeredMessage, true);
            return message;
        }


        public async Task<ServiceBusMessage> Deserialize(BrokeredMessage message, bool autocompleteMessage)
        {
            return await DeserializeInternal(message, autocompleteMessage);
        }

        private async Task<ServiceBusMessage> DeserializeInternal(BrokeredMessage message, bool completeMessage = true)
        {
            if (message == null)
            {
                return null;
            }

            ServiceBusMessage result = null;

            try
            {
                string bodyString;

                using (Stream body = message.GetBody<Stream>())
                {
                    using (StreamReader reader = new StreamReader(body))
                    {
                        bodyString = await reader.ReadToEndAsync();
                    }
                }

                bodyString = PrepareDeserilizationBody(bodyString);

                result = JsonDeserialization(bodyString);

            }
            catch (Exception ex)
            {
                _logger.Error("Unable to deserialize message content, message will be completed.", ex);
            }
            finally
            {
                if (completeMessage)
                {
                    await message.CompleteAsync();
                }
            }

            return result;
        }

        protected string PrepareDeserilizationBody(string bodyString)
        {
            if (!string.IsNullOrEmpty(bodyString))
            {
                var jsonStart = bodyString.IndexOf("{", StringComparison.Ordinal);
                var jsonEnd = bodyString.LastIndexOf("}", StringComparison.Ordinal) + 1;
                var jsonLength = jsonEnd - jsonStart;

                bodyString = bodyString.Substring(jsonStart, jsonLength);
            }

            return bodyString;
        }

        protected ServiceBusMessage JsonDeserialization(string bodyString)
        {
            var result = JsonConvert.DeserializeObject<ServiceBusMessage>(bodyString);

            return result;
        }


    }
}
