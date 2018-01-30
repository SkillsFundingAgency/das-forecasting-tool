using System;
using AutoMoq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Levy.Aggregates;

namespace SFA.DAS.Forecasting.Levy.UnitTests.Domain
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
    }
}