using System;
using System.Collections.Generic;
using SFA.DAS.Forecasting.Models.Payments;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Messages.Projections
{
    public class GenerateExpiringFundsCommand
    {
        public long EmployerAccountId { get; set; }
        public IList<AccountProjectionModel> AccountProjectionModels { get; set; }
    }
}