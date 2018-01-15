using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace SFA.DAS.Messaging.POC
{
    public interface ISubscriptionService
    {
        Task AddSubscription<T>(string queue);
        Task<List<Subscription>> GetSubscriptions<T>();
    }

    public class SubscriptionService : ISubscriptionService
    {
        protected CloudTable SubscriptionsTable { get; set; }
        public SubscriptionService(CloudTableClient cloudTableClient)
        {
            SubscriptionsTable = cloudTableClient?.GetTableReference("MessageSubscriptions") ?? throw new ArgumentNullException(nameof(cloudTableClient));
        }

        public async Task AddSubscription<T>(string queue)
        {
            await SubscriptionsTable.CreateIfNotExistsAsync();
            await SubscriptionsTable.ExecuteAsync(
                TableOperation.InsertOrReplace(new TableEntityAdapter<Subscription>(new Subscription { QueueName = queue, MessageTypeName = typeof(T).FullName }, GetPartitionKey<T>(), queue.ToLower())));
        }

        public async Task<List<Subscription>> GetSubscriptions<T>()
        {
            await SubscriptionsTable.CreateIfNotExistsAsync();
            var partitionKey = GetPartitionKey<T>();
            var subscriptions = new List<Subscription>();
            TableContinuationToken token = null;
            do
            {
                var queryResult = await SubscriptionsTable.ExecuteQuerySegmentedAsync(new TableQuery<TableEntityAdapter<Subscription>>()
                        .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,
                            partitionKey)), token);
                token = queryResult.ContinuationToken;
                subscriptions.AddRange(queryResult.Results.Select(adapter => adapter.OriginalEntity));
            } while (token != null);
            return subscriptions;
        }

        public static string GetPartitionKey<T>() => typeof(T).FullName.ToLower();
    }
}