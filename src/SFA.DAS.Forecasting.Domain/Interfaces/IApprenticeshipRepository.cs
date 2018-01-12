using System.Collections.Generic;
using System.Threading.Tasks;

using SFA.DAS.Forecasting.Domain.Entities;

namespace SFA.DAS.Forecasting.Domain.Interfaces
{
    public interface IApprenticeshipRepository
    {
        Task<IEnumerable<Apprenticeship>> GetApprenticeships(long employerId);
    }
}