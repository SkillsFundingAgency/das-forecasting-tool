using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using SFA.DAS.Forecasting.Models.Estimation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using static SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition.DisplayModelledApprenticeshipsSteps;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests.Infrastructure
{
    public class ApprenticeshipDataService
    {
        public async Task UpsertApprenticeshipModel(IEnumerable<TestApprenticeship> apprenticeships)
        {
            var vas = apprenticeships.Select(m => 
            {
                return new VirtualApprenticeship
                {
                    Id = Guid.NewGuid().ToString("N"),
                    ApprenticesCount = m.NumberOfApprentices,
                    CourseId = "rockstar-999",
                    CourseTitle = m.Apprenticeship,
                    Level = 3,
                    StartDate = DateTime.Parse($"{m.StartDateYear}-{m.NumberOfMonths}-1"),
                    TotalCost = int.Parse(m.TotalCost),
                    TotalCompletionAmount = 800,
                    TotalInstallments = m.NumberOfMonths,
                    TotalInstallmentAmount = 700,
                    FundingSource = m.FundingSource == "Levy" ? Models.Payments.FundingSource.Levy : Models.Payments.FundingSource.Transfer
                };
            });

            var model = new AccountEstimationModel
            {
                Id = "12345",
                EstimationName = "default",
                EmployerAccountId = 12345,
                Apprenticeships = vas.ToList()
            };

            var document = new
            {
                id = $"{nameof(AccountEstimationModel)}-{model.Id}",
                type = $"{typeof(AccountEstimationModel)}",
                Document = model
            };

            await Run(async (c, dc) =>
            {
    await c.UpsertDocumentAsync(dc.SelfLink, document);
                });
        }


        public async Task Run(Func<DocumentClient, DocumentCollection, Task> func)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["CosmosDbConnectionString"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("No 'DocumentConnectionString' connection string found.");
            var documentConnectionString = new DocumentSessionConnectionString { ConnectionString = connectionString };


            using (var client = new DocumentClient(new Uri(documentConnectionString.AccountEndpoint), documentConnectionString.AccountKey))
            {
                var documentCollection = client
                    .ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(documentConnectionString.Database, documentConnectionString.Collection))
                    .Result.Resource;
                await func(client, documentCollection);
            }
        }
    }
}
