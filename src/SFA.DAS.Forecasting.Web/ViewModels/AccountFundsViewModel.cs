﻿using SFA.DAS.Forecasting.Web.Orchestrators.Estimations;
using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class AccountFundsViewModel
    {
        public IReadOnlyList<AccountFundsItem> Records { get; internal set; }
        public decimal MonthlyInstallmentAmount { get; set; }
    }
}