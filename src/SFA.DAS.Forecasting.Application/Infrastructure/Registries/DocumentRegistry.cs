using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using SFA.DAS.Forecasting.Application.Infrastructure.Persistence;
using StructureMap;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
{
    public class DocumentRegistry : Registry
    {
        public DocumentRegistry()
        {
            var connectionString = ConfigurationHelper.GetConnectionString("DocumentConnectionString");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("No 'DocumentConnectionString' connection string found.");
            var documentConnectionString = new DocumentSessionConnectionString { ConnectionString = connectionString };
            ValidateConnectionStringSetting(documentConnectionString.Database, "Database");
            ValidateConnectionStringSetting(documentConnectionString.AccountEndpoint, "AccountEndpoint");
            ValidateConnectionStringSetting(documentConnectionString.Collection, "Collection");
            ValidateConnectionStringSetting(documentConnectionString.ThroughputOffer, "ThroughputOffer");
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

        private void ValidateConnectionStringSetting(string setting, string name)
        {
            if (string.IsNullOrEmpty(setting))
                throw new InvalidOperationException($"DocumentSessionConnectionString is missing the '{name}' setting.");

        }
    }
}