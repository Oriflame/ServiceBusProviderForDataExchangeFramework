using Sitecore.DataExchange.Attributes;
using Sitecore.DataExchange.Converters.PipelineSteps;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Plugins;
using Sitecore.DataExchange.Repositories;
using Sitecore.Services.Core.Model;

namespace Ori.DataExchange.Providers.ServiceBus.ReadData
{
    [SupportedIds(ReadServiceBusMessagesStepTemplateId)]
    class ReadServiceBusMessagesStepConverter : BasePipelineStepConverter
    {
        public const string ReadServiceBusMessagesStepTemplateId = "{C18CDB2C-A7DF-46F4-BC02-AFD72EFE2C34}";
        public const string TemplateFieldEndpointFrom = "EndpointFrom";
        public ReadServiceBusMessagesStepConverter(IItemModelRepository repository) : base(repository)
        {
        }
        protected override void AddPlugins(ItemModel source, PipelineStep pipelineStep)
        {
            //
            //create the plugin
            var settings = new EndpointSettings
            {
                //
                //populate the plugin using values from the item
                EndpointFrom = this.ConvertReferenceToModel<Endpoint>(source, TemplateFieldEndpointFrom)
            };
            //
            //add the plugin to the pipeline step
            pipelineStep.AddPlugin(settings);
        }
    }
}
