using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class BalanceViewModel
    {
        public IEnumerable<BalanceItemViewModel> BalanceItemViewModels { get; set; }

        public string BackLink { get; set; }
    }
}