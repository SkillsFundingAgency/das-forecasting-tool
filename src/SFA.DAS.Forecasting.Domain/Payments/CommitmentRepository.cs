using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Commitments.Services;
using SFA.DAS.Forecasting.Domain.Payments.Services;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Domain.Payments
{
    public interface ICommitmentRepository
    {
        Task StoreCommitment(Commitment commitment);
    }

    public class CommitmentRepository : ICommitmentRepository
	{
		public ICommitmentsDataService CommitmentDataService { get; set; }


		public CommitmentRepository(ICommitmentsDataService commitmentDataService)
		{
			CommitmentDataService = commitmentDataService ?? throw new ArgumentNullException(nameof(commitmentDataService));
		}

		public async Task StoreCommitment(Commitment commitment)
		{
			await CommitmentDataService.Store(commitment);
		}
	}
}
