using Microsoft.ServiceBus;
using System;
using System.Linq;
using Microsoft.ServiceBus.Messaging;


namespace Ori.DataExchange.Providers.ServiceBus.ServiceBus
{
    class ServiceBusProvider : IServiceBusProvider
    {
        private readonly NamespaceManager _namespaceManager;

        public ServiceBusProvider(string connectionString)
        {
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
            _namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
        }


        public bool TopicExist(string name)
        {
            return _namespaceManager.TopicExists(name);
        }

        public void CreateTopic(string name)
        {
            _namespaceManager.CreateTopic(name);
        }

        public bool SubscribtionExist(string topicName, string subscriptionName)
        {
            return _namespaceManager.SubscriptionExists(topicName, subscriptionName);
        }

        public void CreateSubscribtion(string topicName, string subscriptionName, string ruleName, Filter filter)
        {
            var rule = new RuleDescription(ruleName, filter);
            _namespaceManager.CreateSubscription(topicName, subscriptionName, rule);
        }

        public void CheckFilter(string topicName, string subscriptionName, string ruleName, Filter filter)
        {
            var rules = _namespaceManager.GetRules(topicName, subscriptionName);
            var result = rules.FirstOrDefault(x => x.Name == ruleName);
            if (result != null)
            {
                return; // there is a subscription with the requested filter
            }

            // if not, make it simple: just create the correct one (it won't be possible if similar subscription already exist)
            var rule = new RuleDescription(ruleName, filter);
            _namespaceManager.CreateSubscription(topicName, subscriptionName, rule);
        }
    }
}
