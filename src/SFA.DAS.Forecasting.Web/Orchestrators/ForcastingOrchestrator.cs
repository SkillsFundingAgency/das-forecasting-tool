using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Web.Extensions;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.Forecasting.Domain.Projections.Services;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.HashingService;
using System.Collections.ObjectModel;
using SFA.DAS.Forecasting.Web.ViewModels.EqualComparer;

namespace SFA.DAS.Forecasting.Web.Orchestrators
{
    public class ForecastingOrchestrator
    {
        private readonly IHashingService _hashingService;
        private readonly IAccountProjectionDataSession _accountProjection;
        private readonly IBalanceDataService _balanceDataService;
        private readonly IApplicationConfiguration _applicationConfiguration;
        private readonly IForecastingMapper _mapper;
        private readonly IAccountProjectionDataSession _accountProjectionDataSession;

        private static readonly DateTime BalanceMaxDate = DateTime.Parse("2019-05-01");

        public ForecastingOrchestrator(
            IHashingService hashingService,
            IAccountProjectionDataSession accountProjection,
            IBalanceDataService balanceDataService,
            IApplicationConfiguration applicationConfiguration,
            IForecastingMapper mapper,
            IAccountProjectionDataSession accountProjectionDataSession
            )
        {
            _hashingService = hashingService ?? throw new ArgumentNullException(nameof(hashingService));
            _accountProjection = accountProjection ?? throw new ArgumentNullException(nameof(accountProjection));
            _balanceDataService = balanceDataService ?? throw new ArgumentNullException(nameof(balanceDataService));
            _applicationConfiguration = applicationConfiguration ?? throw new ArgumentNullException(nameof(applicationConfiguration));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _accountProjectionDataSession = accountProjectionDataSession ?? throw new ArgumentNullException(nameof(accountProjectionDataSession));
        }

        public async Task<BalanceViewModel> Balance(string hashedAccountId)
        {
            var accountProjection = await GetAccountProjection(hashedAccountId);

            var balance = await GetBalance(hashedAccountId);
            var currentBalance = balance.Item1;
            var overdueCompletionPayments = balance.Item2;

            return new BalanceViewModel {
                BalanceItemViewModels = accountProjection,
                ProjectionTables = CreateProjectionTable(accountProjection),
                BackLink = _applicationConfiguration.BackLink,
                HashedAccountId = hashedAccountId,
                BalanceStringArray = string.Join(",", accountProjection.Select(m => m.Balance.ToString())),
                DatesStringArray = string.Join(",", accountProjection.Select(m => m.Date.ToString("yyyy-MM-dd"))),
                CurrentBalance = currentBalance,
                OverdueCompletionPayments = overdueCompletionPayments,
                DisplayCoInvestment = accountProjection.Any(m => m.CoInvestmentEmployer + m.CoInvestmentGovernment > 0)
            };
        }

        public async Task<IEnumerable<BalanceCsvItemViewModel>> BalanceCsv(string hashedAccountId)
        {
            return (await GetAccountProjection(hashedAccountId))
                .Select(m => _mapper.ToCsvBalance(m));
        }

        public async Task<IEnumerable<ApprenticeshipCsvItemViewModel>> ApprenticeshipsCsv(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var commitments = await _accountProjectionDataSession.GetCommitments(accountId, _applicationConfiguration.LimitForecast ? BalanceMaxDate : (DateTime?)null);
            return commitments
                .Select(m => _mapper.ToCsvApprenticeship(m, accountId));
        }

        private async Task<List<ProjectiontemViewModel>> GetAccountProjection(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var result = await _accountProjection.Get(accountId);
            var d = _mapper.MapProjections(result);

            return d.Where(m => m.Date.IsAfterOrSameMonth(DateTime.Today) && (!_applicationConfiguration.LimitForecast || m.Date < BalanceMaxDate))
                .OrderBy(m => m.Date)
                .Take(48)
                .ToList();
        }

        private IDictionary<FinancialYear, ReadOnlyCollection<ProjectiontemViewModel>> CreateProjectionTable(List<ProjectiontemViewModel> accountProjection)
        {
            var dict = accountProjection.Select(m =>
                {
                    return Tuple.Create(new FinancialYear(m.Date), m);
                }).GroupBy(m => m.Item1, new FinancialYearIEqualityComparer())
                .ToDictionary(
                    m => m.Key,
                    v => v.Select(t => t.Item2).ToList().AsReadOnly());

            foreach(var s in dict )
            {
                s.Key.FirstStartDate = s.Value.OrderBy(m => m.Date).First().Date;
                s.Key.LastEndDate = s.Value.OrderBy(m => m.Date).Last().Date;
            };

            return dict;
        }

        private async Task<Tuple<decimal, decimal>> GetBalance(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var balance = await _balanceDataService.Get(accountId);

            return Tuple.Create(balance.Amount, balance.UnallocatedCompletionPayments);
        }
    }
}