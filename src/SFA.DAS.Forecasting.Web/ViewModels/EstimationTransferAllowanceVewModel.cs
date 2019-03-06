using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class EstimationTransferAllowanceVewModel
    {
        public IEnumerable<EstimationTransferAllowance> Records { get; set; }

        public decimal AnnualTransferAllowance { get; set; }
    }
}