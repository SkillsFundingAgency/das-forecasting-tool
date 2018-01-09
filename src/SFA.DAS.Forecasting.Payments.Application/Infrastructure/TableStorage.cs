using Microsoft.WindowsAzure.Storage.Table;
using SFA.DAS.PaymentsAdapter.Functions.Infrastructure.Models;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Payments.Application.Infrastructure
{
    public class TableStorage
    {
        private const string TableName = "infotable";
        private const string RowKey = "InfoLastRunKey";
        private const string PartitionKey = "InfoPartitionKey";
        private readonly CloudTable _table;

        public TableStorage(string connectionString)
        {
            var storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference(TableName);
        }

        public async Task<Info> GetLatestInfo()
        {
            var io = TableOperation.Retrieve<Info>(PartitionKey, RowKey);
            var result = await _table.ExecuteAsync(io);
            if (result.Result != null)
            {
                return (Info)result.Result;
            }
            return new Info { PageNumber = 0 };
        }

        public async Task Insert(Info data)
        {
            data.PartitionKey = PartitionKey;
            data.RowKey = RowKey;
            var op = TableOperation.InsertOrReplace(data);
            await _table.ExecuteAsync(op);
        }
    }
}
