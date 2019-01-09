using Microsoft.ServiceBus.Messaging;

namespace Ori.DataExchange.Providers.ServiceBus.ServiceBus
{
    public interface IServiceBusService
    {
        /// <summary>
        /// It provides name of the subscription for the selected market.
        /// </summary>
        /// <param name="marketCode">two letter code of the market</param>
        /// <param name="sender">identification of the sender from which we will process the messages</param>
        /// <returns>It retuns null for null marketCode.</returns>
        string GetSubscriptionName(string marketCode, string sender);

        /// <summary>
        /// It returns the filter definition for the provided market.
        /// </summary>
        /// <param name="marketCode">two letter code of the market</param>
        /// <param name="sender">identification of the sender from which we will process the messages</param>
        /// <returns>It retuns null for null marketCode.</returns>
        SqlFilter GetFilter(string marketCode, string sender);

        /// <summary>
        /// Filter name for marketing automation subscription.
        /// </summary>
        /// <returns></returns>
        string GetRuleName();
    }
}
