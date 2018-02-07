using System;
using System.Collections.Generic;
using System.Linq;
using AutoMoq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Commitments.Model;
using SFA.DAS.Forecasting.Domain.Events;
using SFA.DAS.Forecasting.Domain.Projections.Model;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Projection
{
    [TestFixture]
    public class AccountProjectionTests
    {
        protected AutoMoqer Moqer { get; private set; }
        private AccountActivity _accountActivity;
        private Commitment _commitment;
        private List<Commitment> _commitments;

        [SetUp]
        public void SetUp()
        {
            Moqer = new AutoMoqer();
            _accountActivity = new AccountActivity
            {
                LevyPeriod = DateTime.Today,
                EmployerAccountId = 1,
                LevyDeclared = 7000,
                Balance = 47700,
                TotalCostOfTraining = 100,
                TrainingCostPeriod = DateTime.Today
            };
            Moqer.SetInstance(_accountActivity);
            _commitment = new Commitment
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.AddMonths(25),
                MonthlyInstallment = 500,
                NumberOfInstallments = 24,
                CompletionAmount = 3000
            };
            _commitments = new List<Commitment> { _commitment };
            var employerCommitments = new EmployerCommitments(1, _commitments, Moqer.GetMock<IEventPublisher>().Object);
            Moqer.SetInstance(employerCommitments);
        }

        [Test]
        public void Skips_Current_Month()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            var projections = accountProjection.GetLevyTriggeredProjections(DateTime.Today, 1);
            Assert.AreEqual(DateTime.Today.Month + 1, projections.FirstOrDefault()?.Month);
        }

        [Test]
        public void Projects_Requested_Number_Of_Months()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            var projections = accountProjection.GetLevyTriggeredProjections(DateTime.Today, 48);
            Assert.AreEqual(projections.Count(), 48);
        }

        [Test]
        public void Projects_Levy_Forward_For_Requested_Number_Of_Months()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            var projections = accountProjection.GetLevyTriggeredProjections(DateTime.Today, 48);
            Assert.AreEqual(projections.Count(projection => projection.FundsIn == 7000), 48);
        }

        [Test]
        public void Does_Not_Include_Levy_In_First_Month_For_Levy_Triggered_Projection()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            var projections = accountProjection.GetLevyTriggeredProjections(DateTime.Today, 1);
            Assert.AreEqual(projections.FirstOrDefault()?.FutureFunds, _accountActivity.Balance - _commitment.MonthlyInstallment);
        }

        [Test]
        public void Includes_Levy_In_Months_After_First_Month_For_Levy_Triggered_Projection()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            var projections = accountProjection.GetLevyTriggeredProjections(DateTime.Today, 2);
            Assert.AreEqual(projections.Skip(1).FirstOrDefault()?.FutureFunds, projections.FirstOrDefault()?.FutureFunds + _accountActivity.LevyDeclared - _commitment.MonthlyInstallment);
        }

        [Test]
        public void Includes_Levy_In_First_Months_For_Payroll_Period_End_Triggered_Projection()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            var projections = accountProjection.GetPayrollPeriodEndTriggeredProjections(DateTime.Today, 2);
            Assert.AreEqual(_accountActivity.Balance + _accountActivity.LevyDeclared - _commitment.MonthlyInstallment, projections.FirstOrDefault()?.FutureFunds);
        }

        [Test]
        public void Includes_Completion_Payments()
        {
            _commitment.StartDate = DateTime.Today.AddMonths(-23);
            _commitment.PlannedEndDate = DateTime.Today.AddMonths(1);
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            var projections = accountProjection.GetLevyTriggeredProjections(DateTime.Today, 2);
            Console.WriteLine(projections.ToJson());
            Assert.AreEqual(projections.FirstOrDefault()?.FutureFunds, _accountActivity.Balance - _commitment.CompletionAmount);
        }

        [Test]
        public void Includes_Installments()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            var projections = accountProjection.GetLevyTriggeredProjections(DateTime.Today, 2);
            Console.WriteLine(projections.ToJson());
            Assert.AreEqual(_accountActivity.Balance - _commitment.MonthlyInstallment, projections.FirstOrDefault()?.FutureFunds);
            Assert.AreEqual(projections.FirstOrDefault()?.FutureFunds + _accountActivity.LevyDeclared - _commitment.MonthlyInstallment, projections.Skip(1).FirstOrDefault()?.FutureFunds);
        }
    }
}