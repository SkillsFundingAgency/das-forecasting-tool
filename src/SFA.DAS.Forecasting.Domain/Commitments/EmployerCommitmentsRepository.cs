using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Commitments.Services;

namespace SFA.DAS.Forecasting.Domain.Commitments
{
    public interface IEmployerCommitmentsRepository
    {
        Task<EmployerCommitments> Get(long employerAccountId);
        Task<DateTime?> GetLastTimeRecieved(long employerAccountId);
    }

    public class EmployerCommitmentsRepository : IEmployerCommitmentsRepository
    {
        private readonly ICommitmentsDataService _dataService;

        public EmployerCommitmentsRepository(
            ICommitmentsDataService dataService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        }

        public async Task<EmployerCommitments> Get(long employerAccountId)
        {
            var commitments = await _dataService.GetCurrentCommitments(employerAccountId);

            return new EmployerCommitments(employerAccountId, commitments);
        }

        public async Task<DateTime?> GetLastTimeRecieved(long employerAccountId)
        {
            return await _dataService.GetLastReceivedTime(employerAccountId);
        }
    }
}