using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class BalanceViewModel
    {
        public IEnumerable<BalanceItemViewModel> BalanceItemViewModels { get; set; }

        public string BackLink { get; set; }

        public string HashedAccountId { get; set; }

        public string BalanceStringArray { get; set; }
        public string DatesStringArray { get; set; }
        public decimal OverdueCompletionPayments { get; set; }
        public bool DisplayCoInvestment { get; set; }
    }
}