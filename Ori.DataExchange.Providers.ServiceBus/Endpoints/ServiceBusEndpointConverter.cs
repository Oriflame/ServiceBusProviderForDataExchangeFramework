using Sitecore.DataExchange.Attributes;
using Sitecore.DataExchange.Converters.Endpoints;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Repositories;
using Sitecore.Services.Core.Model;

namespace Ori.DataExchange.Providers.ServiceBus.Endpoints
{
    [SupportedIds(ServiceBusEndpointTemplateId)]
    public class ServiceBusEndpointConverter : BaseEndpointConverter
    {
        public const string ServiceBusEndpointTemplateId = "{3F5EAAD4-17F8-4896-8C1A-AF2F6CBECCA0}";
        public const string TemplateFieldConnectionString = "ConnectionString";
        public const string TemplateFieldTopic = "Topic";
        public const string TemplateFieldMarket = "Market";
        public const string TemplateFieldSender = "Sender";
        public const string TemplateFieldBatchSize = "BatchSize";
        public const string TemplateFieldMaxNumberOfMessages = "MaxNumberOfMessages";

        public ServiceBusEndpointConverter(IItemModelRepository repository) : base(repository)
        {
        }
        protected override void AddPlugins(ItemModel source, Endpoint endpoint)
        {
            //
            //create the plugin
            var settings = new ServiceBusSettings
            {
                //
                //populate the plugin using values from the item
                ConnectionString = GetStringValue(source, TemplateFieldConnectionString),
                Topic = GetStringValue(source, TemplateFieldTopic),
                Market = GetStringValue(source, TemplateFieldMarket),
                Sender = GetStringValue(source, TemplateFieldSender),
                BatchSize = GetIntValue(source, TemplateFieldBatchSize),
                MaxNumberOfMessages = GetIntValue(source, TemplateFieldMaxNumberOfMessages)
            };
            //
            //add the plugin to the endpoint
            endpoint.AddPlugin(settings);
        }

    }
}
