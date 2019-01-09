using Sitecore.DataExchange;

namespace Ori.DataExchange.Providers.ServiceBus.Endpoints
{
    public class ServiceBusSettings : IPlugin
    {
        public ServiceBusSettings() { }

        /// <summary>
        /// Azure bus endpoint - it can be found in Azure portal
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// A topic to which we should connect. If it doesn't exist yet, it will be created.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Identification of the market, e.g. CN. It's used in the Subscriber name 
        /// </summary>
        public string Market { get; set; }

        /// <summary>
        /// Sender identification, e.g. Orisales. It's used in the Subscriber name 
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// Number of messages that are read from ServiceBus in one (read) batch. 
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// Maximum nuber of messages that are read during one round (in several batches). 
        /// </summary>
        public int MaxNumberOfMessages { get; set; }
    }
}
