using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Forecasting.Domain.Interfaces;

namespace SFA.DAS.Forecasting.Application.Queries.Balance
{
    public class EmployerBalanceHandler : IRequestHandler<EmployerBalanceRequest, EmployerBalanceResponse>
    {
        private readonly IBalanceRepository _balanceRepository;

        public EmployerBalanceHandler(IBalanceRepository balanceRepository)
        {
            _balanceRepository = balanceRepository;
        }

        public async Task<EmployerBalanceResponse> Handle(EmployerBalanceRequest request, CancellationToken cancellationToken)
        {
            var data = await _balanceRepository.GetBalanceAsync(request.EmployerAccountId);

            return new EmployerBalanceResponse { Data =  data };
        }
    }
}