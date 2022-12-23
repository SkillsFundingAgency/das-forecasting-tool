using System;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Core.Configuration;

namespace SFA.DAS.Forecasting.Application.Shared.Services
{
    public interface IQueueService
    {
        Task SendMessageWithVisibilityDelay<T>(T message, string queueName) where T : class;
        Task SendMessageWithVisibilityDelay<T>(T element, string queueName, TimeSpan visibilityDelay) where T : class;
    }

	public class QueueService: IQueueService
	{
	    private readonly ForecastingConfiguration _forecastingConfiguration;

	    public QueueService(ForecastingConfiguration forecastingConfiguration)
	    {
		    _forecastingConfiguration = forecastingConfiguration;
	    }

	    public async Task SendMessageWithVisibilityDelay<T>(T message, string queueName)
	        where T : class
	    {
            await SendMessageWithVisibilityDelay(message,queueName,TimeSpan.FromSeconds(_forecastingConfiguration.SecondsToWaitToAllowProjections));
	    }

        public async Task SendMessageWithVisibilityDelay<T>(T message, string queueName, TimeSpan visibilityDelay) 
			where  T : class 
		{
			var cloudMessage = new CloudQueueMessage(JsonConvert.SerializeObject(message));

			// Retrieve storage account from connection string.
			var storageAccount = CloudStorageAccount.Parse(_forecastingConfiguration.StorageConnectionString);

			// Create the queue client.
			var queueClient = storageAccount.CreateCloudQueueClient();

			// Retrieve a reference to a container.
			var queue = queueClient.GetQueueReference(queueName);

			// Create the queue if it doesn't already exist
			await queue.CreateIfNotExistsAsync();

			var random = new Random(Guid.NewGuid().GetHashCode());
		    var concurrencyMitigation = TimeSpan.FromSeconds( random.Next(0, 10));
			await queue.AddMessageAsync(cloudMessage, null, visibilityDelay.Add(concurrencyMitigation), null, null);
		}
	}
}
