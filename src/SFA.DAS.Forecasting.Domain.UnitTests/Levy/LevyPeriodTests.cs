using System;
using AutoMoq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Levy;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Levy
{
    [TestFixture]
    public class LevyPeriodTests
    {
        protected AutoMoqer Moqer;

        [SetUp]
        public void SetUp()
        {
            Moqer = new AutoMoqer();
        }

        [Test]
        public void Rejects_Levy_Declarations_Over_2_Years_Old()
        {
            var levyPeriod = new LevyPeriod();
            Assert.Catch(() => levyPeriod.AddDeclaration(12345, "18-19", 1, 10000, "abcdef", DateTime.Today.AddYears(-3)));
        }

        [Test]
        public void GetPeriodAmount_Returns_0_If_No_Levy_Declared()
        {
            var levyPeriod = new LevyPeriod();
            Assert.AreEqual(levyPeriod.GetPeriodAmount(), 0);
        }

        [Test]
        public void GetPeriodAmount_Returns_Correct_Amount()
        {
            var levyPeriod = new LevyPeriod();
            levyPeriod.AddDeclaration(12345, "18-19", 1, 10000, "abcdef", DateTime.Today);
            levyPeriod.AddDeclaration(12345, "18-19", 1, 10000, "ghijkl", DateTime.Today);
            Assert.AreEqual(levyPeriod.GetPeriodAmount(), 20000);
        }

        [Test]
        public void Declaration_For_Same_Scheme_And_Period_Should_Overwrite_Previous_Declaration()
        {
            var levyPeriod = new LevyPeriod();
            levyPeriod.AddDeclaration(12345, "18-19", 1, 10000, "abcdef", DateTime.Today);
            levyPeriod.AddDeclaration(12345, "18-19", 1, 20000, "abcdef", DateTime.Today);
            Assert.AreEqual(levyPeriod.GetPeriodAmount(), 20000);
        }


        [Test]
        public void GetLastTimeReceived_Returns_Null_If_No_Levy_Declared()
        {
            var levyPeriod = new LevyPeriod();
            Assert.IsNull(levyPeriod.GetLastTimeReceivedLevy());
        }
    }
}