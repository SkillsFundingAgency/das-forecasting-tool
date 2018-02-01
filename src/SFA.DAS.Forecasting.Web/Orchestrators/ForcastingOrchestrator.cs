using System;
using System.Linq;
using System.Threading.Tasks;

using SFA.DAS.Forecasting.Domain.Interfaces;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Web.Orchestrators
{
    public class ForecastingOrchestrator
    {
        private readonly IHashingService _hashingService;
        private readonly IBalanceRepository _balanceRepository;
        private readonly IApprenticeshipRepository _apprenticeshipRepository;
        private readonly ILog _logger;

        private readonly Mapper _mapper;

        private readonly static DateTime balanceMaxDate = DateTime.Parse("2019-05-01");

        public ForecastingOrchestrator(
            IHashingService hashingService,
            IBalanceRepository balanceRepository,
            IApprenticeshipRepository apprenticeshipRepository,
            ILog logger,
            Mapper mapper)
        {
            _hashingService = hashingService;
            _balanceRepository = balanceRepository;
            _apprenticeshipRepository = apprenticeshipRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<BalanceViewModel> Balance(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            var result = await  _balanceRepository.GetBalanceAsync(accountId);
            
            return new BalanceViewModel { BalanceItemViewModels = _mapper.MapBalance(result).Where(m => m.Date < balanceMaxDate) };
        }

        public async Task<ApprenticeshipPageViewModel> Apprenticeships(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            var apprenticeships = await _apprenticeshipRepository.GetApprenticeships(accountId);

            return new ApprenticeshipPageViewModel
            {
                Apprenticeships = apprenticeships.Select(_mapper.MapApprenticeship)
            };
        }

        public async Task<VisualisationViewModel> Visualisation(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            var result = await _balanceRepository.GetBalanceAsync(accountId);
            
            var viewModel = new VisualisationViewModel
            {
                ChartTitle = "Your 4 Year Forecast",
                ChartItems = result.Select(m => new ChartItemViewModel { BalanceMonth = m.Date, Amount = m.Balance })
            };

            return viewModel;
        }
    }
}