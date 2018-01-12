using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Payments.Application.Infrastructure.Models;
using SFA.DAS.Forecasting.Payments.Messages.Events;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Payments.Application.Infrastructure
{
    public class PaymentsRepository
    {
        private const string TableName = "payments";
        private const string PartitionKey = "Payments";
        private readonly CloudTable _table;


        public PaymentsRepository(string connectionString)
        {
            var storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference(TableName);
        }

        public async Task StorePayment(PaymentEvent payment)
        {
            var data = new TableRow();
            data.PartitionKey = PartitionKey;
            data.RowKey = payment.Id;
            data.Data = JsonConvert.SerializeObject(payment);
            
            var op = TableOperation.InsertOrReplace(data);
            await _table.ExecuteAsync(op);
        }
    }
}