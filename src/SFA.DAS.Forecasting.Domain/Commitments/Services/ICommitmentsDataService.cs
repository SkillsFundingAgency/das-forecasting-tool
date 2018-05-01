using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Domain.Commitments.Services
{
    public interface ICommitmentsDataService
    {
        Task<List<CommitmentModel>> GetCurrentCommitments(long employerAccountId);
        
        Task<CommitmentModel> Get(long employerAccountId, long apprenticeshipId);
        Task Store(CommitmentModel commitment);
    }
}