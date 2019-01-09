using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Ori.DataExchange.Providers.ServiceBus.Endpoints;
using Ori.DataExchange.Providers.ServiceBus.ServiceBus;
using Sitecore.DataExchange.Attributes;
using Sitecore.DataExchange.Contexts;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Plugins;
using Sitecore.DataExchange.Processors.PipelineSteps;
using Sitecore.Services.Core.Diagnostics;


namespace Ori.DataExchange.Providers.ServiceBus.ReadData
{
    [RequiredEndpointPlugins(typeof(ServiceBusSettings))]
    public class ReadServiceBusMessagesStepProcessor : BaseReadDataStepProcessor
    {
        /// <summary>
        /// Reasonable maximum of message received in one batch round.
        /// </summary>
        internal const int MaximumNumberOfMessages = 1 * 1000 * 1000;

        /// <summary>
        /// 2000 looks like a reasonable maximum. From measurements the optimal batch size is 500. Service bus didn't returned more than 700 messages even with batch size 2000.
        /// </summary>
        internal const int MaximumServiceBusMessagesBatchSize = 2 * 1000;

        internal const int MessageBusWaitTime = 10;

        protected override void ReadData(Endpoint endpoint, PipelineStep pipelineStep, PipelineContext pipelineContext, ILogger logger)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }
            if (pipelineStep == null)
            {
                throw new ArgumentNullException(nameof(pipelineStep));
            }
            if (pipelineContext == null)
            {
                throw new ArgumentNullException(nameof(pipelineContext));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            var settings = endpoint.GetServiceBusSettings();
            if (settings == null)
            {
                logger.Error("Unable to read data, there are no settings set in the ednpoint.");
                return;
            }

            // Check the endpoint settings
            if (settings.MaxNumberOfMessages <= 0 || settings.MaxNumberOfMessages > MaximumNumberOfMessages)
            {
                logger.Error($"Invalid settings. {nameof(settings.MaxNumberOfMessages)} have to be between 0 and {MaximumNumberOfMessages}. (pipeline step: {pipelineStep.Name}, endpoint: {endpoint.Name})");
                return;
            }
            if (settings.BatchSize <= 0 || settings.BatchSize > MaximumServiceBusMessagesBatchSize)
            {
                logger.Error($"Invalid settings. {nameof(settings.BatchSize)} have to be between 0 and {MaximumServiceBusMessagesBatchSize}. (pipeline step: {pipelineStep.Name}, endpoint: {endpoint.Name})");
                return;
            }
            if (settings.BatchSize > settings.MaxNumberOfMessages)
            {
                logger.Error($"Invalid settings. {nameof(settings.BatchSize)} ({settings.BatchSize}) have to be <= {nameof(settings.MaxNumberOfMessages)} ({settings.MaxNumberOfMessages}). (pipeline step: {pipelineStep.Name}, endpoint: {endpoint.Name})");
                return;
            }

            // Create the ServiceBus Reciever
            ServiceBusReceiver messageReceiver;
            try
            {
                var serviceBusService = new ServiceBusService();
                messageReceiver = new ServiceBusReceiver(MessagingFactory.CreateFromConnectionString(
                        settings.ConnectionString),
                    settings.Topic,
                    serviceBusService.GetSubscriptionName(settings.Market, settings.Sender),
                    logger);
            }
            catch (Exception exception)
            {
                logger.Error($"Unable to connect with ServiceBus ({exception.Message}). Please run the Troubleshooter to propper set the connection first.");
                return;
            }


            // read messages from the ServiceBus (in batches, up to the maximum number of messages)
            var dataList = RecieveMessages(settings, messageReceiver, logger).ToList();
            if (!dataList.Any())
            {
                logger.Info("No data read.");
                return;
            }

            logger.Debug($"{dataList.Count} messages read from ServiceBus.");
            var dataSettings = new IterableDataSettings(dataList);

            //
            //add the plugin to the pipeline context
            pipelineContext.AddPlugin(dataSettings);

        }

        private IEnumerable<Dictionary<string, object>> RecieveMessages(ServiceBusSettings settings, ServiceBusReceiver reciever, ILogger logger)
        {
            int total = 0;
            int batchSize;


            while (true)
            {
                if (settings.MaxNumberOfMessages - total >= settings.BatchSize)
                {
                    batchSize = settings.BatchSize;
                }
                else
                {
                    batchSize = settings.MaxNumberOfMessages - total;
                }

                var messages = new IServiceBusMessage[0];
                if (batchSize <= 0)
                {
                    break;
                }


                var messagesTask = Task.Run(() =>
                {
                    try
                    {
                        return reciever.ReceiveMessageBatchAsync(batchSize, TimeSpan.FromSeconds(MessageBusWaitTime));
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"Cannot receive messages. {Environment.NewLine}{ex}");
                        return Task.FromResult(new IServiceBusMessage[0]);
                    }
                });
                messages = messagesTask.Result ?? new IServiceBusMessage[0];
                total += messages.Length;


                // no message in queue => let's finish
                if (messages.Length == 0)
                {
                    break;
                }

                // enumerate messages
                for (int j = 0; j < messages.Length; j++)
                {
                    var oneLevelMessage = new Dictionary<string, object>(messages[j].Properties);
                    oneLevelMessage.Add("ID", messages[j].Id);
                    logger.Debug($"Enumerating message with consultant '{messages[j].Id}'.");
                    logger.Debug($"oneLevelMessage: '{string.Join(";", oneLevelMessage)}'.");

                    yield return oneLevelMessage;
                }
            }
        }
    }
}
