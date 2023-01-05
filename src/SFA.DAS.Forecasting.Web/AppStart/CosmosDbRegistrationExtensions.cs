using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Forecasting.Application.Infrastructure.Persistence;
using SFA.DAS.Forecasting.Core.Configuration;

namespace SFA.DAS.Forecasting.Web;

public static class CosmosDbRegistrationExtensions
{
    public static void AddCosmosDbServices(this IServiceCollection services, ForecastingConfiguration configuration, bool createIfNotExists = true)
    {
        var documentConnectionString = new DocumentSessionConnectionString { ConnectionString = configuration.CosmosDbConnectionString };
        services.AddSingleton(documentConnectionString);

        var client = new DocumentClient(new Uri(documentConnectionString.AccountEndpoint), documentConnectionString.AccountKey);
        if (createIfNotExists)
        {
            client.CreateDatabaseIfNotExistsAsync(new Database { Id = documentConnectionString.Database }).Wait();
        }
        services.AddSingleton<IDocumentClient>(client);
        services.AddTransient<IDocumentSession>(_=> CreateDocumentSession(configuration.CosmosDbConnectionString, createIfNotExists));
        if (createIfNotExists)
        {
            client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(documentConnectionString.Database), new DocumentCollection
                {
                    Id = documentConnectionString.Collection
                },
                new RequestOptions { OfferThroughput = int.Parse(documentConnectionString.ThroughputOffer) }).Wait();    
        }
    }

    public static IDocumentSession CreateDocumentSession(string connectionString, bool createIfNotExists)
    {
        var documentConnectionString = new DocumentSessionConnectionString { ConnectionString = connectionString };

        var client = new DocumentClient(new Uri(documentConnectionString.AccountEndpoint), documentConnectionString.AccountKey);
        var documentCollection = new DocumentCollection();
        if (createIfNotExists)
        {
            client.CreateDatabaseIfNotExistsAsync(new Database {Id = documentConnectionString.Database}).Wait();
            client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(documentConnectionString.Database), new DocumentCollection
                {
                    Id = documentConnectionString.Collection
                },
                new RequestOptions {OfferThroughput = int.Parse(documentConnectionString.ThroughputOffer)}).Wait();
            documentCollection =  client
                .ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(documentConnectionString.Database, documentConnectionString.Collection))
                .Result.Resource;
        }

        return new DocumentSession(client, documentConnectionString, documentCollection);
    
    }
}