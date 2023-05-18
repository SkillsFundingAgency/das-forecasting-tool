using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Shared.Services;

namespace SFA.DAS.Forecasting.Application.UnitTests.Shared
{
    [TestFixture]
    public class PayrollDateServiceTests
    {
        [TestCase(1, "17-18", 4 ,2017)]
        [TestCase(2, "17-18", 5 ,2017)]
        [TestCase(3, "17-18", 6 ,2017)]
        [TestCase(4, "17-18", 7 ,2017)]
        [TestCase(5, "17-18", 8 ,2017)]
        [TestCase(6, "17-18", 9 ,2017)]
        [TestCase(7, "17-18", 10,2017)]
        [TestCase(8, "17-18", 11,2017)]
        [TestCase(9, "17-18", 12,2017)]
        [TestCase(10,"17-18",  1,2018)]
        [TestCase(11,"17-18",  2,2018)]
        [TestCase(12,"17-18",  3,2018)]
        public void Converts_Payroll_Month_To_Month_Of_Year(short payrollMonth, string payrollYear, int month, int year)
        {
            Assert.AreEqual(month, new PayrollDateService().GetPayrollDate(payrollYear, payrollMonth).Month);
            Assert.AreEqual(year, new PayrollDateService().GetPayrollDate(payrollYear, payrollMonth).Year);
        }

        [TestCase("17-18", 2017)]
        [TestCase("18-19", 2018)]
        public void Converts_Payroll_Year(string payrollYear, int year)
        {
            Assert.AreEqual(year, new PayrollDateService().GetPayrollDate(payrollYear, 1).Year);
        }
    }
}