using System;
using System.Collections.Generic;
using AutoMoq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Commitments
{
    [TestFixture]
    public class EmployerCommitmentsAsSenderTests
    {
        protected List<CommitmentModel> Commitments;
        protected List<CommitmentModel> CommitmentsAsSender;
        protected AutoMoqer Moqer;
        private DateTime startDate;
        private DateTime endDate;

        [SetUp]
        public void SetUp()
        {
            Moqer = new AutoMoqer();
            endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddYears(1);
            startDate = endDate.AddYears(-1);
            Commitments = new List<CommitmentModel>();
            CommitmentsAsSender = new List<CommitmentModel>();
        }

        private EmployerCommitments GetEmployerCommitments(long employerAccountId = 1) =>
            new EmployerCommitments(employerAccountId, Commitments, CommitmentsAsSender);

        [Test]
        public void Includes_Installments_for_each_month()
        {
            CommitmentsAsSender.Add(new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = startDate,
                PlannedEndDate = endDate,
                MonthlyInstallment = 800,
                NumberOfInstallments = 12,
                CompletionAmount = 6000
            });

            var employerCommitments = GetEmployerCommitments();
            var date = startDate.AddMonths(1);
            while (date <= endDate)
            {
                var costOfTraining = employerCommitments.GetTotalCostOfTraining(date);
                costOfTraining.TotalCostAsSender.Should().Be(800);
                date = date.AddMonths(1);
            }
        }

        [Test]
        public void If_Last_Day_Of_Month_Last_Payment_Is_Made_In_Month_After_Planned_End_Date__2()
        {
            CommitmentsAsSender.Add(new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = startDate,
                PlannedEndDate = endDate,
                MonthlyInstallment = 1000,
                NumberOfInstallments = 12,
                CompletionAmount = 6000
            });

            endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
            startDate = endDate.AddYears(-1);
            var employerCommitments = GetEmployerCommitments();
            var costOfTraining = employerCommitments.GetTotalCostOfTraining(endDate.AddMonths(1));
            costOfTraining.TotalCostAsSender.Should().Be(1000);

            var costOfTraining2 = employerCommitments.GetTotalCostOfTraining(endDate.AddMonths(2));
            costOfTraining2.TotalCostAsSender.Should().Be(1000);
        }
    }
}
