using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Infrastructure.Persistence;
using SFA.DAS.Forecasting.Models;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.AcceptanceTests
{
    internal class DocumentSession
    {
        private IDocumentClient _client;
        private string _databaseId;
        private DocumentCollection _documentCollection;

        public DocumentSession(IDocumentClient client, DocumentSessionConnectionString connectionString)
        {
            _client = client;
            _databaseId = connectionString.Database;
            _documentCollection = client
                .ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(connectionString.Database, connectionString.Collection))
                .Result.Resource;
        }

        private string GenerateDocumentId<T>(string id) where T : class, IDocument => $"{typeof(T).Name}-{id.ToLower()}";

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

        public async Task Delete<T>(string id) where T : class, IDocument
        {
            var uri = UriFactory.CreateDocumentUri(_databaseId, _documentCollection.Id, GenerateDocumentId<T>(id));
            await _client.DeleteDocumentAsync(uri);
        }

    }
}