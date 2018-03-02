using System.Collections.Generic;

namespace SFA.DAS.Forecasting.PreLoad.Functions.Models
{
    internal class PreLoadRequest
    {
        public IEnumerable<string> EmployerAccountIds { get; set; }

        public string PeriodYear { get; set; }

        public short? PeriodMonth { get; set; }
    }
}
