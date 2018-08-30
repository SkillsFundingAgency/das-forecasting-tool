using System;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class AccountFundsItem
    {
        public DateTime Date { get; set; }
        public decimal ActualCost { get; set; }
        public decimal EstimatedCost { get; set; }
        public decimal Balance { get; set; }
        public string FormattedBalance { get; set; }
    }
}