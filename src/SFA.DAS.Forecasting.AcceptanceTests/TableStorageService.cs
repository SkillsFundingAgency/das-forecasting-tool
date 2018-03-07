using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.AcceptanceTests.Infrastructure;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.AcceptanceTests
{
    public class TableStorageService
    {
        private CloudTable _table;
        private readonly Config _settings;

        public TableStorageService(Config settings)
        {
            _settings = settings;
        }

        public void SetTable(string tableName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_settings.AzureStorageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference(tableName);
        }

        public async Task Store<T>(T item, string partitionKey, string  rowKey)
        {
            EnsureExists(_table);

            var tableModel = new TableEntry
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                Data = JsonConvert.SerializeObject(item)
            };

            var op = TableOperation.InsertOrReplace(tableModel);

            await _table.ExecuteAsync(op);
        }

        public async Task<T> Retrieve<T>(string partitionKey, string rowKey)
        {
            EnsureExists(_table);

            var op = TableOperation.Retrieve<TableEntry>(partitionKey, rowKey);
            var res = await _table.ExecuteAsync(op);

            var te = (TableEntry)res.Result;
            return JsonConvert.DeserializeObject<T>(te.Data);
        }

        private void EnsureExists(CloudTable table)
        {
            if (!table.Exists())
            {
                table.Create();
            }
        }
    }

    internal class TableEntry : TableEntity
    {
        public string Data { get; set; }
    }
}
