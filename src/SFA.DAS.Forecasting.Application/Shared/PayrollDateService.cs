using System;
using System.Linq;

namespace SFA.DAS.Forecasting.Application.Shared
{
    public interface IPayrollDateService
    {
        DateTime GetPayrollDate(string payrollYear, short payrollMonth);
    }

    public class PayrollDateService : IPayrollDateService
    {
        public DateTime GetPayrollDate(string payrollYear, short payrollMonth)
        {
            var yearText = payrollYear.Split('-').FirstOrDefault();
            if (string.IsNullOrEmpty(yearText) || !int.TryParse(yearText, out int year))
                throw new InvalidOperationException($"Invalid payroll year '{payrollYear}'. Cannot parse start year.");
            return new DateTime(year, payrollMonth <= 9 ? payrollMonth + 3 : payrollMonth - 9, 1);
        }
    }
}