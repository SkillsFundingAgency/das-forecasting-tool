using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SFA.DAS.Forecasting.Domain.Levy.Validation;
using SFA.DAS.Forecasting.Domain.Shared;
using SFA.DAS.Forecasting.Models.Levy;

namespace SFA.DAS.Forecasting.Domain.Levy
{
    public class LevyPeriod
    {
        public long EmployerAccountId { get; set; }
        private DateTime? _lastReceieved { get; set; }
        public string PayrollYear { get; set; }
        public byte PayrollMonth { get; set; }
        private decimal _totalNetLevyDeclared { get;}
        public decimal TotalNetLevyDeclared => _totalNetLevyDeclared;

        public LevyPeriod()
        {

        }
        public LevyPeriod(long employerAccountId, string payrollYear, byte payrollMonth,
            decimal netTotal, DateTime? lastReceived)
        {
            EmployerAccountId = employerAccountId;
            PayrollYear = payrollYear;
            PayrollMonth = payrollMonth;
            _totalNetLevyDeclared = netTotal;
            _lastReceieved = lastReceived;
        }

        public decimal GetPeriodAmount()
        {
            return _totalNetLevyDeclared;
        }

        public DateTime? GetLastTimeReceivedLevy()
        {
            return _lastReceieved;
        }
    }
}