using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.Web.Orchestrators
{
    public class EstimationOrchestrator : IEstimationOrchestrator
    {
        private readonly IAccountEstimationProjectionRepository _estimationProjectionRepository;
        private readonly IAccountEstimationRepository _estimationRepository;
        private readonly IHashingService _hashingService;

        public EstimationOrchestrator(IAccountEstimationProjectionRepository estimationProjectionRepository,
            IAccountEstimationRepository estimationRepository,
            IHashingService hashingService)
        {
            _estimationProjectionRepository = estimationProjectionRepository ?? throw new ArgumentNullException(nameof(estimationProjectionRepository));
            _estimationRepository = estimationRepository ?? throw new ArgumentNullException(nameof(estimationRepository));
            _hashingService = hashingService ?? throw new ArgumentNullException(nameof(hashingService));
        }

        public async Task<EstimationPageViewModel> CostEstimation(string hashedAccountId, string estimateName, bool? apprenticeshipRemoved)
        {
            var accountEstimation = await GetEstimation(hashedAccountId);
            var estimationProjector = await _estimationProjectionRepository.Get(accountEstimation);
            estimationProjector.BuildProjections();
            var viewModel = new EstimationPageViewModel
            {
                Apprenticeships = new EstimationApprenticeshipsViewModel
                {
                    VirtualApprenticeships = accountEstimation.VirtualApprenticeships?.Select(o =>
                        new EstimationApprenticeshipViewModel
                        {
                            Id = o.Id,
                            CompletionPayment = o.TotalCompletionAmount,
                            ApprenticesCount = o.ApprenticesCount,
                            CourseTitle = o.CourseTitle,
                            Level = o.Level,
                            MonthlyPayment = o.TotalInstallmentAmount,
                            MonthlyPaymentCount = o.TotalInstallments,
                            StartDate = o.StartDate
                        }),
                },
                EstimationName = accountEstimation.Name,
                TransferAllowances = estimationProjector.Projections.Select(o => new EstimationTransferAllowanceVewModel
                {
                    Date = new DateTime(o.Year, o.Month, 1),
                    Cost = o.TotalCostOfTraining,
                    RemainingAllowance = o.FutureFunds
                }),
                ApprenticeshipRemoved = apprenticeshipRemoved.GetValueOrDefault(),
            };
            return viewModel;
        }

        public async Task<AccountEstimation> GetEstimation(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            return await _estimationRepository.Get(accountId);
        }
    }
}