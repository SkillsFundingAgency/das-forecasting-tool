using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class VisualisationViewModel
    {
        public string ChartTitle { get; set; }
        public List<ChartItemViewModel> ChartItems { get; set; }

    }
}