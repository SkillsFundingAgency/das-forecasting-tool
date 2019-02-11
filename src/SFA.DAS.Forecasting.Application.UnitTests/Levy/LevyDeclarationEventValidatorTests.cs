using System;
using AutoMoq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Application.Levy.Validation;

namespace SFA.DAS.Forecasting.Application.UnitTests.Levy
{
    [TestFixture]
    public class LevyDeclarationEventValidatorTests
    {
        protected AutoMoqer Moqer { get; private set; }
        protected LevySchemeDeclarationUpdatedMessage LevySchemeDeclarationUpdatedMessage { get; set; }
        [SetUp]
        public void SetUp()
        {
            Moqer = new AutoMoqer();
            LevySchemeDeclarationUpdatedMessage = new LevySchemeDeclarationUpdatedMessage
            {
                LevyDeclaredInMonth = 1000,
                AccountId = 123456,
                PayrollYear = "18/19",
                PayrollMonth = 1,
                EmpRef = "ABCD",
                CreatedDate = DateTime.Today
            };
        }

        [Test]
        public void Fails_If_Employer_Account_Id_Is_Not_Populated()
        {
            var validator = new LevyDeclarationEventValidator();
            LevySchemeDeclarationUpdatedMessage.AccountId = 0;
            var result = validator.Validate(LevySchemeDeclarationUpdatedMessage);
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void Fails_If_Declaration_Date_Is_Invalid()
        {
            var validator = new LevyDeclarationEventValidator();
            LevySchemeDeclarationUpdatedMessage.PayrollYear = null;
            var result = validator.Validate(LevySchemeDeclarationUpdatedMessage);
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void Fails_If_Scheme_Invalid()
        {
            var validator = new LevyDeclarationEventValidator();
            LevySchemeDeclarationUpdatedMessage.EmpRef = null;
            var result = validator.Validate(LevySchemeDeclarationUpdatedMessage);
            result.IsValid.Should().BeFalse();
        }


        [Test]
        public void Passes_If_Amount_Is_Negative()
        {
            var validator = new LevyDeclarationEventValidator();
            LevySchemeDeclarationUpdatedMessage.LevyDeclaredInMonth = -1;
            var result = validator.Validate(LevySchemeDeclarationUpdatedMessage);
            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void Passes_If_Amount_Is_Zero()
        {
            var validator = new LevyDeclarationEventValidator();
            LevySchemeDeclarationUpdatedMessage.LevyDeclaredInMonth = 0;
            var result = validator.Validate(LevySchemeDeclarationUpdatedMessage);
            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void Fails_If_Transaction_Date_Is_Invalid()
        {
            var validator = new LevyDeclarationEventValidator();
            LevySchemeDeclarationUpdatedMessage.CreatedDate = new DateTime(0001, 01, 01);
            var result = validator.Validate(LevySchemeDeclarationUpdatedMessage);
            result.IsValid.Should().BeFalse();
        }
    }
}