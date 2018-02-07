using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Application.Payments.Repositories.Models;
using SFA.DAS.Forecasting.Domain.Payments.Entities;
using SFA.DAS.Forecasting.Domain.Payments.Repositories;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Payments.Repositories
{
	public class TrainingCostRepository : ITrainingCostRepository
	{
		// ToDo: Const or config...
		private static string EmployerPaymentsTotalCostsTableName = "TrainingCosts";

		private readonly ILog _logger;
		private CloudTable _table;

		public TrainingCostRepository(ILog logger, IConfig settings)
		{
			_logger = logger;

			CloudStorageAccount storageAccount = CloudStorageAccount.Parse(settings.StorageConnectionString);
			var tableClient = storageAccount.CreateCloudTableClient();
			_table = tableClient.GetTableReference(EmployerPaymentsTotalCostsTableName);
		}

		public async Task StoreTrainingCost(EmployerTotalCostOfTraining employerTotalCostOfTraining)
		{
			EnsureExists(_table);

			var tableModel = new TableEntry
			{
				PartitionKey = employerTotalCostOfTraining.EmployerAccountId.ToString(),
				RowKey = $"{employerTotalCostOfTraining.PeriodYear}-{employerTotalCostOfTraining.PeriodMonth}",
				Data = JsonConvert.SerializeObject(employerTotalCostOfTraining)
			};

			var op = TableOperation.InsertOrReplace(tableModel);

			await _table.ExecuteAsync(op);
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
