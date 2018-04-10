using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Estimations.Service;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;
using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.Orchestrators
{
    public class EstimationOrchestrator : IEstimationOrchestrator
    {
        private readonly IAccountEstimationBuilderService _accountEstimationBuilder;

        public EstimationOrchestrator(IAccountEstimationBuilderService accountEstimationBuilder)
        {
            _accountEstimationBuilder = accountEstimationBuilder;
        }

        public async Task<EstimationPageViewModel> CostEstimation(string hashedAccountId, string estimateName)
        {
            var viewModel = new EstimationPageViewModel();
            var accountEstimations = await _accountEstimationBuilder.CostBuildEstimations(hashedAccountId, estimateName);
            if (accountEstimations != null)
            {
                viewModel = new EstimationPageViewModel
                {
                    Apprenticeships = new EstimationApprenticeshipsViewModel
                    {
                        VirtualApprenticeships = accountEstimations.Apprenticeships?.Select(o => new EstimationApprenticeshipViewModel
                        {
                            Id = o.Id,
                            CompletionPayment = o.CompletionPayment,
                            ApprenticesCount = o.ApprenticesCount,
                            CourseTitle = o.CourseTitle,
                            Level = o.Level,
                            MonthlyPayment = o.MonthlyPayment,
                            MonthlyPaymentCount = o.TotalInstallments,
                            StartDate = o.StartDate
                        }),
                    },
                    EstimationName = accountEstimations.EstimationName,
                    TransferAllowances = accountEstimations.Estimations?.Select(o => new EstimationTransferAllowanceVewModel
                    {
                        Date = new DateTime(o.Year, o.Month, 1),
                        Cost = o.TotalCostOfTraining,
                        RemainingAllowance = o.FutureFunds
                    })
                };
            }


            return viewModel;
        }


    }
}