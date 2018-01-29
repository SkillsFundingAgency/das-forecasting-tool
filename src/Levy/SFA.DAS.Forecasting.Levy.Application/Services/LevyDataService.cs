using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Levy.Application.Services.Models;
using SFA.DAS.Forecasting.Levy.Domain.Model;
using SFA.DAS.Forecasting.Levy.Domain.Services;

namespace SFA.DAS.Forecasting.Levy.Application.Services
{
    public class LevyDataService : ILevyDataService
    {
        private readonly CloudTable _table;

        public LevyDataService(IConfig settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            var storageAccount = CloudStorageAccount.Parse(settings.StorageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference(settings.LevyDeclarationTableName);
            EnsureExists(_table);
        }

        public async Task<List<LevyDeclaration>> GetLevyDeclarationsForPeriod(long employerAccountId, string periodYear, int month)
        {
            var partitionKey = GetPartitionKey(employerAccountId, periodYear, month);
            TableContinuationToken continuationToken = null;
            var levyDeclarations = new List<LevyDeclaration>();
            do
            {
                var results = await _table.ExecuteQuerySegmentedAsync(
                    new TableQuery<TableEntry>().Where(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey)),
                    continuationToken);
                continuationToken = results.ContinuationToken;
                levyDeclarations.AddRange(results.Select(item => JsonConvert.DeserializeObject<LevyDeclaration>(item.Data)));
            } while (continuationToken != null);

            return levyDeclarations;
        }

        private static string GetPartitionKey(long employerAccountId, string periodYear, int periodMonth)
        {
            return $"{employerAccountId}_{periodYear}_{periodMonth}".ToLower().Replace("-", "_").Replace("/","_");
        }

        public async Task StoreLevyDeclaration(LevyDeclaration levyDeclaration)
        {
            EnsureExists(_table);

            var tableModel = new TableEntry
            {
                PartitionKey = GetPartitionKey(levyDeclaration.EmployerAccountId, levyDeclaration.PayrollYear, levyDeclaration.PayrollMonth),
                RowKey = levyDeclaration.Scheme,
                Data = JsonConvert.SerializeObject(levyDeclaration)
            };

            await _table.ExecuteAsync(TableOperation.InsertOrReplace(tableModel));
        }

        private void EnsureExists(CloudTable table)
        {
            if (!table.Exists())
            {
                table.Create();
            }
        }
    }
}
