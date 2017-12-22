using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Forecasting.Application.Queries.Apprenticeships;
using SFA.DAS.Forecasting.Application.Queries.Balance;
using SFA.DAS.Forecasting.Domain.Entities;
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

        public ForecastingOrchestrator(IMediator mediator, IHashingService hashingService, ILog logger)
        {
            _mediator = mediator;
            _hashingService = hashingService;
            _logger = logger;
        }

        public async Task<BalanceViewModel> Balance(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            
            var result = await _mediator.Send(new EmployerBalanceRequest {EmployerAccountId = accountId });
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

        public async Task<ApprenticeshipPageViewModel> Apprenticeships(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            var result = await _mediator.Send(new ApprenticeshipsRequest { EmployerAccountId = accountId });

            return new ApprenticeshipPageViewModel
                       {
                           Apprenticeships =
                               result.Data.Select(
                                   m =>
                                   new ApprenticeshipViewModel
                                       {
                                           Name =
                                               $"{m.FirstName} {m.LastName}",
                                           StartDate = m.StartDate,
                                           MonthlyPayment =
                                               m.MonthlyPayment,
                                           TotalInstallments =
                                               m.TotalInstallments,
                                           CompletionPayment =
                                               m.CompletionPayment
                                       })
                       };
        }
    }
}