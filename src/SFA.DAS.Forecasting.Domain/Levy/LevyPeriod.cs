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
        private readonly decimal _totalNetLevyDeclared;
        private readonly DateTime? _lastReceieved;
        private readonly DateTime _payrollDate;
        public long EmployerAccountId { get; set; }
        
        public string PayrollYear { get; set; }
        public byte PayrollMonth { get; set; }
        public int CalendarMonth => _payrollDate.Month;
        public int CalendarYear => _payrollDate.Year;


        public decimal TotalNetLevyDeclared => _totalNetLevyDeclared;

        public LevyPeriod()
        {

        }
        public LevyPeriod(long employerAccountId, string payrollYear, byte payrollMonth, DateTime payrollDate,
            decimal netTotal, DateTime? lastReceived)
        {
            EmployerAccountId = employerAccountId;
            PayrollYear = payrollYear;
            PayrollMonth = payrollMonth;
            _totalNetLevyDeclared = netTotal;
            _lastReceieved = lastReceived;
            _payrollDate = payrollDate;
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