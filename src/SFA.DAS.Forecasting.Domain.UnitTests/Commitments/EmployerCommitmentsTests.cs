using System;
using System.Collections.Generic;
using System.Linq;
using AutoMoq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Commitments
{
    [TestFixture]
    public class EmployerCommitmentsTests
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
        public void Includes_Installment_For_Each_Month_Including_Planned_End_Date()
        {
            Commitments.Add(new CommitmentModel
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
            var employerCommitments = GetEmployerCommitments();
            var date = startDate.AddMonths(1);
            while (date <= endDate)
            {
                var costOfTraining = employerCommitments.GetTotalCostOfTraining(date);
                Assert.AreEqual(1000, costOfTraining.Value, $"Invalid total cost of training for: {date:dd/MM/yyyy}, expected £1000 but was £{costOfTraining}");
                date = date.AddMonths(1);
            }

            var costOfTrainingCompletionMonth = employerCommitments.GetTotalCostOfTraining(startDate);
            Assert.AreEqual(0, costOfTrainingCompletionMonth.Value, $"Invalid total cost of training for: {startDate:dd/MM/yyyy}, expected £0 but was £{costOfTrainingCompletionMonth}");
        }

        [Test]
        public void If_Last_Day_Of_Month_Last_Payment_Is_Made_In_Month_After_Planned_End_Date()
        {
            Commitments.Add(new CommitmentModel
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
            Assert.AreEqual(1000, costOfTraining.Value, $"Invalid total cost of training for: {endDate.AddMonths(1):dd/MM/yyyy}, expected £1000 but was £{costOfTraining}");
        }

        [Test]
        public void Aggregates_Correct_Installments_In_Month()
        {
            Commitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(2),
                MonthlyInstallment = 10,
                NumberOfInstallments = 2
            });
            Commitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 3,
                LearnerId = 4,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(5),
                MonthlyInstallment = 15,
                NumberOfInstallments = 5
            });
            Commitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 4,
                LearnerId = 5,
                StartDate = DateTime.Today.AddMonths(3),
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(12),
                MonthlyInstallment = 20,
                NumberOfInstallments = 10
            });

            var employerCommitments = GetEmployerCommitments();

            Assert.AreEqual(0, employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth()).Value);
            Assert.AreEqual(25, employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(1)).Value);
            Assert.AreEqual(25, employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(2)).Value);
            Assert.AreEqual(15, employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(3)).Value);
            Assert.AreEqual(35, employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(4)).Value);
            Assert.AreEqual(20, employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(6)).Value);
        }

        [Test]
        public void Includes_Correct_Installment_Commitments_In_Month()
        {
            Commitments.Add(new CommitmentModel
            {
                Id = 1,
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(2),
                MonthlyInstallment = 10,
                NumberOfInstallments = 2
            });
            Commitments.Add(new CommitmentModel
            {
                Id = 2,
                EmployerAccountId = 1,
                ApprenticeshipId = 3,
                LearnerId = 4,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(5),
                MonthlyInstallment = 15,
                NumberOfInstallments = 5
            });
            Commitments.Add(new CommitmentModel
            {
                Id = 3,
                EmployerAccountId = 1,
                ApprenticeshipId = 4,
                LearnerId = 5,
                StartDate = DateTime.Today.AddMonths(2),
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(12),
                MonthlyInstallment = 20,
                NumberOfInstallments = 10
            });
            var employerCommitments = GetEmployerCommitments();
            Assert.IsFalse(employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth()).CommitmentIds.Any());
            Assert.IsTrue(employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(1)).CommitmentIds.All(id => id == 1 || id == 2));
            Assert.IsTrue(employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(2)).CommitmentIds.All(id => id == 1 || id == 2));
            Assert.IsTrue(employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(3)).CommitmentIds.All(id => id == 2 || id == 3));
            Assert.IsTrue(employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(5)).CommitmentIds.All(id => id == 2 || id == 3));
            Assert.IsTrue(employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(6)).CommitmentIds.All(id => id == 3));
        }

        [Test]
        public void Get_Total_Cost_Of_Training_Includes_Is_Included_On_Planned_End_Date()
        {
            Commitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(2),
                MonthlyInstallment = 10,
                NumberOfInstallments = 2
            });
            var employerCommitments = GetEmployerCommitments();
            Assert.AreEqual(10, employerCommitments.GetTotalCostOfTraining(DateTime.Today.AddMonths(2)).Value);
        }

        [Test]
        public void Completion_Payments_Are_Aggregated_In_Month_After_Planed_End_Date()
        {
            Commitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(2),
                MonthlyInstallment = 10,
                NumberOfInstallments = 2,
                CompletionAmount = 30
            });
            Commitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 3,
                LearnerId = 4,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(5),
                MonthlyInstallment = 10,
                NumberOfInstallments = 5,
                CompletionAmount = 50
            });
            var employerCommitments = GetEmployerCommitments();
            Assert.AreEqual(0, employerCommitments.GetTotalCompletionPayments(DateTime.Today).Item1);
            Assert.AreEqual(30, employerCommitments.GetTotalCompletionPayments(DateTime.Today.AddMonths(3)).Item1);
            Assert.AreEqual(50, employerCommitments.GetTotalCompletionPayments(DateTime.Today.AddMonths(6)).Item1);
        }

        [Test]
        public void Completion_Payments_Are_Included_In_Month_After_Planned_End_Date()
        {
            Commitments.Add(new CommitmentModel
            {
                Id = 1,
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(2),
                MonthlyInstallment = 10,
                NumberOfInstallments = 2,
                CompletionAmount = 30
            });
            Commitments.Add(new CommitmentModel
            {
                Id = 2,
                EmployerAccountId = 1,
                ApprenticeshipId = 3,
                LearnerId = 4,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(5),
                MonthlyInstallment = 10,
                NumberOfInstallments = 5,
                CompletionAmount = 50
            });
            var employerCommitments = GetEmployerCommitments();

            Assert.IsFalse(employerCommitments.GetTotalCompletionPayments(DateTime.Today.AddMonths(2)).Item2.Any());
            Assert.IsTrue(employerCommitments.GetTotalCompletionPayments(DateTime.Today.AddMonths(3)).Item2.All(id => id == 1));
            Assert.IsTrue(employerCommitments.GetTotalCompletionPayments(DateTime.Today.AddMonths(6)).Item2.All(id => id == 2));
        }
    }
}