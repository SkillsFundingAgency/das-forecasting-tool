using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Domain.Projections.Services
{
    public interface IProjectionsDataService
    {
        Task<AccountProjectionDocument> Get(long employerId);

        Task Store(AccountProjectionDocument accountProjections);
    }
}
