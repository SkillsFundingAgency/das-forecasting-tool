using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Commitments.Model;

namespace SFA.DAS.Forecasting.Domain.Commitments.Services
{
    public interface ICommitmentsDataService
    {
        Task<List<Commitment>> GetCurrentCommitments(long employerAccountId);
        Task Store(IEnumerable<Commitment> commitments);
    }
}