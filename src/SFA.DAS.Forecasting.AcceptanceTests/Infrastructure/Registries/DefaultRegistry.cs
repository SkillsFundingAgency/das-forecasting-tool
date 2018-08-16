using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using SFA.DAS.Forecasting.Application.Infrastructure.Persistence;
using SFA.DAS.Forecasting.Application.Infrastructure.Registries;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Data;
using StructureMap;

namespace SFA.DAS.Forecasting.AcceptanceTests.Infrastructure.Registries
{
    public class DefaultRegistry : Registry
    {
        private const string ServiceNamespace = "SFA.DAS";

        public DefaultRegistry()
        {
            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceNamespace));
                scan.TheCallingAssembly();
                scan.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            ForSingletonOf<Config>().Use(new Config());
            
            For<IForecastingDataContext>()
                .Use<ForecastingDataContext>()
                .Ctor<IApplicationConnectionStrings>("config")
                .Is(ctx => ctx.GetInstance<Config>());

            For<IDbConnection>()
                .Use<SqlConnection>()
                .SelectConstructor(() =>
                    new SqlConnection(ConfigurationManager.ConnectionStrings["DatabaseConnectionString"]
                        .ConnectionString))
                .Ctor<string>("connectionString")
                .Is(ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString);

            For<IDocumentSession>()
                .Use(ctx => CreateDocumentSession());
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
    }
}
