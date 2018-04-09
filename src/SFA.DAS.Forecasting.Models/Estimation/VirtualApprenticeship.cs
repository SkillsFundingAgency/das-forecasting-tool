using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Models.Estimation
{
    public class VirtualApprenticeship
    {
        public string Id { get; set; }
        public long CourseId { get; set; }
        public string CourseTitle { get; set; }
        public int Level { get; set; }
        public int ApprenticesCount { get; set; }
        public DateTime StartDate { get; set; }
        public decimal MonthlyPayment { get; set; }
        public int TotalInstallments { get; set; }
        public decimal TotalCost { get; set; }

    }
}
