using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Forecasting.Application.Queries.Apprenticeships;
using SFA.DAS.Forecasting.Application.Queries.Balance;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Web.Orchestrators
{
    public class ForecastingOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IHashingService _hashingService;
        private readonly ILog _logger;

        private readonly Mapper _mapper;

        public ForecastingOrchestrator(
            IMediator mediator, 
            IHashingService hashingService, 
            ILog logger,
            Mapper mapper)
        {
            _mediator = mediator;
            _hashingService = hashingService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<BalanceViewModel> Balance(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            
            var result = await _mediator.Send(new EmployerBalanceRequest {EmployerAccountId = accountId });
            return new BalanceViewModel { BalanceItemViewModels = _mapper.MapBalance(result.Data) };
        }

        public async Task<ApprenticeshipPageViewModel> Apprenticeships(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            var result = await _mediator.Send(new ApprenticeshipsRequest { EmployerAccountId = accountId });

            return new ApprenticeshipPageViewModel
            {
                Apprenticeships = result.Data.Select(_mapper.MapApprenticeship)
            };
        }

        public async Task<VisualisationViewModel> Visualisation(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            var result = await _mediator.Send(new EmployerBalanceRequest { EmployerAccountId = accountId });
            
            var viewModel = new VisualisationViewModel
            {
                ChartTitle = "Your 4 Year Forecast",
                ChartItems = result.Data.Select(m => new ChartItemViewModel { BalanceMonth = m.Date, Amount = m.Balance })
            };

            return viewModel;
        }
    }
}