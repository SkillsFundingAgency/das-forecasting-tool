using System;
using System.Collections.Generic;
using AutoMoq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Levy;
using SFA.DAS.Forecasting.Domain.Shared;
using SFA.DAS.Forecasting.Models.Levy;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Levy
{
    [TestFixture]
    public class LevyPeriodTests
    {
        private AutoMoqer _moqer;
        private List<LevyDeclarationModel> _levyDeclarations;
        [SetUp]
        public void SetUp()
        {
            _moqer = new AutoMoqer();
            _moqer.GetMock<IPayrollDateService>()
                .Setup(svc => svc.GetPayrollDate(It.IsAny<string>(), It.IsAny<short>()))
                .Returns(DateTime.Today);
            _levyDeclarations = new List<LevyDeclarationModel>();
            _moqer.SetInstance(_levyDeclarations);
        }

        [Test]
        public void GetPeriodAmount_Returns_0_If_No_Levy_Declared()
        {
            var levyPeriod = _moqer.Resolve<LevyPeriod>();
            Assert.AreEqual(levyPeriod.GetPeriodAmount(), 0);
        }

        [Test]
        public void GetPeriodAmount_Returns_Correct_Amount()
        {
            _levyDeclarations.Add(new LevyDeclarationModel
            {
                EmployerAccountId = 12345,
                Id = 1,
                PayrollMonth = 1,
                PayrollYear = "18-19",
                LevyAmountDeclared = 10,
                TransactionDate = DateTime.Today,
                DateReceived = DateTime.Now,
                Scheme = "ABC/123"
            });
            _levyDeclarations.Add(new LevyDeclarationModel
            {
                EmployerAccountId = 12345,
                Id = 1,
                PayrollMonth = 1,
                PayrollYear = "18-19",
                LevyAmountDeclared = 20,
                TransactionDate = DateTime.Today,
                DateReceived = DateTime.Now,
                Scheme = "ABC/123"
            });
            var levyPeriod = _moqer.Resolve<LevyPeriod>();
            Assert.AreEqual(levyPeriod.GetPeriodAmount(), 30);
        }

        [Test]
        public void GetLastTimeReceived_Returns_Null_If_No_Levy_Declared()
        {
            var levyPeriod = _moqer.Resolve<LevyPeriod>();
            Assert.IsNull(levyPeriod.GetLastTimeReceivedLevy());
        }
    }
}