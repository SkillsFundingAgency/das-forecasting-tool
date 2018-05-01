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
        private readonly CloudTable _paymentTable;
        private readonly CloudTable _earningTable;

        public PreLoadPaymentDataService(IApplicationConfiguration settings)
        {
            var storageAccount = CloudStorageAccount.Parse(settings.StorageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            _paymentTable = tableClient.GetTableReference("PaymentTable");
            _earningTable = tableClient.GetTableReference("EarningsTable");
        }

        public IEnumerable<EmployerPayment> GetPayments(long employerId)
        {
            EnsureExists(_paymentTable);

            var entities = _paymentTable
                .ExecuteQuery(new TableQuery<TableEntry>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, employerId.ToString())));

            return entities
                .Select(tableEntry => JsonConvert.DeserializeObject<EmployerPayment>(tableEntry.Data))
                .ToList();
        }

        public IEnumerable<EarningDetails> GetEarningDetails(long employerId)
        {
            EnsureExists(_earningTable);

            var entities = _earningTable.ExecuteQuery(new TableQuery<TableEntry>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, employerId.ToString())));

            return entities
                .Select(tableEntry => JsonConvert.DeserializeObject<EarningDetails>(tableEntry.Data))
                .ToList();
        }

        public async Task StoreEarningDetails(long employerAccountId, EarningDetails earningDetails)
        {
            EnsureExists(_earningTable);

            var tableModel = new TableEntry
            {
                PartitionKey = employerAccountId.ToString(),
                RowKey = earningDetails.PaymentId.ToLower(),
                Data = JsonConvert.SerializeObject(earningDetails)
            };

            var op = TableOperation.InsertOrReplace(tableModel);

            await _earningTable.ExecuteAsync(op);
        }

        public async Task StorePayment(EmployerPayment payment)
        {
            EnsureExists(_paymentTable);

            var tableModel = new TableEntry
            {
                PartitionKey = payment.AccountId.ToString(),
                RowKey = payment.PaymentId.ToString().ToLower(),
                Data = JsonConvert.SerializeObject(payment)
            };

            var op = TableOperation.InsertOrReplace(tableModel);

            await _paymentTable.ExecuteAsync(op);
        }

        public async Task DeleteEarningDetails(long employerAccountId)
        {
            await DeleteEmployerData(_earningTable, employerAccountId);
        }

        public async Task DeletePayment(long employerAccountId)
        {
            await DeleteEmployerData(_paymentTable, employerAccountId);
        }

        private static async Task DeleteEmployerData(CloudTable cloudTable, long employerAccountId)
        {
            if (!await cloudTable.ExistsAsync())
                return;

            var entities = cloudTable
                .ExecuteQuery(new TableQuery<TableEntry>()
                    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, employerAccountId.ToString())))
                .ToList();

            while (entities.Any())
            {
                var batch = entities.Take(99).ToList();
                var batchOperation = new TableBatchOperation();
                foreach (var item in batch)
                    batchOperation.Delete(item);
                await cloudTable.ExecuteBatchAsync(batchOperation);
                entities = entities.Skip(99).ToList();
            }
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
