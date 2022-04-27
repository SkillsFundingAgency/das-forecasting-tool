using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Domain.Commitments.Services
{
    public interface ICommitmentsDataService
    {
        Task<EmployerCommitmentsModel> GetCurrentCommitments(long employerAccountId, DateTime? forecastLimitDate = null);
        Task<CommitmentModel> Get(long employerAccountId, long apprenticeshipId);
        Task Store(CommitmentModel commitment);
        Task<DateTime?> GetLastReceivedTime(long employerAccountId);
        Task DeletePledgeApplicationCommitmentsForSendingEmployer(long employerAccountId);
    }
}