using SFA.DAS.Forecasting.Models.Estimation;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class RemoveApprenticeshipViewModel
    {
        public string ApprenticeshipId { get; set; }
        public string EstimationName { get; set; }
        public string HashedAccountId { get; set; }
    }
}