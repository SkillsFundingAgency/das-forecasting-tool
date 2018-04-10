
using System;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class EstimationTransferAllowanceVewModel
    {
        public DateTime Date { get; set; }
        public decimal RemainingAllowance { get; set; }
        public decimal Cost { get; set; }
        public bool IsLessThanCost => Cost > RemainingAllowance;
    }
}