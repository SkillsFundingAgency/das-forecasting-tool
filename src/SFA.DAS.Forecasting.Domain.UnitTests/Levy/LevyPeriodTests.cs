using System;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Levy;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Levy
{
    [TestFixture]
    public class LevyPeriodTests
    {
        [Test]
        public void GetPeriodAmount_Returns_0_If_No_Levy_Declared()
        {
            var levyPeriod = new LevyPeriod();
            Assert.AreEqual(levyPeriod.GetPeriodAmount(), 0);
        }

        [Test]
        public void GetPeriodAmount_Returns_Correct_Amount()
        {

            var levyPeriod = new LevyPeriod(12345,"18-19",1,DateTime.Parse("2018-04-01"), 30,DateTime.Now);
            Assert.AreEqual(levyPeriod.GetPeriodAmount(), 30);
        }

        [Test]
        public void GetLastTimeReceived_Returns_Null_If_No_Levy_Declared()
        {
            var levyPeriod = new LevyPeriod();
            Assert.IsNull(levyPeriod.GetLastTimeReceivedLevy());
        }
    }
}