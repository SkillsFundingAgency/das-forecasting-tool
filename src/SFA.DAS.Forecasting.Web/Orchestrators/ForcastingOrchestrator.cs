using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Projections.Services;
using SFA.DAS.Forecasting.Domain.Commitments.Services;
using SFA.DAS.Forecasting.Web.Extensions;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Web.Orchestrators
{
    public class ForecastingOrchestrator
    {
        private readonly IHashingService _hashingService;
        private readonly IAccountProjectionReadModelDataService _accountProjection;
        private readonly ICommitmentsDataService _commitmentsDataService;
        private readonly IApplicationConfiguration _applicationConfiguration;
        private readonly Mapper _mapper;
        private readonly ILog _logger;
        private static readonly DateTime BalanceMaxDate = DateTime.Parse("2019-05-01");

        public ForecastingOrchestrator(
            IHashingService hashingService,
            IAccountProjectionReadModelDataService accountProjection,
            ICommitmentsDataService commitmentsDataService,
            IApplicationConfiguration applicationConfiguration,
            Mapper mapper,
            ILog logger)
        {
            _hashingService = hashingService;
            _accountProjection = accountProjection;
            _commitmentsDataService = commitmentsDataService;
            _applicationConfiguration = applicationConfiguration;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BalanceViewModel> Balance(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            var balance = await GetBalance(hashedAccountId);

            return new BalanceViewModel {
                BalanceItemViewModels = balance,
                BackLink = _applicationConfiguration.BackLink,
                HashedAccountId = hashedAccountId,
                BalanceStringArray = string.Join(",", balance.Select(m => m.Balance.ToString())),
                DatesStringArray = string.Join(",", balance.Select(m => m.Date.ToString("yyyy-MM-dd"))),
                PendingCompletionPayments = await _commitmentsDataService.GetPendingCompletionPayments(accountId)
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