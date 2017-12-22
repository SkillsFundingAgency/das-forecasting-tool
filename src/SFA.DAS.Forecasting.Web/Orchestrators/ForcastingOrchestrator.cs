using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Forecasting.Application.Queries.Balance;
using SFA.DAS.Forecasting.Domain.Entities;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Web.Orchestrators
{
    public class ForecastingOrchestrator
    {
        private readonly IMediator _mediator;

        private readonly ILog _logger;

        public ForecastingOrchestrator(IMediator mediator, ILog logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<BalanceViewModel> Balance(string hashedAccountId)
        {
            _logger.Info($"Hellow world {hashedAccountId}");
            var result = await _mediator.Send(new EmployerBalanceRequest {EmployerAccountId = 12345 });
            return new BalanceViewModel { BalanceItemViewModels = MapBalanceData(result.Data) };
        }

        private IEnumerable<BalanceItemViewModel> MapBalanceData(IEnumerable<BalanceItem> data)
        {
            return data.Select(x => 
                new BalanceItemViewModel
                {
                    Date = x.Date,
                    LevyCredit = x.LevyCredit,
                    CostOfTraining =  x.CostOfTraining,
                    CompletionPayments = x.CompletionPayments,
                    ExpiredFunds = x.ExpiredFunds,
                    Balance = x.Balance
                });

        }
    }
}