using System;
using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Messages.Projections
{
    public class AccountProjectionCreatedEvent
    {
        public long EmployerAccountId { get; }
        public IEnumerable<AccountProjectionModel> ProjectionModels { get; set; }

        public AccountProjectionCreatedEvent(long employerAccountId)
        {
            EmployerAccountId = employerAccountId;
        }
    }
}