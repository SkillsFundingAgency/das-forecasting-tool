using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Forecasting.Application.Infrastructure.Persistence;

namespace SFA.DAS.Forecasting.Application.Infrastructure.RegistrationExtensions;

public static class CosmosDbRegistrationExtensions
{
    public static void AddCosmosDbServices(this IServiceCollection services, string connectionString, bool createIfNotExists = true)
    {
        var documentConnectionString = new DocumentSessionConnectionString { ConnectionString = connectionString };
        services.AddSingleton(documentConnectionString);

        var client = new DocumentClient(new Uri(documentConnectionString.AccountEndpoint), documentConnectionString.AccountKey);
        if (createIfNotExists)
        {
            client.CreateDatabaseIfNotExistsAsync(new Database { Id = documentConnectionString.Database }).Wait();
        }
        services.AddSingleton<IDocumentClient>(client);
        services.AddTransient<IDocumentSession>(_=> CreateDocumentSession(client, createIfNotExists));
    }

    private static IDocumentSession CreateDocumentSession(IDocumentClient client, DocumentSessionConnectionString documentConnectionString, bool createIfNotExists)
    {
        var documentCollection = new DocumentCollection();
        if (createIfNotExists)
        {
            client.CreateDatabaseIfNotExistsAsync(new Database {Id = documentConnectionString.Database}).Wait();
            client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(documentConnectionString.Database), new DocumentCollection
                {
                    Id = documentConnectionString.Collection
                },
                new RequestOptions {OfferThroughput = int.Parse((string) documentConnectionString.ThroughputOffer)}).Wait();
            documentCollection =  client
                .ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(documentConnectionString.Database, documentConnectionString.Collection))
                .Result.Resource;
        }

        return new DocumentSession(client, documentConnectionString, documentCollection);
    
    }
}