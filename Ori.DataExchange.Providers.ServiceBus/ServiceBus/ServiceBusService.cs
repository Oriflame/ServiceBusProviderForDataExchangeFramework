using Microsoft.ServiceBus.Messaging;

namespace Ori.DataExchange.Providers.ServiceBus.ServiceBus
{
    public class ServiceBusService : IServiceBusService
    {
        private const string SubscriptionNamePrefix = "SC9_";
        private const string RuleName = "MarketingAutomation";

        /// <summary>
        /// Naming pattern: SC9_XY_Sender (XY - market code, Sender - sender identification)
        /// </summary>
        /// <param name="marketCode"></param>
        /// <param name="sender"></param>
        /// <returns>It retuns null for null marketCode.</returns>
        public string GetSubscriptionName(string marketCode, string sender)
        {
            if (marketCode == null)
                return null;

            return SubscriptionNamePrefix + marketCode.ToUpperInvariant() + "_" + sender;
        }

        public SqlFilter GetFilter(string marketCode, string sender)
        {
            if (marketCode == null)
                return null;

            return new SqlFilter(
                $@"(EXISTS(user.country_code) AND user.country_code = '{marketCode.ToUpperInvariant()}') 
                AND (EXISTS(user.sender) AND user.sender = '{sender}')");
        }

        public string GetRuleName()
        {
            return RuleName;
        }
    }
}
