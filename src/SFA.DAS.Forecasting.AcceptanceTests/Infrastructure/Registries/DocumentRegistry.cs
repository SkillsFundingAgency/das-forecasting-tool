using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using SFA.DAS.Forecasting.Application.Infrastructure.Persistence;
using SFA.DAS.Forecasting.Application.Infrastructure.Registries;
using StructureMap;

namespace SFA.DAS.Forecasting.AcceptanceTests.Infrastructure.Registries
{
    public class DocumentRegistry : Registry
    {
        public DocumentRegistry()
        {
            var connectionString = ConfigurationHelper.GetConnectionString("CosmosDbConnectionString");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("No 'DocumentConnectionString' connection string found.");
            var documentConnectionString = new DocumentSessionConnectionString { ConnectionString = connectionString };

            ForSingletonOf<DocumentSessionConnectionString>().Use(documentConnectionString);

            var client = new DocumentClient(new Uri(documentConnectionString.AccountEndpoint), documentConnectionString.AccountKey);
            client.CreateDatabaseIfNotExistsAsync(new Database { Id = documentConnectionString.Database }).Wait();
            ForSingletonOf<IDocumentClient>().Use(client);
            client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(documentConnectionString.Database), new DocumentCollection
                {
                    Id = documentConnectionString.Collection
                },
                new RequestOptions { OfferThroughput = int.Parse(documentConnectionString.ThroughputOffer) }).Wait();

        }
    }
}
