using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Web.Extensions;
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
        private readonly IApprenticeshipCourseDataService _apprenticeshipCourseService;

        public EstimationOrchestrator(IAccountEstimationProjectionRepository estimationProjectionRepository,
            IAccountEstimationRepository estimationRepository,
            IHashingService hashingService, 
            ICurrentBalanceRepository currentBalanceRepository,
            IApprenticeshipCourseDataService apprenticeshipCourseService)
        {
            _estimationProjectionRepository = estimationProjectionRepository ?? throw new ArgumentNullException(nameof(estimationProjectionRepository));
            _estimationRepository = estimationRepository ?? throw new ArgumentNullException(nameof(estimationRepository));
            _hashingService = hashingService ?? throw new ArgumentNullException(nameof(hashingService));
            _currentBalanceRepository = currentBalanceRepository ?? throw new ArgumentNullException(nameof(currentBalanceRepository));
            _apprenticeshipCourseService = apprenticeshipCourseService;
        }

        public async Task<EstimationPageViewModel> CostEstimation(string hashedAccountId, string estimateName, bool? apprenticeshipRemoved)
        {
            var accountId = GetAccountId(hashedAccountId);
            await RefreshCurrentBalance(accountId);
            var accountEstimation = await _estimationRepository.Get(accountId);
            var estimationProjector = await _estimationProjectionRepository.Get(accountEstimation);
            estimationProjector.BuildProjections();

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
                }).ToList()
            };
            return viewModel;
        }

        private long GetAccountId(string hashedAccountId) => _hashingService.DecodeValue(hashedAccountId);

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

        public async Task<EditApprenticeshipsViewModel> EditApprenticeshipModel(string hashedAccountId, string apprenticeshipsId, string estimationName)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var estimations = await _estimationRepository.Get(accountId);

            var model = estimations.FindVirtualApprenticeship(apprenticeshipsId);

            var course = await _apprenticeshipCourseService.GetApprenticeshipCourse(model.CourseId);

            var fundingPeriods = course.FundingPeriods.Select(m =>
                        new FundingPeriodViewModel
                        {
                            FromDate = m.EffectiveFrom,
                            ToDate = m.EffectiveTo,
                            FundingCap = m.FundingCap
                        });

            return new EditApprenticeshipsViewModel
            {
                CourseId = course.Id,
                CourseTitle = model.CourseTitle,
                ApprenticeshipsId = apprenticeshipsId,
                EstimationName = estimationName,
                Level = model.Level,
                NumberOfApprentices = model.ApprenticesCount,
                TotalInstallments = model.TotalInstallments,
                TotalCostAsString = model.TotalCost.FormatValue(),
                StartDateMonth = model.StartDate.Month,
                StartDateYear = model.StartDate.Year,
                HashedAccountId = hashedAccountId,
                FundingPeriodsJson = JsonConvert.SerializeObject(fundingPeriods),
            };
        }

        public async Task UpdateApprenticeshipModel(EditApprenticeshipsViewModel model)
        {
            var accountId = _hashingService.DecodeValue(model.HashedAccountId);
            var estimations = await _estimationRepository.Get(accountId);

            estimations.UpdateApprenticeship(model.ApprenticeshipsId, model.StartDateMonth, model.StartDateYear, model.NumberOfApprentices, model.TotalInstallments, model.TotalCostAsString.ToDecimal());
            await _estimationRepository.Store(estimations);
        }
    }
}