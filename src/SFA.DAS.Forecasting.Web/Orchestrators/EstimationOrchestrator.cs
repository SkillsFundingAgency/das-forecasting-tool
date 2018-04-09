using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Estimations.Service;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;
using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.Orchestrators
{
    public class EstimationOrchestrator : IEstimationOrchestrator
    {
        private readonly Mapper _mapper;
        private readonly IAccountEstimationBuilderService _accountEstimationBuilder;

        public EstimationOrchestrator(Mapper mapper, IAccountEstimationBuilderService accountEstimationBuilder)
        {
            _mapper = mapper;
            _accountEstimationBuilder = accountEstimationBuilder;
        }

        public async Task<EstimationPageViewModel> CostEstimation(string hashedAccountId, string estimateName)
        {
            var accountEstimations = await _accountEstimationBuilder.CostBuildEstimations(hashedAccountId, estimateName);



            return await Task.FromResult(new EstimationPageViewModel());

        }

    }
}