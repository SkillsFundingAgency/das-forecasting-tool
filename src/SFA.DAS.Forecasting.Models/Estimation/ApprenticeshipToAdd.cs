using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Models.Estimation
{
    public class ApprenticeshipToAdd
    {
            public int ApprenticesCount { get; set; }
            public int NumberOfMonths { get; set; }
            public int? StartMonth { get; set; }
            public int StartYear { get; set; }
            public decimal TotalCost { get; set; }
    }
}
