using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Web.ViewModels.EqualComparer
{
    public class FinancialYearIEqualityComparer : IEqualityComparer<FinancialYear>
    {
        public bool Equals(FinancialYear x, FinancialYear y)
        {
            return x.StartDate == y.StartDate && x.EndDate == y.EndDate;
        }

        public int GetHashCode(FinancialYear x)
        {
            return x.StartDate.ToShortDateString().GetHashCode() ^ x.EndDate.ToShortDateString().GetHashCode();
        }

    }
}