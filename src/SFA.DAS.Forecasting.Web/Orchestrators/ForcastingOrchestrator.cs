using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Domain.Commitments.Services;
using SFA.DAS.Forecasting.Web.Extensions;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.Forecasting.Domain.Projections.Services;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.Web.Orchestrators
{
    public class ForecastingOrchestrator
    {
        private readonly IHashingService _hashingService;
        private readonly IAccountProjectionDataSession _accountProjection;
        private readonly IBalanceDataService _balanceDataService;
        private readonly IApplicationConfiguration _applicationConfiguration;
        private readonly ForecastingMapper _mapper;
        private readonly ICommitmentsDataService _commitmentsDataService;

        private static readonly DateTime BalanceMaxDate = DateTime.Parse("2019-05-01");

        public ForecastingOrchestrator(
            IHashingService hashingService,
            IAccountProjectionDataSession accountProjection,
            IBalanceDataService balanceDataService,
            IApplicationConfiguration applicationConfiguration,
            ForecastingMapper mapper,
            ICommitmentsDataService commitmentsDataService)
        {
            _hashingService = hashingService;
            _accountProjection = accountProjection;
            _balanceDataService = balanceDataService;
            _applicationConfiguration = applicationConfiguration;
            _mapper = mapper;
            _commitmentsDataService = commitmentsDataService;
        }

        public async Task<BalanceViewModel> Balance(string hashedAccountId)
        {
            var accountProjection = await GetAccountProjection(hashedAccountId);

            var overdueCompletionPayments = await GetOverdueCompletionPayments(hashedAccountId);
            return new BalanceViewModel {
                BalanceItemViewModels = accountProjection,
                BackLink = _applicationConfiguration.BackLink,
                HashedAccountId = hashedAccountId,
                BalanceStringArray = string.Join(",", accountProjection.Select(m => m.Balance.ToString())),
                DatesStringArray = string.Join(",", accountProjection.Select(m => m.Date.ToString("yyyy-MM-dd"))),
                OverdueCompletionPayments = overdueCompletionPayments,
                DisplayCoInvestment = accountProjection.Any(m => m.CoInvestmentEmployer + m.CoInvestmentGovernment > 0)
            };
        }

        private async Task<decimal> GetOverdueCompletionPayments(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var balance = await _balanceDataService.Get(accountId);
            return balance.UnallocatedCompletionPayments;
        }

        public async Task<IEnumerable<BalanceCsvItemViewModel>> BalanceCsv(string hashedAccountId)
        {
            return (await GetAccountProjection(hashedAccountId))
                .Select(m => _mapper.ToCsvBalance(m));
        }

        public async Task<IEnumerable<ApprenticeshipCsvItemViewModel>> ApprenticeshipsCsv(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            var commitmentModels = await _commitmentsDataService.GetCurrentCommitments(accountId);

            return commitmentModels
                .Where(c => c.StartDate <= BalanceMaxDate && c.PlannedEndDate.IsAfterOrSameMonth(DateTime.Today))
                .Select(m => _mapper.ToCsvApprenticeship(m, accountId));
        }

        private async Task<List<ProjectiontemViewModel>> GetAccountProjection(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var result = await _accountProjection.Get(accountId);
            return _mapper.MapProjections(result)
                .Where(m => !_applicationConfiguration.LimitForecast || m.Date < BalanceMaxDate)
                .Where(m => m.Date.IsAfterOrSameMonth(DateTime.Today))
                .Take(48)
                .ToList();
        }

    }
}