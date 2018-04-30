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
        
        public async Task<decimal> GetPendingCompletionPayments(long employerAccountId)
        {
            //_dataContext.Commitments
            //    .Where(m => m.EmployerAccountId == employerAccountId)
            //    .Where(m => m.ActualEndDate == null);

            //return await WithConnection(async cnn =>
            //{
            //    var parameters = new DynamicParameters();
            //    parameters.Add("@employerAccountId", employerAccountId, DbType.Int64);

            //    var sql = @"
            //        SELECT Sum(CompletionAmount) FROM Commitment
            //        WHERE ActualEndDate is null 
            //        and EmployerAccountId = @employerAccountId
            //        and PlannedEndDate < 
            //         (
            //         SELECT top 1 datefromparts(Year, Month, 1) as d FROM AccountProjection
            //         WHERE EmployerAccountId = @employerAccountId
            //         order by d ASC)
            //        ";

            //    var result = await cnn.QueryAsync<decimal?>(
            //        sql,
            //        parameters,
            //        commandType: CommandType.Text);
            //    return result?.FirstOrDefault() ?? 0;
            //});
            //return await Task.FromResult(0.0);
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