using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Levy.Application.Messages;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Forecasting.AcceptanceTests
{
    public class AzureTableService
    {
        private readonly CloudTable _table;

        public AzureTableService(string connectionString, string tableName)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference(tableName);
        }

        internal void EnsureExcists()
        {

            if (!_table.Exists())
            {
                _table.Create();
            }
        }

        internal void ClearTable()
        {
            _table.DeleteIfExists();
        }

        internal void DeleteEntities(string partitionKey)
        {
            var query = new TableQuery<TablrRow>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));
            foreach (var item in _table.ExecuteQuery(query))
            {
                var oper = TableOperation.Delete(item);
                _table.Execute(oper);
            }
        }

        internal IEnumerable<LevyDeclarationEvent> GetRecords(string partitionKey)
        {
            var list = new List<LevyDeclarationEvent>();

            var query = new TableQuery<TablrRow>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

            return _table.ExecuteQuery(query)
                .Select(entity => JsonConvert.DeserializeObject<LevyDeclarationEvent>(entity.Data));
        }
    }

    public class TablrRow : TableEntity
    {
        public string Data { get; set; }
    }
}