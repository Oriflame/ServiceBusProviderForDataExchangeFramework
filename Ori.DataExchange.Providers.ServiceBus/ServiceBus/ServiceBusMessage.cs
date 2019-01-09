using System.Collections.Generic;

namespace Ori.DataExchange.Providers.ServiceBus.ServiceBus
{
    public class ServiceBusMessage : IServiceBusMessage
    {
        public string Id { get; set; }
        public Dictionary<string, object> Properties { get; }

        public ServiceBusMessage()
        {
            Properties = new Dictionary<string, object>();
        }
    }
}
