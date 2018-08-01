using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class ProjectionViewModel
    {
        public IEnumerable<ProjectiontemViewModel> BalanceItemViewModels { get; set; }

        public IDictionary<FinancialYear, ReadOnlyCollection<ProjectiontemViewModel>> ProjectionTables { get; set; }

        public string BackLink { get; set; }

        public string HashedAccountId { get; set; }

        public string BalanceStringArray { get; set; }
        public string DatesStringArray { get; set; }
        public decimal CurrentBalance { get; internal set; }
        public decimal OverdueCompletionPayments { get; set; }
        public bool DisplayCoInvestment { get; set; }
    }
}