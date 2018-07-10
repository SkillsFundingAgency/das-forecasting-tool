using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Domain.Projections.Services
{
    public interface IAccountProjectionDataSession
    {
        Task<AccountProjectionModel> Get(long employerId);
        Task Store(AccountProjectionModel accountProjection);
    }
}
