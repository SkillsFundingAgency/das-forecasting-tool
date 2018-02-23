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
    public class ForecastingOrchestrator
    {
        private readonly IHashingService _hashingService;
        private readonly IAccountProjectionReadModelDataService _accountProjection;
        private readonly IApplicationConfiguration _applicationConfiguration;
        private readonly Mapper _mapper;


        // ToDo: Move to config
        private static readonly DateTime BalanceMaxDate = DateTime.Parse("2019-05-01");

        public ForecastingOrchestrator(
            IHashingService hashingService,
            IAccountProjectionReadModelDataService accountProjection,
            IApplicationConfiguration applicationConfiguration,
            Mapper mapper)
        {
            _hashingService = hashingService;
            _accountProjection = accountProjection;
            _applicationConfiguration = applicationConfiguration;
            _mapper = mapper;
        }

        public async Task<BalanceViewModel> Balance(string hashedAccountId)
        {
            // BalanceItemViewModels.Select(m => m.Date.ToString("yyyy-MM-dd"))).Replace("\"", "")
            var balance = await GetBalance(hashedAccountId);
            return new BalanceViewModel {
                BalanceItemViewModels = balance,
                BackLink = _applicationConfiguration.BackLink,
                HashedAccountId = hashedAccountId,
                BalanceStringArray = MakeBalanceStringArray(balance),
                DatesStringArray = MakeDateStringArray(balance)
            };
        }

        private string MakeDateStringArray(IEnumerable<BalanceItemViewModel> balance)
        {
            var bb = balance.Select(m => m.Date.ToString("yyyy-MM-dd"));
            return string.Join(",", bb);
        }

        private string MakeBalanceStringArray(IEnumerable<BalanceItemViewModel> balance)
        {
            var bb = balance.Select(m => m.Balance.ToString());
            return string.Join(",", bb);
        }

        private async Task<IEnumerable<BalanceItemViewModel>> GetBalance(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var result = await _accountProjection.Get(accountId);
            return _mapper.MapBalance(result)
                .Where(m => !_applicationConfiguration.LimitForecast || m.Date < BalanceMaxDate);
        }

        public async Task<IEnumerable<BalanceCsvItemViewModel>> BalanceCsv(string hashedAccountId)
        {
            return (await GetBalance(hashedAccountId))
                .Select(m => _mapper.ToCsvBalance(m));
        }

        public async Task<VisualisationViewModel> Visualisation(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            var result = await _accountProjection.Get(accountId);
            
            var viewModel = new VisualisationViewModel
            {
                ChartTitle = "Your 4 Year Forecast",
                ChartItems = result.Select(m => new ChartItemViewModel { BalanceMonth = new DateTime(m.Year, m.Month, 1), Amount = m.FutureFunds })
            };

            return viewModel;
        }
    }
}