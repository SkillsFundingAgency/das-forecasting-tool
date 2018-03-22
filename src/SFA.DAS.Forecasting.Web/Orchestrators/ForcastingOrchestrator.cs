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

        public async Task<ProjectionsViewModel> Projections(string hashedAccountId)
        {
            var projections = await GetProjections(hashedAccountId);
            return new ProjectionsViewModel {
                ProjectionsItemViewModels = projections,
                BackLink = _applicationConfiguration.BackLink,
                HashedAccountId = hashedAccountId,
                BalanceStringArray = string.Join(",", projections.Select(m => m.Balance.ToString())),
                DatesStringArray = string.Join(",", projections.Select(m => m.Date.ToString("yyyy-MM-dd")))
            };
        }

        public async Task<IEnumerable<ProjectionsCsvItemViewModel>> ProjectionsCsv(string hashedAccountId)
        {
            return (await GetProjections(hashedAccountId))
                .Select(m => _mapper.ToCsvProjections(m));
        }

        private async Task<List<ProjectionsItemViewModel>> GetProjections(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var result = await _accountProjection.Get(accountId);
            return _mapper.MapProjections(result)
                .Where(m => !_applicationConfiguration.LimitForecast || m.Date < BalanceMaxDate)
                .ToList();
        }
    }
}