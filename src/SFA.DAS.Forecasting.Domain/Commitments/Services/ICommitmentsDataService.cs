using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Domain.Commitments.Services
{
    public interface ICommitmentsDataService
    {
        Task<List<Commitment>> GetCurrentCommitments(long employerAccountId);
        Task<decimal> GetOverdueCompletionPayments(long employerAccountId);
        Task Store(IEnumerable<Commitment> commitments);
    }
}