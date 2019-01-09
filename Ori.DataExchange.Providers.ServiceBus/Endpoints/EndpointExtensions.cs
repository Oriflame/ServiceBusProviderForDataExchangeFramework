using Sitecore.DataExchange.Models;

namespace Ori.DataExchange.Providers.ServiceBus.Endpoints
{
    public static class EndpointExtensions
    {
        public static ServiceBusSettings GetServiceBusSettings(this Endpoint endpoint)
        {
            return endpoint.GetPlugin<ServiceBusSettings>();
        }
        public static bool HasServiceBusSettings(this Endpoint endpoint)
        {
            return (GetServiceBusSettings(endpoint) != null);
        }
    }
}
