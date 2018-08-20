using System;
using System.Configuration;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Infrastructure.Persistence;
using SFA.DAS.Forecasting.Application.Infrastructure.Registries;
using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Messages.Projections;
using SFA.DAS.Forecasting.Models.Balance;

namespace SFA.DAS.Forecasting.PerformanceTests
{
    [TestFixture]
    public class BaseProcessingTests
    {
        protected long[] AccountIds { get; set; } = { 9912345, 9923451, 9934512, 9945123, 9951234, 9954321 };
        protected ForecastingDataContext DataContext { get; private set; }
        [SetUp]
        public void SetUp()
        {
            DataContext = new ForecastingDataContext(ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString);
        }
        protected IDocumentSession CreateDocumentSession()
        {
            var connectionString = ConfigurationHelper.GetConnectionString("CosmosDbConnectionString");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("No 'DocumentConnectionString' connection string found.");
            var documentConnectionString = new DocumentSessionConnectionString { ConnectionString = connectionString };

            var client = new DocumentClient(new Uri(documentConnectionString.AccountEndpoint), documentConnectionString.AccountKey);
            client.CreateDatabaseIfNotExistsAsync(new Microsoft.Azure.Documents.Database { Id = documentConnectionString.Database }).Wait();

            client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(documentConnectionString.Database), new DocumentCollection
                {
                    Id = documentConnectionString.Collection
                },
                new RequestOptions { OfferThroughput = int.Parse(documentConnectionString.ThroughputOffer) }).Wait();
            return new DocumentSession(client, documentConnectionString);
        }

        protected void RemoveEmployerProjectionAuditDocuments(ProjectionSource projectionSource, params long[] employerAccountIds)
        {
            var session = CreateDocumentSession();
            RemoveEmployerProjectionAuditDocuments(session, projectionSource, employerAccountIds);
        }

        protected void RemoveEmployerProjectionAuditDocuments(IDocumentSession session, ProjectionSource projectionSource, params long[] employerAccountIds)
        {
            foreach (var employerAccountId in employerAccountIds)
            {
                var docId = $"employerprojectionaudit-{projectionSource.ToString("G").ToLower()}-{employerAccountId}";
                session.Delete(docId).Wait();
            }
        }

        protected void CreateAccountBalances()
        {
            var balances = DataContext.Balances.Where(balance => AccountIds.Contains(balance.EmployerAccountId)).ToList();
            foreach (var accountId in AccountIds)
            {
                var balance = balances.FirstOrDefault(b => b.EmployerAccountId == accountId) ?? DataContext.Balances.Add(new BalanceModel { EmployerAccountId = accountId, Amount = 500000, BalancePeriod = DateTime.Now, ReceivedDate = DateTime.Now, RemainingTransferBalance = 50000, TransferAllowance = 50000, UnallocatedCompletionPayments = 0 });
                balance.ReceivedDate = DateTime.Now;
            }
            DataContext.SaveChanges();
        }
    }
}