using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Events.Messages.PreLoad
{
    public class PreLoadPaymentRequest
    {
        public IEnumerable<string> EmployerAccountIds { get; set; }

        public int PeriodYear { get; set; }

        public int PeriodMonth { get; set; }

        public string PeriodId { get; set; }
    }
}