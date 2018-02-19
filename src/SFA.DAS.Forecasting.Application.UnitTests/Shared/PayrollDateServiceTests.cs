using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Shared;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.UnitTests.Shared
{
    [TestFixture]
    public class PayrollDateServiceTests
    {
        [TestCase(1, 4)]
        [TestCase(2, 5)]
        [TestCase(3, 6)]
        [TestCase(4, 7)]
        [TestCase(5, 8)]
        [TestCase(6, 9)]
        [TestCase(7, 10)]
        [TestCase(8, 11)]
        [TestCase(9, 12)]
        [TestCase(10, 1)]
        [TestCase(11, 2)]
        [TestCase(12, 3)]
        public void Converts_Payroll_Month_To_Month_Of_Year(short payrollMonth, int month)
        {
            Assert.AreEqual(month, new PayrollDateService().GetPayrollDate("17-18", payrollMonth).Month);
        }

        [TestCase("17-18", 2017)]
        [TestCase("18-19", 2018)]
        public void Converts_Payroll_Year(string payrollYear, int year)
        {
            Assert.AreEqual(year, new PayrollDateService().GetPayrollDate(payrollYear, 1).Year);
        }
    }
}