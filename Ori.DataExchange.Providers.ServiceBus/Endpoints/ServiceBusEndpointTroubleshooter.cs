using System;
using Ori.DataExchange.Providers.ServiceBus.ServiceBus;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Troubleshooters;

namespace Ori.DataExchange.Providers.ServiceBus.Endpoints
{
    public class ServiceBusEndpointTroubleshooter : BaseEndpointTroubleshooter
    {

        /// <summary>
        /// Check the connection, topic and subscriber. Create the topic / subscriber if needed (not exist yet).
        /// Subscriber name pattern: SC9_MC (market code).
        /// Note: we're checking the filter as well
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override ITroubleshooterResult Troubleshoot(Endpoint endpoint, TroubleshooterContext context)
        {
            // data validation
            if (endpoint == null)
                return TroubleshooterResult.FailResult("The endpoint is null.");

            var settings = endpoint.GetPlugin<ServiceBusSettings>();
            if (settings == null)
            {
                return TroubleshooterResult.FailResult("The endpoint is missing the ServiceBusSettings plugin.");
            }

            var result = new TroubleshooterResult();

            bool wasWrong = false;
            if (string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                wasWrong = true;
                result.Data.Add("ConnectionString", "There is no Connection String in the endpoint configuration.");
            }
            if (string.IsNullOrWhiteSpace(settings.Topic))
            {
                wasWrong = true;
                result.Data.Add("Topic", "There is no Topic in the endpoint configuration.");
            }
            if (string.IsNullOrWhiteSpace(settings.Market))
            {
                wasWrong = true;
                result.Data.Add("Market", "There is no Market in the endpoint configuration.");
            }
            if (wasWrong)
            {
                result.Success = false;
                result.Message = "Wrong configuration.";
                return result;
            }

            // filter preparation:
            try
            {
                var serviceBusService = new ServiceBusService();
                var subscriptionName = serviceBusService.GetSubscriptionName(settings.Market, settings.Sender);
                var filter = serviceBusService.GetFilter(settings.Market, settings.Sender);
                var ruleName = serviceBusService.GetRuleName();

                // test the connection to selected bus
                var serviceBus = new ServiceBusProvider(settings.ConnectionString);

                // check the topic existence
                if (!serviceBus.TopicExist(settings.Topic))
                {
                    // try to create one
                    try
                    {
                        serviceBus.CreateTopic(settings.Topic);
                        result.Data.Add("Topic", $"The '{settings.Topic}' topic has been created.");
                    }
                    catch (Exception exception)
                    {
                        return TroubleshooterResult.FailResult($"Unable to create the '{settings.Topic}' topic.",
                            exception);
                    }
                }
                else
                {
                    result.Data.Add("Topic", $"The '{settings.Topic}' topic already exist.");
                }

                // check the subscription existence
                if (!serviceBus.SubscribtionExist(settings.Topic, subscriptionName))
                {
                    // try to create one
                    try
                    {
                        serviceBus.CreateSubscribtion(settings.Topic, subscriptionName, ruleName, filter);
                        result.Data.Add("Subscription", $"The '{subscriptionName}' subscription has been created.");
                    }
                    catch (Exception exception)
                    {
                        return TroubleshooterResult.FailResult(
                            $"Unable to create the '{subscriptionName}' subscription.", exception);
                    }
                }
                else
                {
                    result.Data.Add("Subscription", $"The '{subscriptionName}' subscription already exist.");
                }

                // check the filter
                try
                {
                    serviceBus.CheckFilter(settings.Topic, subscriptionName, ruleName, filter);
                    result.Data.Add("Filter", "The filter has been checked successfully.");
                }
                catch (Exception exception)
                {
                    return TroubleshooterResult.FailResult($"Unable to check filter for '{subscriptionName}' subscription: {exception.Message}", exception);
                }
            }
            catch (Exception e)
            {
                return TroubleshooterResult.FailResult($"Something is wrong with ServiceBus: {e.Message}", e);
            }

            result.Success = true;
            result.Message = "The Service Bus endpoint is OK.";
            return result;
        }
    }
}
