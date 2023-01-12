using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Web.Extensions;
using SFA.DAS.Forecasting.Domain.Projections.Services;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.HashingService;
using System.Collections.ObjectModel;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Domain.Commitments.Services;
using SFA.DAS.Forecasting.Web.ViewModels.EqualComparer;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Web.Orchestrators
{
    public interface IForecastingOrchestrator
    {
        Task<ProjectionViewModel> Projection(string hashedAccountId);
        Task<IEnumerable<BalanceCsvItemViewModel>> BalanceCsv(string hashedAccountId);
        Task<IEnumerable<ApprenticeshipCsvItemViewModel>> ApprenticeshipsCsv(string hashedAccountId);
    }

    public class ForecastingOrchestrator : IForecastingOrchestrator
    {
        private readonly IHashingService _hashingService;
        private readonly IAccountProjectionDataSession _accountProjection;
        private readonly ICurrentBalanceRepository _balanceRepository;
        private readonly ForecastingConfiguration _applicationConfiguration;
        private readonly IForecastingMapper _mapper;
        private readonly ICommitmentsDataService _commitmentsDataService;

        private static readonly DateTime BalanceMaxDate = DateTime.Parse("2019-05-01");

        public ForecastingOrchestrator(
            IHashingService hashingService,
            IAccountProjectionDataSession accountProjection,
            ICurrentBalanceRepository balanceRepository,
            ForecastingConfiguration applicationConfiguration,
            IForecastingMapper mapper,
            ICommitmentsDataService commitmentsDataService
            )
        {
            _hashingService = hashingService ?? throw new ArgumentNullException(nameof(hashingService));
            _accountProjection = accountProjection ?? throw new ArgumentNullException(nameof(accountProjection));
            _balanceRepository = balanceRepository;
            _applicationConfiguration = applicationConfiguration ?? throw new ArgumentNullException(nameof(applicationConfiguration));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _commitmentsDataService = commitmentsDataService ?? throw new ArgumentNullException(nameof(commitmentsDataService));
        }

        public async Task<ProjectionViewModel> Projection(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            var balance = await GetBalance(hashedAccountId);
            var accountProjection = await ReadProjection(accountId);

            return new ProjectionViewModel {
                BalanceItemViewModels = accountProjection.Projections,
                ProjectionTables = CreateProjectionTable(accountProjection.Projections),
                BackLink = _applicationConfiguration.BackLink,
                HashedAccountId = hashedAccountId,
                BalanceStringArray = string.Join(",", accountProjection.Projections.Select(m => m.Balance.ToString())),
                DatesStringArray = string.Join(",", accountProjection.Projections.Select(m => m.Date.ToString("yyyy-MM-dd"))),
                CurrentBalance = balance.Amount,
                OverdueCompletionPayments = balance.UnallocatedCompletionPayments,
                DisplayCoInvestment = accountProjection.Projections.Any(m => m.CoInvestmentEmployer + m.CoInvestmentGovernment > 0),
                ProjectionDate = accountProjection.CreatedOn
            };
        }

        public async Task<IEnumerable<BalanceCsvItemViewModel>> BalanceCsv(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            return (await ReadProjection(accountId))
                .Projections
                .Select(m => _mapper.ToCsvBalance(m));
        }

        public async Task<IEnumerable<ApprenticeshipCsvItemViewModel>> ApprenticeshipsCsv(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var commitments = await _commitmentsDataService.GetCurrentCommitments(accountId, _applicationConfiguration.LimitForecast ? BalanceMaxDate : (DateTime?)null);
            var csvCommitments = new List<CommitmentModel>();
            csvCommitments = csvCommitments.Concat(commitments.LevyFundedCommitments)
                .Concat(commitments.ReceivingEmployerTransferCommitments)
                .Concat(commitments.SendingEmployerTransferCommitments).OrderBy(c=>c.StartDate).ToList();

            return csvCommitments
                .Select(m => _mapper.ToCsvApprenticeship(m, accountId));
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

        private async Task<CurrentBalance> GetBalance(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var currentBalance = await _balanceRepository.Get(accountId);
            if(currentBalance == null)
                await currentBalance.RefreshBalance();
            await _balanceRepository.Store(currentBalance);

            return currentBalance;
        }

        private async Task<ProjectionModel> ReadProjection(long accountId)
        {
            var result = await _accountProjection.Get(accountId);

            var date = result.FirstOrDefault()?.ProjectionCreationDate;

            var d = _mapper.MapProjections(result);

            var projections = d.Where(m => m.Date.IsAfterOrSameMonth(DateTime.Today.AddMonths(1)) && (!_applicationConfiguration.LimitForecast || m.Date < BalanceMaxDate))
                .OrderBy(m => m.Date)
                .Take(48)
                .ToList();

            return new ProjectionModel
            {
                CreatedOn = date,
                Projections = projections

            };
        }

        private struct ProjectionModel
        {
            public List<ProjectiontemViewModel> Projections { get; set; }
            public DateTime? CreatedOn { get; set; }
        }
    }
}