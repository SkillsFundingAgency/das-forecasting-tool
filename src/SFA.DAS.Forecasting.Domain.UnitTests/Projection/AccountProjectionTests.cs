using System;
using System.Collections.Generic;
using System.Linq;
using AutoMoq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Commitments.Validation;
using SFA.DAS.Forecasting.Domain.Events;
using SFA.DAS.Forecasting.Models.Balance;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Projection
{
    [TestFixture]
    public class AccountProjectionTests
    {
        protected AutoMoqer Moqer { get; private set; }
        private Account _account;
        private CommitmentModel _commitment;
        private List<CommitmentModel> _commitments;

        [SetUp]
        public void SetUp()
        {
            Moqer = new AutoMoqer();
            _account = new Account(1, 12000, 300, 0, 0);
            Moqer.SetInstance(_account);
            _commitment = new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.AddMonths(25),
                MonthlyInstallment = 2100,
                NumberOfInstallments = 24,
                CompletionAmount = 3000
            };
            _commitments = new List<CommitmentModel> { _commitment };
            var employerCommitments = new EmployerCommitments(1, _commitments, null);
            Moqer.SetInstance(employerCommitments);
        }

        [Test]
        public void Starts_From_Next_Month()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            accountProjection.BuildLevyTriggeredProjections(DateTime.Today, 1);
            Assert.AreEqual(DateTime.Today.Month + 1, accountProjection.Projections.FirstOrDefault()?.Month);
        }

        [Test]
        public void Projects_Requested_Number_Of_Months()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            accountProjection.BuildLevyTriggeredProjections(DateTime.Today, 48);
            Assert.AreEqual(accountProjection.Projections.Count, 48);
        }

        [Test]
        public void Projects_Levy_Forward_For_Requested_Number_Of_Months()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            accountProjection.BuildLevyTriggeredProjections(DateTime.Today, 48);
            Assert.AreEqual(accountProjection.Projections.Count(projection => projection.FundsIn == 300), 48);
        }

        [Test]
        public void Does_Not_Include_Levy_In_First_Month_For_Levy_Triggered_Projection()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            accountProjection.BuildLevyTriggeredProjections(DateTime.Today, 1);
            Assert.AreEqual(accountProjection.Projections.FirstOrDefault()?.FutureFunds, _account.Balance - _commitment.MonthlyInstallment);
        }

        [Test]
        public void Includes_Levy_In_Months_After_First_Month_For_Levy_Triggered_Projection()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            accountProjection.BuildLevyTriggeredProjections(DateTime.Today, 2);
            Assert.AreEqual(accountProjection.Projections.Skip(1).FirstOrDefault()?.FutureFunds, accountProjection.Projections.FirstOrDefault()?.FutureFunds + _account.LevyDeclared - _commitment.MonthlyInstallment);
        }

        [Test]
        public void Includes_Levy_In_First_Months_For_Payroll_Period_End_Triggered_Projection()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            accountProjection.BuildPayrollPeriodEndTriggeredProjections(DateTime.Today, 2);
            Assert.AreEqual(_account.Balance + _account.LevyDeclared - _commitment.MonthlyInstallment, accountProjection.Projections.FirstOrDefault()?.FutureFunds);
        }

        [Test]
        public void Includes_Completion_Payments()
        {
            _commitment.StartDate = DateTime.Today.AddMonths(-1);
            _commitment.PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(1);
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            accountProjection.BuildLevyTriggeredProjections(DateTime.Today, 2);
            Console.WriteLine(accountProjection.Projections.ToJson());
            Assert.AreEqual(accountProjection.Projections.Skip(1).FirstOrDefault()?.CompletionPayments, _commitment.CompletionAmount);
        }

        [Test]
        public void Includes_Installments()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            accountProjection.BuildLevyTriggeredProjections(DateTime.Today, 2);
            Console.WriteLine(accountProjection.Projections.ToJson());
            Assert.AreEqual(_account.Balance - _commitment.MonthlyInstallment, accountProjection.Projections.FirstOrDefault()?.FutureFunds);
            Assert.AreEqual(accountProjection.Projections.FirstOrDefault()?.FutureFunds + _account.LevyDeclared - _commitment.MonthlyInstallment, accountProjection.Projections.Skip(1).FirstOrDefault()?.FutureFunds);
        }

        [Test]
        public void Includes_Co_Investment()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            accountProjection.BuildLevyTriggeredProjections(DateTime.Today, 7);
            accountProjection.Projections[5].CoInvestmentEmployer.Should().Be(0);
            accountProjection.Projections[5].CoInvestmentGovernment.Should().Be(0);
            accountProjection.Projections[5].FutureFunds.Should().Be(900);

            accountProjection.Projections[6].CoInvestmentEmployer.Should().Be(90);
            accountProjection.Projections[6].CoInvestmentGovernment.Should().Be(810);
            accountProjection.Projections[6].FutureFunds.Should().Be(0);
        }
    }
}