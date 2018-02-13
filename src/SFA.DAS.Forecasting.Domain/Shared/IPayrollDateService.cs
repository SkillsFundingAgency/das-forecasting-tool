using System;

namespace SFA.DAS.Forecasting.Domain.Shared
{
    public interface IPayrollDateService
    {
        DateTime GetPayrollDate(string payrollYear, short payrollMonth);
    }
}