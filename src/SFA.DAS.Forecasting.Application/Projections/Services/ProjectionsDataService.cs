using SFA.DAS.Forecasting.Application.Infrastructure.Persistence;
using SFA.DAS.Forecasting.Domain.Projections.Services;
using SFA.DAS.Forecasting.Models.Projections;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Application.Projections.Services
{
    public class ProjectionsDataService : IProjectionsDataService
    {
        private readonly IDocumentSession _documentSession;

        public ProjectionsDataService(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public async Task<AccountProjectionDocument> Get(long employerId)
        {
            return await _documentSession.Get<AccountProjectionDocument>(employerId.ToString());
        }

        public async Task Store(AccountProjectionDocument accountProjections)
        {
            await _documentSession.Store(accountProjections);
        }       
    }
}
