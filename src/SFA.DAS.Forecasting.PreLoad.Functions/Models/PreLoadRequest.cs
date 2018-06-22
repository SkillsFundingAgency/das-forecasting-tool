using System.Collections.Generic;

namespace SFA.DAS.Forecasting.PreLoad.Functions.Models
{
    public class PreLoadRequest
    {
        public IEnumerable<string> EmployerAccountIds { get; set; }

        public string PeriodYear { get; set; }

        public short PeriodMonth { get; set; }

        public long? SubstitutionId { get; set; }
    }
}
