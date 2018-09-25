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
        private readonly CloudTable _paymentTableNoCommitment;
		private readonly CloudTable _earningTable;
		private readonly CloudTable _earningTableNoCommitment;

		public PreLoadPaymentDataService(IApplicationConfiguration settings)
        {
            var storageAccount = CloudStorageAccount.Parse(settings.StorageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            _paymentTable = tableClient.GetTableReference("PaymentTable");
            _paymentTableNoCommitment = tableClient.GetTableReference("PaymentTableNoCommitment");
			_earningTable = tableClient.GetTableReference("EarningsTable");
			_earningTableNoCommitment = tableClient.GetTableReference("EarningsTableNoCommitment");
		}

		public IEnumerable<EmployerPayment> GetPayments(long employerId)
		{
			return GetPayments(_paymentTable, employerId);
		}

	    public IEnumerable<EmployerPayment> GetPaymentsNoCommitment(long employerId)
	    {
		    return GetPayments(_paymentTableNoCommitment, employerId);
	    }

	    private IEnumerable<EmployerPayment> GetPayments(CloudTable cloudTable, long employerId)
	    {
		    EnsureExists(cloudTable);

		    var entities = cloudTable
				.ExecuteQuery(new TableQuery<TableEntry>()
				    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, employerId.ToString())));

		    return entities
			    .Select(tableEntry => JsonConvert.DeserializeObject<EmployerPayment>(tableEntry.Data))
			    .ToList();
	    }

	    public IEnumerable<EarningDetails> GetEarningDetails(long employerId)
		{
			return GetEarningDetails(_earningTable, employerId);
		}

		public IEnumerable<EarningDetails> GetEarningDetailsNoCommitment(long employerId)
		{
			return GetEarningDetails(_earningTableNoCommitment, employerId);
		}

	    private IEnumerable<EarningDetails> GetEarningDetails(CloudTable cloudTable, long employerId)
	    {
		    EnsureExists(cloudTable);

		    var entities = cloudTable.ExecuteQuery(new TableQuery<TableEntry>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, employerId.ToString())));

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
	        await StorePayment(_paymentTable, payment);
		}

		public async Task StorePaymentNoCommitment(EmployerPayment payment)
		{
			await StorePayment(_paymentTableNoCommitment, payment);
		}

	    private async Task StorePayment(CloudTable cloudTable, EmployerPayment payment)
	    {
			EnsureExists(cloudTable);

		    var tableModel = new TableEntry
		    {
			    PartitionKey = payment.AccountId.ToString(),
			    RowKey = payment.PaymentId.ToString().ToLower(),
			    Data = JsonConvert.SerializeObject(payment)
		    };

		    var op = TableOperation.InsertOrReplace(tableModel);

		    await cloudTable.ExecuteAsync(op);
		}

		public async Task DeleteEarningDetails(long employerAccountId)
        {
            await DeleteEmployerData(_earningTable, employerAccountId);
		}

	    public async Task DeleteEarningDetailsNoCommitment(long employerAccountId)
	    {
			await DeleteEmployerData(_earningTableNoCommitment, employerAccountId);
		}

		public async Task DeletePayment(long employerAccountId)
        {
            await DeleteEmployerData(_paymentTable, employerAccountId);
		}

	    public async Task DeletePaymentNoCommitment(long employerAccountId)
	    {
			await DeleteEmployerData(_paymentTableNoCommitment, employerAccountId);
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
