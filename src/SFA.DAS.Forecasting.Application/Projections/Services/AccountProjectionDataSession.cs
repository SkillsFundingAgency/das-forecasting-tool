using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Persistence;
using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Domain.Projections.Services;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Projections;
using AccountProjection = SFA.DAS.Forecasting.Domain.Projections.AccountProjection;

namespace SFA.DAS.Forecasting.Application.Projections.Services
{
    public class AccountProjectionDataSession : IAccountProjectionDataSession
    {
        private readonly IDocumentSession _session;

        public AccountProjectionDataSession(IDocumentSession session, IForecastingDataContext dataContext)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public async Task<AccountProjectionModel> Get(long employerAccountId)
        {
            return await _session.Get<AccountProjectionModel>(employerAccountId.ToString());
        }

        public async Task Store(AccountProjectionModel accountProjection)
        {
            await _session.Store(accountProjection);
        }
    }
}
