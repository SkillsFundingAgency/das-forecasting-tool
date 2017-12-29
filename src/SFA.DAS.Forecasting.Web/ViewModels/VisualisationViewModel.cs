using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class VisualisationViewModel
    {
        public string ChartTitle { get; set; }
        public IEnumerable<ChartItemViewModel> ChartItems { get; set; }

    }
}