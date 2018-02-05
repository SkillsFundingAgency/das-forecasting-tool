﻿using System.Collections.Generic;
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
	public class EmployerPaymentRepository : IEmployerPaymentRepository
	{
		// ToDo: Const or config...
		private static string EmployerPaymentTableName = "EmployerPayment";

		private readonly ILog _logger;
		private CloudTable _table;

		public EmployerPaymentRepository(ILog logger, IConfig settings)
		{
			_logger = logger;

			CloudStorageAccount storageAccount = CloudStorageAccount.Parse(settings.StorageConnectionString);
			var tableClient = storageAccount.CreateCloudTableClient();
			_table = tableClient.GetTableReference(EmployerPaymentTableName);
		}

		public async Task StorePayment(Payment payment)
		{
			EnsureExists(_table);

			var tableModel = new TableEntry
			{
				PartitionKey = payment.EmployerAccountId.ToString(),
				RowKey = payment.Id,
				Data = JsonConvert.SerializeObject(payment)
			};

			var op = TableOperation.InsertOrReplace(tableModel);

			await _table.ExecuteAsync(op);
		}

		public List<Payment> GetPayments(string partitionKey)
		{
			EnsureExists(_table);

			var entities = _table.ExecuteQuery(new TableQuery<TableEntry>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey)));

			return entities
				.Select(tableEntry => JsonConvert.DeserializeObject<Payment>(tableEntry.Data))
				.ToList();
		}

		public List<Payment> GetPayments(string employerAccountId, int month, int year)
		{
			EnsureExists(_table);

			var entities = _table.ExecuteQuery(new TableQuery<TableEntry>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, employerAccountId)));

			return entities
				.Select(tableEntry => JsonConvert.DeserializeObject<Payment>(tableEntry.Data))
				.Where(x => x.CollectionPeriod.Month == month && x.CollectionPeriod.Year == year)
				.ToList();
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
