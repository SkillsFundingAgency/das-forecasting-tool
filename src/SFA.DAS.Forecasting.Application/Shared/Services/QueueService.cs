using System;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;

namespace SFA.DAS.Forecasting.Application.Shared.Services
{
    public interface IQueueService
    {
        void SendMessageWithVisibilityDelay<T>(T message, string queueName) where T : class;
        void SendMessageWithVisibilityDelay<T>(T element, string queueName, TimeSpan visibilityDelay) where T : class;
    }

	public class QueueService: IQueueService
	{
	    private readonly IApplicationConfiguration _configuration;

	    public QueueService(IApplicationConfiguration configuration)
	    {
	        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
	    }

	    public void SendMessageWithVisibilityDelay<T>(T message, string queueName)
	        where T : class
	    {
            SendMessageWithVisibilityDelay(message,queueName,TimeSpan.FromSeconds(_configuration.SecondsToWaitToAllowProjections));
	    }

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

		    var random = new Random(Guid.NewGuid().GetHashCode());
		    var concurrencyMitigation = TimeSpan.FromSeconds( random.Next(0, 10));
			queue.AddMessage(cloudMessage, null, visibilityDelay.Add(concurrencyMitigation));
		}
	}
}
