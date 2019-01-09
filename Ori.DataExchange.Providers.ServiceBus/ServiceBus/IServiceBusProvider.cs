using Microsoft.ServiceBus.Messaging;

namespace Ori.DataExchange.Providers.ServiceBus.ServiceBus
{
    public interface IServiceBusProvider
    {
        bool TopicExist(string name);

        void CreateTopic(string name);

        bool SubscribtionExist(string topicName, string subscriptionName);

        void CreateSubscribtion(string topicName, string subscriptionName, string ruleName, Filter filter);

        /// <summary>
        /// If the provided filter doesn't exist on subscribtion yet, create it.
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="subscriptionName"></param>
        /// <param name="ruleName"></param>
        /// <param name="filter"></param>
        void CheckFilter(string topicName, string subscriptionName, string ruleName, Filter filter);

    }
}
