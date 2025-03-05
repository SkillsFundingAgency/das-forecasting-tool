using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using SFA.DAS.Forecasting.Application.Infrastructure.Persistence;

namespace SFA.DAS.Forecasting.Application.Projections.Services;

public interface IEmployerProjectionAuditService
{
    Task<bool> RecordRunOfProjections(long employerAccountId, string source);
}

public class EmployerProjectionAuditService : IEmployerProjectionAuditService
{
    private readonly IDocumentSession _documentSession;

    public EmployerProjectionAuditService(IDocumentSession documentSession)
    {
        _documentSession = documentSession ?? throw new ArgumentNullException(nameof(documentSession));
    }

    public async Task<bool> RecordRunOfProjections(long employerAccountId, string source)
    {
        try
        {
            var docId = $"employerprojectionaudit-{source.ToLower()}-{employerAccountId}";
            var doc = await _documentSession.GetDocument(docId);
            if (doc != null)
            {
                var lastRunTime = doc.GetPropertyValue<DateTimeOffset>("lastRunTime");
                var allowTime = DateTime.UtcNow.AddMinutes(-10);
                if (lastRunTime >= allowTime)
                    return false;
            }
            else
            {
                doc = new Document();
                doc.SetPropertyValue("id", docId);
            }

            doc.SetPropertyValue("employerAccountId", employerAccountId);
            doc.SetPropertyValue("lastRunTime", DateTime.UtcNow);
            await _documentSession.Store(doc);
        }
        catch (DocumentClientException dce)
        {
            //   now notice the failure when attempting the update 
            //   this is because the ETag on the server no longer matches the ETag of doc (b/c it was changed by another thread)
            if (dce.StatusCode == HttpStatusCode.PreconditionFailed)
            {
                //no need to record the audit as a concurrent thread has already recorded the audit
                return false;
            }
            throw;
        }
        return true;
    }
}