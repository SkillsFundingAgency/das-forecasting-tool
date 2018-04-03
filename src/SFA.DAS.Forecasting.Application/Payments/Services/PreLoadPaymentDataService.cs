using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Models.Payments;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Application.Payments.Services
{
    public class PreLoadPaymentDataService
    {
        private readonly CloudTable _pTable;
        private readonly CloudTable _eTable;

        public PreLoadPaymentDataService(IApplicationConfiguration settings)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(settings.StorageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            _pTable = tableClient.GetTableReference("PaymentTable");
            _eTable = tableClient.GetTableReference("EarningsTable");
        }

        public IEnumerable<EmployerPayment> GetPayments(long employerId)
        {
            EnsureExists(_pTable);

            var entities = _pTable
                .ExecuteQuery(new TableQuery<TableEntry>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, employerId.ToString())));

            return entities
                .Select(tableEntry => JsonConvert.DeserializeObject<EmployerPayment>(tableEntry.Data))
                .ToList();
        }

        public IEnumerable<EarningDetails> GetEarningDetails(long employerId)
        {
            EnsureExists(_eTable);

            var entities = _eTable.ExecuteQuery(new TableQuery<TableEntry>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, employerId.ToString())));

            return entities
                .Select(tableEntry => JsonConvert.DeserializeObject<EarningDetails>(tableEntry.Data))
                .ToList();
        }

        public async Task StoreEarningDetails(long employerAccountId, EarningDetails earningDetails)
        {
            EnsureExists(_eTable);

            var tableModel = new TableEntry
            {
                PartitionKey = employerAccountId.ToString(),
                RowKey = earningDetails.PaymentId.ToLower(),
                Data = JsonConvert.SerializeObject(earningDetails)
            };

            var op = TableOperation.InsertOrReplace(tableModel);

            await _eTable.ExecuteAsync(op);
        }

        public async Task StorePayment(EmployerPayment payment)
        {
            EnsureExists(_pTable);

            var tableModel = new TableEntry
            {
                PartitionKey = payment.AccountId.ToString(),
                RowKey = payment.PaymentId.ToString().ToLower(),
                Data = JsonConvert.SerializeObject(payment)
            };

            var op = TableOperation.InsertOrReplace(tableModel);

            await _pTable.ExecuteAsync(op);
        }

        public async Task DeleteEarningDetails(long employerAccountId)
        {
            EnsureExists(_eTable);

            var opb = new TableBatchOperation();

            var entities = _eTable
                .ExecuteQuery(new TableQuery<TableEntry>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, employerAccountId.ToString())));

            foreach (var item in entities)
            {
                opb.Delete(item);
            }

            if(opb.Any())
                await _eTable.ExecuteBatchAsync(opb);
        }

        public async Task DeletePayment(long employerAccountId)
        {
            EnsureExists(_pTable);

            var opb = new TableBatchOperation();

            var entities = _pTable
                .ExecuteQuery(new TableQuery<TableEntry>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, employerAccountId.ToString())));

            foreach (var item in entities)
            {
                opb.Delete(item);
            }

            if (opb.Any())
                await _pTable.ExecuteBatchAsync(opb);
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
