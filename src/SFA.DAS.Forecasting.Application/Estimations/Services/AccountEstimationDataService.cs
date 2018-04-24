using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Persistence;
using SFA.DAS.Forecasting.Domain.Estimations.Services;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.Estimations.Services
{
    public class AccountEstimationDataService : IAccountEstimationDataService
    {
        private readonly IDocumentSession _documentSession;

        public AccountEstimationDataService(IDocumentSession documentSession)
        {
            _documentSession = documentSession ?? throw new ArgumentNullException(nameof(documentSession));
        }

        public async Task<AccountEstimationModel> Get(long accountId)
        {
            return await _documentSession.Get<AccountEstimationModel>(accountId.ToString());
        }

        public async Task Store(AccountEstimationModel model)
        {
            await _documentSession.Store(model);
        }
    }
}