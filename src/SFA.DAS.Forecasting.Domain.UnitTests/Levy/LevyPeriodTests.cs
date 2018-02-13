using System;
using AutoMoq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Levy;
using SFA.DAS.Forecasting.Domain.Shared;

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
            Moqer.GetMock<IPayrollDateService>()
                .Setup(svc => svc.GetPayrollDate(It.IsAny<string>(), It.IsAny<short>()))
                .Returns(DateTime.Today);
        }

        [Test]
        public void Rejects_Levy_Declarations_Over_2_Years_Old()
        {
            var levyPeriod = Moqer.Resolve<LevyPeriod>();
            Assert.Catch(() => levyPeriod.AddDeclaration(12345, "18-19", 1, 10000, "abcdef", DateTime.Today.AddYears(-3)));
        }

        [Test]
        public void GetPeriodAmount_Returns_0_If_No_Levy_Declared()
        {
            var levyPeriod = Moqer.Resolve<LevyPeriod>();
            Assert.AreEqual(levyPeriod.GetPeriodAmount(), 0);
        }

        [Test]
        public void GetPeriodAmount_Returns_Correct_Amount()
        {
            var levyPeriod = Moqer.Resolve<LevyPeriod>();
            levyPeriod.AddDeclaration(12345, "18-19", 1, 10000, "abcdef", DateTime.Today);
            levyPeriod.AddDeclaration(12345, "18-19", 1, 10000, "ghijkl", DateTime.Today);
            Assert.AreEqual(levyPeriod.GetPeriodAmount(), 20000);
        }

        [Test]
        public void Declaration_For_Same_Scheme_And_Period_Should_Overwrite_Previous_Declaration()
        {
            var levyPeriod = Moqer.Resolve<LevyPeriod>();
            levyPeriod.AddDeclaration(12345, "18-19", 1, 10000, "abcdef", DateTime.Today);
            levyPeriod.AddDeclaration(12345, "18-19", 1, 20000, "abcdef", DateTime.Today);
            Assert.AreEqual(levyPeriod.GetPeriodAmount(), 20000);
        }


        [Test]
        public void GetLastTimeReceived_Returns_Null_If_No_Levy_Declared()
        {
            var levyPeriod = Moqer.Resolve<LevyPeriod>();
            Assert.IsNull(levyPeriod.GetLastTimeReceivedLevy());
        }
    }
}