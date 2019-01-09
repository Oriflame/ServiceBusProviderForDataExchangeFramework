using System;
using System.Threading.Tasks;

namespace Ori.DataExchange.Providers.ServiceBus.ServiceBus
{
    public interface IServiceBusReceiver
    {

        /// <summary>
        /// Receive messages asynchronously.
        /// </summary>
        /// <param name="messageCount">Number of messages to receive.</param>
        /// <param name="serverWaitTime">Wait time for server to respond.</param>
        /// <returns></returns>
        Task<IServiceBusMessage[]> ReceiveMessageBatchAsync(int messageCount, TimeSpan serverWaitTime);
    }
}
