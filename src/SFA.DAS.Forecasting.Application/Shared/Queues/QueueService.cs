using System;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.Application.Shared.Queues
{
	public class QueueService
	{
		public void SendMessageWithVisibilityDelay<T>(T element, string queueName, int visibilityDelay) 
			where  T : class 
		{
			var message = new CloudQueueMessage(JsonConvert.SerializeObject(element));

			// Retrieve storage account from connection string.
			CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
				CloudConfigurationManager.GetSetting("StorageConnectionString"));

			// Create the queue client.
			CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

			// Retrieve a reference to a container.
			CloudQueue queue = queueClient.GetQueueReference(queueName);

			// Create the queue if it doesn't already exist
			queue.CreateIfNotExists();

			queue.AddMessage(message, null, TimeSpan.FromMinutes(visibilityDelay));
		}
	}
}
