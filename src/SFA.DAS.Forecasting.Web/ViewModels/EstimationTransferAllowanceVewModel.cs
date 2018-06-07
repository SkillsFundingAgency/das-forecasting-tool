
using System;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class EstimationTransferAllowanceVewModel
    {
        public DateTime Date { get; set; }
        public decimal RemainingAllowance { get; set; }
        public decimal EstimatedCost { get; set; }
        public decimal ActualCost { get; set; }
        public bool IsLessThanCost => RemainingAllowance < 0;
    }
}