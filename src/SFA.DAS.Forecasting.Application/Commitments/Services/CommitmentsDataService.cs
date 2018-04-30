using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Domain.Commitments.Services;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Application.Commitments.Services
{
    public class CommitmentsDataService : ICommitmentsDataService
    {
        private readonly IForecastingDataContext _dataContext;

        public CommitmentsDataService(IForecastingDataContext dataContext)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public async Task<List<CommitmentModel>> GetCurrentCommitments(long employerAccountId)
        {
            return await _dataContext.Commitments
                .Where(commitment => commitment.EmployerAccountId == employerAccountId && 
                                     commitment.ActualEndDate == null)
                .ToListAsync();
        }

        public async Task<CommitmentModel> Get(long employerAccountId, long apprenticeshipId)
        {
            return await _dataContext.Commitments
                .FirstOrDefaultAsync(commitment =>
                    commitment.EmployerAccountId == employerAccountId &&
                    commitment.ApprenticeshipId == apprenticeshipId);
        }

        public async Task Store(CommitmentModel commitment)
        {
            if (commitment.Id <= 0)
                _dataContext.Commitments.Add(commitment);

            await _dataContext.SaveChangesAsync();
        }
    }
}