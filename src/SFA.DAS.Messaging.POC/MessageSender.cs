using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace SFA.DAS.Messaging.POC
{
    public interface IMessageSender
    {
        Task Send<T>(string endpoint, T message);
    }

    public class MessageSender : IMessageSender
    {
        public CloudQueueClient CloudQueueClient { get; }

        public MessageSender(CloudQueueClient cloudQueueClient)
        {
            CloudQueueClient = cloudQueueClient ?? throw new ArgumentNullException(nameof(cloudQueueClient));
        }

        public async Task Send<T>(string endpoint, T message)
        {
            //TODO: assumes endpoint is in same storage account as sender.
            var queue = CloudQueueClient.GetQueueReference(endpoint);
            await queue.CreateIfNotExistsAsync();  //TODO: probably don't want to do this in real implementation
            await queue.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(message)));
        }
    }
}