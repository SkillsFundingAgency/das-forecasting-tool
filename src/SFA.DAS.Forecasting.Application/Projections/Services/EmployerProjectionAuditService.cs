using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Infrastructure.Persistence;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Application.Projections.Services
{
    public interface IEmployerProjectionAuditService
    {
        Task<bool> RecordRunOfProjections(long employerAccountId);
    }

    public class EmployerProjectionAuditService : IEmployerProjectionAuditService
    {
        private readonly IDocumentSession _documentSession;

        public EmployerProjectionAuditService(IDocumentSession documentSession)
        {
            _documentSession = documentSession ?? throw new ArgumentNullException(nameof(documentSession));
        }

        public async Task<bool> RecordRunOfProjections(long employerAccountId)
        {
            try
            {
                var docId = $"employerprojectionaudit-{employerAccountId}";
                var doc = await _documentSession.GetDocument(docId);
                if (doc != null)
                {
                    var lastRunTime = doc.GetPropertyValue<DateTimeOffset>("lastRunTime");
                    if (lastRunTime >= DateTimeOffset.UtcNow.AddMinutes(-10))
                        return false;
                }
                else
                {
                    doc = new Document();
                    doc.SetPropertyValue("id", docId);
                }

                doc.SetPropertyValue("employerAccountId", employerAccountId);
                doc.SetPropertyValue("lastRunTime", DateTimeOffset.UtcNow);
                await _documentSession.Store(doc);
            }
            catch (DocumentClientException dce)
            {
                //   now notice the failure when attempting the update 
                //   this is because the ETag on the server no longer matches the ETag of doc (b/c it was changed in step 2)
                if (dce.StatusCode == HttpStatusCode.PreconditionFailed)
                {
                    //no need to record the audit as a concurrent thread has already recorded the audit
                    return false;
                }
            }
            return true;
        }
    }
}