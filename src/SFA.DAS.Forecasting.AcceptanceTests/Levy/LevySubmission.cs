using System;
using System.Data.Entity;

namespace SFA.DAS.Forecasting.AcceptanceTests.Levy
{
    public class LevySubmission
    {
        public string Scheme { get; set; }
        public decimal Amount { get; set; }
        public string CreatedDate { get; set; }
        public bool UseCreatedDateAsPayrollDate { get; set; }
        private int _payrollYear => CalculatePayrollYear();
        public string PayrollYear => (_payrollYear-1) + "-" + _payrollYear.ToString();
        public short PayrollMonth => (short)CreatedDateValue.AddMonths(-4).Month;

        public DateTime CreatedDateValue => CreatedDate.ToDateTime();
        
        private int CalculatePayrollYear()
        {
            return int.Parse(CreatedDateValue.Month < 4
                    ? CreatedDateValue.AddYears(- 1).ToString("yy")
                    : CreatedDateValue.ToString("yy"));

        }
    }
}