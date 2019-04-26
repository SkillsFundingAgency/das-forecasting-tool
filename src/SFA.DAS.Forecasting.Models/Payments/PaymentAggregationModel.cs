using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Models.Payments
{
    public class PaymentAggregationModel
    {
        public long Id { get; set; }
        public long EmployerAccountId { get; set; }
        public CalendarPeriod CollectionPeriod { get; set; }
        public decimal Amount { get; set; }
    }
}
