using System;
using AutoMoq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Levy.Application.Messages;
using SFA.DAS.Forecasting.Levy.Application.Validation;

namespace SFA.DAS.Forecasting.Levy.UnitTests.Application
{
    [TestFixture]
    public class LevyDeclarationEventValidatorTests
    {
        protected AutoMoqer Moqer { get; private set; }
        protected LevyDeclarationEvent LevyDeclarationEvent { get; set; }
        [SetUp]
        public void SetUp()
        {
            Moqer = new AutoMoqer();
            LevyDeclarationEvent = new LevyDeclarationEvent
            {
                Amount = 1000,
                EmployerAccountId = Guid.NewGuid().ToString("N"),
                SubmissionDate = DateTime.Today,
                Scheme = "ABCD",
                TransactionDate = DateTime.Today
            };
        }

        [Test]
        public void Fails_If_Employer_Account_Id_Is_Not_Populated()
        {
            var validator = new LevyDeclarationEventValidator();
            LevyDeclarationEvent.EmployerAccountId = null;
            var result = validator.Validate(LevyDeclarationEvent);
            Assert.IsNotEmpty(result);
        }

        [Test]
        public void Fails_If_Declaration_Date_Is_Invalid()
        {
            var validator = new LevyDeclarationEventValidator();
            LevyDeclarationEvent.SubmissionDate = new DateTime(0001, 01, 01);
            var result = validator.Validate(LevyDeclarationEvent);
            Assert.IsNotEmpty(result);
        }

        [Test]
        public void Fails_If_Scheme_Invalid()
        {
            var validator = new LevyDeclarationEventValidator();
            LevyDeclarationEvent.Scheme = null;
            var result = validator.Validate(LevyDeclarationEvent);
            Assert.IsNotEmpty(result);
        }


        [Test]
        public void Fails_If_Amount_Is_Negative()
        {
            var validator = new LevyDeclarationEventValidator();
            LevyDeclarationEvent.Amount = -1;
            var result = validator.Validate(LevyDeclarationEvent);
            Assert.IsNotEmpty(result);
        }


        [Test]
        public void Fails_If_Transaction_Date_Is_Invalid()
        {
            var validator = new LevyDeclarationEventValidator();
            LevyDeclarationEvent.TransactionDate = new DateTime(0001, 01, 01);
            var result = validator.Validate(LevyDeclarationEvent);
            Assert.IsNotEmpty(result);
        }
    }
}