using System;
using System.Collections.Generic;
using AutoMoq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Commitments
{
    [TestFixture]
    public class EmployerCommitmentsAsSenderTests
    {
        protected List<CommitmentModel> Commitments;
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
        }

        private EmployerCommitments GetEmployerCommitments(long employerAccountId = 1) =>
            new EmployerCommitments(employerAccountId, Commitments);

        [Test]
        public void Includes_Installments_for_each_month()
        {
            Commitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1003,
                SendingEmployerAccountId = 1,
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
                costOfTraining.TransferCost.Should().Be(800);
                date = date.AddMonths(1);
            }
        }

        [Test]
        public void If_Last_Day_Of_Month_Last_Payment_Is_Made_In_Month_After_Planned_End_Date()
        {
            endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
            startDate = endDate.AddYears(-1);

            Commitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1003,
                SendingEmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = startDate,
                PlannedEndDate = endDate,
                MonthlyInstallment = 1000,
                NumberOfInstallments = 12,
                CompletionAmount = 6000
            });

            var employerCommitments = GetEmployerCommitments();
            var costOfTraining = employerCommitments.GetTotalCostOfTraining(endDate.AddMonths(1));
            costOfTraining.TransferCost.Should().Be(1000, "Should calculate for next month");

            var costOfTraining2 = employerCommitments.GetTotalCostOfTraining(endDate.AddMonths(2));
            costOfTraining2.TransferCost.Should().Be(0, "No cost after commitment finished");
        }

        [Test]
        public void Aggregates_Correct_Installments_In_Month()
        {
            Commitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1003,
                SendingEmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(2),
                MonthlyInstallment = 10,
                NumberOfInstallments = 2
            });
            Commitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1003,
                SendingEmployerAccountId = 1,
                ApprenticeshipId = 3,
                LearnerId = 4,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(5),
                MonthlyInstallment = 15,
                NumberOfInstallments = 5
            });
            Commitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1003,
                SendingEmployerAccountId = 1,
                ApprenticeshipId = 4,
                LearnerId = 5,
                StartDate = DateTime.Today.AddMonths(3),
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(12),
                MonthlyInstallment = 20,
                NumberOfInstallments = 10
            });

            var employerCommitments = GetEmployerCommitments();

            Assert.AreEqual(0, employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth()).TransferCost);
            Assert.AreEqual(25, employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(1)).TransferCost);
            Assert.AreEqual(25, employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(2)).TransferCost);
            Assert.AreEqual(15, employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(3)).TransferCost);
            Assert.AreEqual(35, employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(4)).TransferCost);
            Assert.AreEqual(20, employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(6)).TransferCost);
        }

        [Test]
        public void Get_Total_Cost_Of_Training_Includes_Is_Included_On_Planned_End_Date()
        {
            Commitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1003,
                SendingEmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(2),
                MonthlyInstallment = 10,
                NumberOfInstallments = 2
            });
            var employerCommitments = GetEmployerCommitments();
            Assert.AreEqual(10, employerCommitments.GetTotalCostOfTraining(DateTime.Today.AddMonths(2)).TransferCost);
        }
    }
}
