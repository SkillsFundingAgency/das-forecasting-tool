using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.Web.Orchestrators
{
    public class EstimationOrchestrator : IEstimationOrchestrator
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

        public async Task<EstimationPageViewModel> CostEstimation(string hashedAccountId, string estimateName)
        {
            /// TODO Get Data From Service
            var startDate = new DateTime(2018, 5, 1);
            const string DATE_FORMAT = "MMM yyyy";

            var fakeApprenticeships = new List<EstimationApprenticeshipViewModel>
            {
                new EstimationApprenticeshipViewModel
                {
                    Id= "ABC001",
                    CourseTitle = "Construction Building: Wood Occupations",
                    Level = 2,
                    Count = 2,
                    StartDate = startDate.ToString(DATE_FORMAT),
                    MonthlyPayment = "£533.33",
                    MonthlyPaymentCount = 18,
                    ComplementionPayment = "£2,400.00"
                }
            };


            var fakeEstimateApprentiships = new EstimationApprenticeshipsViewModel
            {
                VirtualApprenticeships = fakeApprenticeships,
                TotalApprenticeshipCount = 2,
                TotalMonthlyPayment = "£533.33",
                TotalCompletionPayment = "£2,400.00"
            };



            var fakeEstimationTransferAllowance = new List<EstimationTransferAllowanceVewModel>
            {
                new EstimationTransferAllowanceVewModel
                {
                    Date = startDate.ToString(DATE_FORMAT),
                    RemainingAllowance = "£15,000.00",
                    Cost = "£0.00",
                },
                new EstimationTransferAllowanceVewModel
                {
                    Date = startDate.ToString(DATE_FORMAT),
                    RemainingAllowance = "£14,466.67",
                    Cost = "£533.33",
                    IsLessThanCost = true
                }
            };

           

            var result = new EstimationPageViewModel
            {
                Apprenticeships = fakeEstimateApprentiships,
                TransferAllowances = fakeEstimationTransferAllowance,
                CanFund = false
            };

            return await Task.FromResult(result);
        }

    }
}