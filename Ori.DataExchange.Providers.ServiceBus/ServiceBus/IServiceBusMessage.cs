using System.Collections.Generic;

namespace Ori.DataExchange.Providers.ServiceBus.ServiceBus
{
    public interface IServiceBusMessage
    {
        /// <summary>
        /// Id
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Properties
        /// </summary>
        Dictionary<string, object> Properties { get; }

    }
}
