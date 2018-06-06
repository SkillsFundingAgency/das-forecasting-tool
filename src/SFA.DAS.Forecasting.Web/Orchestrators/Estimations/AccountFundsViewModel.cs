using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Web.Orchestrators.Estimations
{
    public class AccountFundsViewModel
    {
        public decimal OpeningBalance { get; set; }
        public IReadOnlyList<AccountFunds> Records { get; internal set; }
    }
}