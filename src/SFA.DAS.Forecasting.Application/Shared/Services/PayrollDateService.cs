using System;
using System.Linq;
using SFA.DAS.Forecasting.Domain.Shared;

namespace SFA.DAS.Forecasting.Application.Shared.Services
{
    public class PayrollDateService : IPayrollDateService
    {
        public DateTime GetPayrollDate(string payrollYear, short payrollMonth)
        {
            var yearTextSplit = payrollYear.Split('-');
            var yearText = payrollMonth <= 9 ? yearTextSplit[0] : yearTextSplit[1];

            yearText = $"20{yearText}";
            if (string.IsNullOrEmpty(yearText) || !int.TryParse(yearText, out int year))
                throw new InvalidOperationException($"Invalid payroll year '{payrollYear}'. Cannot parse start year.");

            var month = payrollMonth <= 9 ? payrollMonth + 3 : payrollMonth - 9;

            return new DateTime(year, month, 1);
        }
    }
}