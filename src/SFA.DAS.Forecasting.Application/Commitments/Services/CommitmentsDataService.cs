using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Domain.Commitments.Services;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Application.Commitments.Services
{
    public class CommitmentsDataService : ICommitmentsDataService
    {
        private readonly IForecastingDataContext _dataContext;

        public CommitmentsDataService(IForecastingDataContext dataContext)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public async Task<EmployerCommitmentsModel> GetCurrentCommitments(long employerAccountId)
        {
            var model = new EmployerCommitmentsModel
            {
                LevyFundedCommitments = await GetCommitments(GetLevyFundedCommiments(employerAccountId)),
                ReceivingEmployerTransferCommitments = await GetCommitments(GetReceivingEmployerTransferCommitments(employerAccountId)),
                SendingEmployerTransferCommitments = await GetCommitments(GetSendingEmployerTransferCommitments(employerAccountId))
            };

            return model;
        }

        private async Task<List<CommitmentModel>> GetCommitments(Expression<Func<CommitmentModel, bool>> filter)
        {
            var model = new List<CommitmentModel>();
            var moreData = true;
            var skip = 0;
            while (moreData)
            {
                var commitments = await _dataContext.Commitments.Where(filter).OrderBy(c=>c.Id).Skip(skip).Take(100).ToListAsync();

                skip = skip + 100;
                if (!commitments.Any())
                {
                    moreData = false;
                }
                else
                {
                    model.AddRange(commitments);
                }
            }
            return model;
        }

        private Expression<Func<CommitmentModel, bool>> GetLevyFundedCommiments(long employerAccountId)
        {
            return commitment => commitment.EmployerAccountId == employerAccountId
                                 && commitment.FundingSource == FundingSource.Levy
                                 && commitment.ActualEndDate == null;
        }

        private Expression<Func<CommitmentModel, bool>> GetReceivingEmployerTransferCommitments(long employerAccountId)
        {
            return commitment => commitment.EmployerAccountId == employerAccountId
                                 && commitment.FundingSource == FundingSource.Transfer
                                 && commitment.ActualEndDate == null;
        }
        private Expression<Func<CommitmentModel, bool>> GetSendingEmployerTransferCommitments(long employerAccountId)
        {
            return commitment => commitment.SendingEmployerAccountId == employerAccountId
                                 && commitment.FundingSource == FundingSource.Levy
                                 && commitment.ActualEndDate == null;
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