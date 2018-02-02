using System;
using System.Collections.Generic;
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
        private readonly IAccountProjectionDataService _accountProjectionRepository;
        private readonly IApplicationConfiguration _applicationConfiguration;
        private readonly ILog _logger;

        private readonly Mapper _mapper;

        // ToDo: Move to config
        private readonly static DateTime balanceMaxDate = DateTime.Parse("2019-05-01");

        public ForecastingOrchestrator(
            IHashingService hashingService,
            IAccountProjectionDataService accountProjectionRepository,
            IApplicationConfiguration applicationConfiguration,
            ILog logger,
            Mapper mapper)
        {
            _hashingService = hashingService;
            _accountProjectionRepository = accountProjectionRepository;
            _applicationConfiguration = applicationConfiguration;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<BalanceViewModel> Balance(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            var result = await _accountProjectionRepository.Get(accountId);
            
            return new BalanceViewModel {
                BalanceItemViewModels = _mapper.MapBalance(result)
                    .Where(m => m.Date < balanceMaxDate),
                BackLink = _applicationConfiguration.BackLink,
                HashedAccountId = hashedAccountId
            };
        }

        public async Task<IEnumerable<BalanceCsvItemViewModel>> BalanceCsv(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            var result = await _accountProjectionRepository.Get(accountId);

            return _mapper.MapBalance(result)
                .Where(m => m.Date < balanceMaxDate)
                .Select(m => _mapper.ToCsvBalance(m));

            
        }

        public async Task<VisualisationViewModel> Visualisation(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            var result = await _accountProjectionRepository.Get(accountId);
            
            var viewModel = new VisualisationViewModel
            {
                ChartTitle = "Your 4 Year Forecast",
                ChartItems = result.Select(m => new ChartItemViewModel { BalanceMonth = new DateTime(m.Year, m.Month, 1), Amount = m.FutureFunds })
            };

            return viewModel;
        }
    }
}