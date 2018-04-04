using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Projections.Services;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.Web.Orchestrators
{
    public class EstimationOrchestrator
    {
        private readonly IHashingService _hashingService;
        private readonly Mapper _mapper;


        public EstimationOrchestrator(
            IHashingService hashingService,
            Mapper mapper)
        {
            _hashingService = hashingService;
            _mapper = mapper;
        }

        public async Task<EstimationPageViewModel> CostEstimations(string hashedAccountId)
        {
            var startDate = new DateTime(2018, 5, 0);
            const string DATE_FORMAT = "MMM yyyy";

            var fakeApprenticeships = new List<EstimationApprenticeshipViewModel>
            {
                new EstimationApprenticeshipViewModel
                {
                    Id= "ABC001",
                    ApprenticeshipSummary = "Construction Building: Wood Occupations 2",
                    Count = 2,
                    StartDate = startDate.ToString(DATE_FORMAT),
                    MonthlyPayment = "£533.33",
                    MonthlyPaymentCount = 18,
                    ComplementionPayment = "£2,400.00"
                }
            };

            var fakeEstimationTransferAllowance = new List<EstimationTransferAllowanceVewModel>();
            for (int i = -1; i < 12; i++)
            {
                fakeEstimationTransferAllowance.Add(new EstimationTransferAllowanceVewModel
                {
                    Date = startDate.ToString(DATE_FORMAT),

                    Cost = "£0.00",

                });

                startDate.AddMonths(i);
            }

            var result = new EstimationPageViewModel
            {
                Apprenticeships = fakeApprenticeships
            };

            return await Task.FromResult(result);
        }

    }
}