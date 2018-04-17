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
        public string CourseId { get; set; }
        public string CourseTitle { get; set; }
        public int Level { get; set; }
        public int ApprenticesCount { get; set; }
        public DateTime StartDate { get; set; }
        public short TotalInstallments { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalCompletionAmount { get; set; }
        public decimal TotalInstallmentAmount { get; set; }
        
    }
}
