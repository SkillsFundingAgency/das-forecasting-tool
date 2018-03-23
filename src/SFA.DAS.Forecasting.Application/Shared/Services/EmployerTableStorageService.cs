using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Models.Payments;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Application.Shared.Services
{
    public class EmployerTableStorageService : IEmployerDatabaseService
    {
        private readonly IApplicationConfiguration _settings;
        private readonly CloudTable _table;

        public EmployerTableStorageService(IApplicationConfiguration settings)
        {
            _settings = settings;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(settings.StorageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference(settings.StubEmployerPaymentTable);
        }

        public async Task<IEnumerable<EmployerPayment>> GetEmployerPayments(long accountId, int year, int month)
        {
            return await Retrieve(accountId.ToString(), $"{year}-{month}");
        }

        private async Task<IEnumerable<EmployerPayment>> Retrieve(string partitionKey, string rowKey)
        {
            EnsureExists(_table);

            var op = TableOperation.Retrieve<TableEntry>(partitionKey, rowKey);
            var res = await _table.ExecuteAsync(op);

            var te = (TableEntry)res.Result;
            if (te == null || te.Data == null)
                return new List<EmployerPayment>();

            return JsonConvert.DeserializeObject<IEnumerable<EmployerPayment>>(te.Data);
        }

        private void EnsureExists(CloudTable table)
        {
            if (!table.Exists())
            {
                table.Create();
            }
        }

        public Task<List<LevyDeclaration>> GetAccountLevyDeclarations(long accountId, string payrollYear, short payrollMonth)
        {
            throw new System.NotImplementedException();
        }
    }
}
