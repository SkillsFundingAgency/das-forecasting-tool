using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Models;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Persistence;

public interface IDocumentSession
{
    IQueryable<T> CreateQuery<T>(int maxItemCount = -1) where T : class, IDocument;
    Task<T> Get<T>(string id) where T : class, IDocument;
    Task<Document> GetDocument(string id);
    Task Store<T>(T item) where T : class, IDocument;
    Task Store(Document document);
    Task Delete(string id);
}

public class DocumentSession : IDocumentSession
{
    private readonly IDocumentClient _client;
    private readonly DocumentCollection _documentCollection;
    private readonly string _databaseId;
    public DocumentSession(IDocumentClient client, DocumentSessionConnectionString connectionString, DocumentCollection documentCollection)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _databaseId = connectionString.Database;
        _documentCollection = documentCollection;
    }

    public IQueryable<T> CreateQuery<T>(int maxItemCount = -1) where T : class, IDocument
    {
        return _client.CreateDocumentQuery<DocumentAdapter<T>>(_documentCollection.SelfLink, new FeedOptions { MaxItemCount = maxItemCount })
            .Where(adapter => adapter.type == typeof(T).FullName)
            .Select(adapter => adapter.Document);
    }

    public static string GenerateDocumentId<T>(string id) where T : class, IDocument => $"{typeof(T).Name}-{id.ToLower()}";

    public async Task<T> Get<T>(string id) where T : class, IDocument
    {
        try
        {
            var response = await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _documentCollection.Id, GenerateDocumentId<T>(id)));
            var adapter = JsonConvert.DeserializeObject<DocumentAdapter<T>>(response.Resource.ToString());
            return adapter?.Document;
        }
        catch (DocumentClientException exception)
        {
            if (exception.StatusCode == HttpStatusCode.NotFound)
                return null;
            throw;
        }
    }

    public async Task<Document> GetDocument(string id)
    {
        try
        {
            var response = await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _documentCollection.Id, id));
            return response.Resource;
        }
        catch (DocumentClientException exception)
        {
            if (exception.StatusCode == HttpStatusCode.NotFound)
                return null;
            throw;
        }
    }

    public async Task Store<T>(T item) where T : class, IDocument
    {
        await _client.UpsertDocumentAsync(_documentCollection.SelfLink, new DocumentAdapter<T>(GenerateDocumentId<T>(item.Id), item));
    }

    public async Task Store(Document document)
    {
        await _client.UpsertDocumentAsync(_documentCollection.SelfLink, document);
    }

    public async Task Delete(string id)
    {
        try
        {
            await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _documentCollection.Id, id));
        }
        catch (DocumentClientException exception)
        {
            if (exception.StatusCode == HttpStatusCode.NotFound)
                return;
            throw;
        }
    }
}