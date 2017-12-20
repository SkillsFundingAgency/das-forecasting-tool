using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

using SFA.DAS.EmployerCommitments.Domain.Entities;

namespace SFA.DAS.Forcasting.Application.Balance
{
    public class EmployerBalanceHandler : IRequestHandler<EmployerBalanceRequest, EmployerBalanceResponse>
    {
        public Task<EmployerBalanceResponse> Handle(EmployerBalanceRequest request, CancellationToken cancellationToken)
        {
            
            return Task.FromResult(new EmployerBalanceResponse
                                       {
                                           Data = GetData()
                                       });
        }

        private IEnumerable<BalanceItem> GetData()
        {
            for (int i = 0; i < 20; i++)
            {
                var m = DateTime.Now.AddMonths(i);
                yield return new BalanceItem
                {
                    Date = m,
                    LevyCredit = 700,
                    CostOfTraining = 100,
                    CompletionPayments = 0,
                    ExpiredFunds = 0,
                    Balance = 700 * (i + 1)
                };
            }
        }
    }
}