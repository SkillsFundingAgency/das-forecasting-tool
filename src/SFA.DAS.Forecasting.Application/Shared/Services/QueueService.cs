using System;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.Application.Shared.Services
{
    public interface IQueueService
    {
        void SendMessageWithVisibilityDelay<T>(T element, string queueName, TimeSpan visibilityDelay) where T : class;
    }

	public class QueueService: IQueueService
	{
		public void SendMessageWithVisibilityDelay<T>(T message, string queueName, TimeSpan visibilityDelay) 
			where  T : class 
		{
			var cloudMessage = new CloudQueueMessage(JsonConvert.SerializeObject(message));

			// Retrieve storage account from connection string.
			var storageAccount = CloudStorageAccount.Parse(
				CloudConfigurationManager.GetSetting("StorageConnectionString"));

			// Create the queue client.
			var queueClient = storageAccount.CreateCloudQueueClient();

			// Retrieve a reference to a container.
			var queue = queueClient.GetQueueReference(queueName);

			// Create the queue if it doesn't already exist
			queue.CreateIfNotExists();

			queue.AddMessage(cloudMessage, null, visibilityDelay);
		}
	}
}
