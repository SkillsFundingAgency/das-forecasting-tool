using System;
using System.Threading.Tasks;
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
		public ICommitmentDataService CommitmentDataService { get; set; }


		public CommitmentRepository(ICommitmentDataService commitmentDataService)
		{
			CommitmentDataService = commitmentDataService ?? throw new ArgumentNullException(nameof(commitmentDataService));
		}

		public async Task StoreCommitment(Commitment commitment)
		{
			await CommitmentDataService.StoreCommitment(commitment);
		}
	}
}
