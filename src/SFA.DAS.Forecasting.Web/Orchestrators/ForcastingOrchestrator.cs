using System;
using System.Collections.Generic;

using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.Orchestrators
{
    public class ForcastingOrchestrator
    {
        public BalanceViewModel Balance(string hashedAccountId)
        {
            return new BalanceViewModel { BalanceItemViewModels = GetBalanceData() };
        }

        private IEnumerable<BalanceItemViewModel> GetBalanceData()
        {
            for (int i = 0; i < 20; i++)
            {
                var m = DateTime.Now.AddMonths(i);
                yield return new BalanceItemViewModel
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