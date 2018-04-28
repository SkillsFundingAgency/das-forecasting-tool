using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Projections.Services;
using SFA.DAS.Forecasting.Domain.Projections.Services;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.Web.Orchestrators
{
    public class ForecastingOrchestrator
    {
        private readonly IHashingService _hashingService;
        private readonly IAccountProjectionDataSession _accountProjection;
        private readonly IApplicationConfiguration _applicationConfiguration;
        private readonly Mapper _mapper;

        private static readonly DateTime BalanceMaxDate = DateTime.Parse("2019-05-01");

        public ForecastingOrchestrator(
            IHashingService hashingService,
            IAccountProjectionDataSession accountProjection,
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
            var balance = await GetBalance(hashedAccountId);
            return new BalanceViewModel {
                BalanceItemViewModels = balance,
                BackLink = _applicationConfiguration.BackLink,
                HashedAccountId = hashedAccountId,
                BalanceStringArray = string.Join(",", balance.Select(m => m.Balance.ToString())),
                DatesStringArray = string.Join(",", balance.Select(m => m.Date.ToString("yyyy-MM-dd")))
            };
        }

        public async Task<IEnumerable<BalanceCsvItemViewModel>> BalanceCsv(string hashedAccountId)
        {
            return (await GetBalance(hashedAccountId))
                .Select(m => _mapper.ToCsvBalance(m));
        }

        private async Task<List<BalanceItemViewModel>> GetBalance(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var result = await _accountProjection.Get(accountId);
            return _mapper.MapBalance(result)
                .Where(m => !_applicationConfiguration.LimitForecast || m.Date < BalanceMaxDate)
                .ToList();
        }
    }
}