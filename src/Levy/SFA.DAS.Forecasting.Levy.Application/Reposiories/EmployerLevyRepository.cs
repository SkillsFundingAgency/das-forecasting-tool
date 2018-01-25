using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Levy.Application.Reposiories.Models;
using SFA.DAS.Forecasting.Levy.Domain.Entities;
using SFA.DAS.Forecasting.Levy.Domain.Repositories;
using SFA.DAS.NLog.Logger;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Levy.Application.Reposiories
{
    public class EmployerLevyRepository : IEmployerLevyRepository
    {
        // ToDo: Const or config...
        private static string LevyDeclarationTableName = "LevyDeclarations";

        private readonly ILog _logger;
        private CloudTable _table;

        public EmployerLevyRepository(ILog logger, IConfig settings)
        {
            _logger = logger;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(settings.StorageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference(LevyDeclarationTableName);
        }


        public async Task StoreLevyDeclaration(LevyDeclaration levyDeclaration)
        {
            EnsureExists(_table);

            var tableModel = new TableEntry
            {
                PartitionKey = levyDeclaration.EmployerAccountId.ToString(),
                RowKey = levyDeclaration.Period,
                Data = JsonConvert.SerializeObject(levyDeclaration)
            };

            var op = TableOperation.InsertOrReplace(tableModel);

            await _table.ExecuteAsync(op);
        }

        private void EnsureExists(CloudTable table)
        {
            if (!table.Exists()) {
                table.Create();
            }
        }
    }
}
