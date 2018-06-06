using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.Web.Orchestrators.Estimations
{
    public class EstimationOrchestrator : IEstimationOrchestrator
    {
        private readonly IAccountEstimationProjectionRepository _estimationProjectionRepository;
        private readonly IAccountEstimationRepository _estimationRepository;
        private readonly IHashingService _hashingService;
        private readonly ICurrentBalanceRepository _currentBalanceRepository;

        public EstimationOrchestrator(IAccountEstimationProjectionRepository estimationProjectionRepository,
            IAccountEstimationRepository estimationRepository,
            IHashingService hashingService, 
            ICurrentBalanceRepository currentBalanceRepository
            )
        {
            _estimationProjectionRepository = estimationProjectionRepository ?? throw new ArgumentNullException(nameof(estimationProjectionRepository));
            _estimationRepository = estimationRepository ?? throw new ArgumentNullException(nameof(estimationRepository));
            _hashingService = hashingService ?? throw new ArgumentNullException(nameof(hashingService));
            _currentBalanceRepository = currentBalanceRepository ?? throw new ArgumentNullException(nameof(currentBalanceRepository));
        }

        public async Task<EstimationPageViewModel> CostEstimation(string hashedAccountId, string estimateName, bool? apprenticeshipRemoved)
        {
            var accountId = GetAccountId(hashedAccountId);
            await RefreshCurrentBalance(accountId);
            var accountEstimation = await _estimationRepository.Get(accountId);
            var estimationProjector = await _estimationProjectionRepository.Get(accountEstimation);
            estimationProjector.BuildProjections();

            var estimationProjections =  estimationProjector.Projections;

            var viewModel = new EstimationPageViewModel
            {
                HashedAccountId = hashedAccountId,
                EstimationName = accountEstimation == null ? estimateName : accountEstimation.Name,
                ApprenticeshipRemoved = apprenticeshipRemoved.GetValueOrDefault(),
                Apprenticeships = new EstimationApprenticeshipsViewModel
                {
                    VirtualApprenticeships = accountEstimation?.VirtualApprenticeships?.Select(o =>
                        new EstimationApprenticeshipViewModel
                        {
                            Id = o.Id,
                            CompletionPayment = o.TotalCompletionAmount,
                            ApprenticesCount = o.ApprenticesCount,
                            CourseTitle = o.CourseTitle,
                            Level = o.Level,
                            MonthlyPayment = o.TotalInstallmentAmount,
                            MonthlyPaymentCount = o.TotalInstallments,
                            StartDate = o.StartDate,
                            TotalCost = o.TotalCost,
                            FundingSource = o.FundingSource
                        }),
                },
                TransferAllowances = estimationProjector?.Projections?
                    .Select(o => new EstimationTransferAllowanceVewModel
                    {
                        Date = new DateTime(o.Year, o.Month, 1),
                        ActualCost = o.ActualCosts.TransferFundsOut,
                        EstimatedCost = o.ModelledCosts.FundsOut,
                        RemainingAllowance = o.FutureFunds
                    }).ToList(),
                AccountFunds =
                    new AccountFundsViewModel
                    {
                        OpeningBalance = GetOpeningBalance(estimationProjector.Projections),
                        Records = GetAccountFunds(estimationProjections)
                    }
            };
            return viewModel;
        }

        private decimal GetOpeningBalance(ReadOnlyCollection<AccountEstimationProjectionModel> projections)
        {
            var first = projections.FirstOrDefault();
            if (first == null)
                return 0;

            return first.FutureFunds;
        }

        public async Task<bool> HasValidApprenticeships(string hashedAccountId)
        {
            var accountEstimation = await _estimationRepository.Get(GetAccountId(hashedAccountId));
            return accountEstimation.HasValidApprenticeships;
        }


        public async Task RefreshCurrentBalance(long accountId)
        {
            var currentBalance = await _currentBalanceRepository.Get(accountId);
            if (!await currentBalance.RefreshBalance())
                return;
            await _currentBalanceRepository.Store(currentBalance);
        }
        private long GetAccountId(string hashedAccountId) => _hashingService.DecodeValue(hashedAccountId);

        private IReadOnlyList<AccountFunds> GetAccountFunds(ReadOnlyCollection<AccountEstimationProjectionModel> estimations)
        {
            decimal estimatedFundsOut = 0;
            var accountFumds = estimations.Select(projection =>
            {
                var currentMonth = projection.Month == DateTime.Today.Month && projection.Year == DateTime.Today.Year;
                estimatedFundsOut += projection.ModelledCosts.FundsOut;
                var balance = projection.FutureFunds - estimatedFundsOut;

                return new AccountFunds
                {
                    Date = new DateTime(projection.Year, projection.Month, 1),
                    ActualCost = currentMonth ? 0 : projection.ActualCosts.FundsOut,
                    EstimatedCost = projection.ModelledCosts.FundsOut,
                    Balance = balance
                };
            });

            return accountFumds.ToList();
        }
    }

    public class AccountFunds
    {
        public DateTime Date { get; set; }
        public decimal ActualCost { get; set; }
        public decimal EstimatedCost { get; set; }
        public decimal Balance { get; set; }
    }
}