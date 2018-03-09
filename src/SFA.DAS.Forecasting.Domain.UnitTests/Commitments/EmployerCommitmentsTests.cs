using System;
using System.Collections.Generic;
using System.Linq;
using AutoMoq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Commitments.Validation;
using SFA.DAS.Forecasting.Domain.Events;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Commitments
{
    [TestFixture]
    public class EmployerCommitmentsTests
    {
        protected List<Commitment> Commitments;
        protected AutoMoqer Moqer;

        [SetUp]
        public void SetUp()
        {
            Moqer = new AutoMoqer();
        }

        //[Test]
        //public void Does_Not_Add_Ended_Commitments()
        //{
        //    var employerCommitments = new EmployerCommitments(1, new List<Commitment>(), Moqer.GetMock<IEventPublisher>().Object);
        //    employerCommitments.AddCommitment(1, 2, DateTime.Today, DateTime.Today.AddDays(1), DateTime.Today, 87.27m, 240, 12);
        //    Assert.IsFalse(employerCommitments.Commitments.Any());
        //}

        [Test]
        public void Updates_Existing_Commitment()
        {
            var employerCommitments = new EmployerCommitments(1, new List<Commitment>
            {
                new Commitment
                {
                    EmployerAccountId = 1,
                    ApprenticeshipId = 2,
                    LearnerId = 0,
                    ApprenticeName = string.Empty,
                    ProviderName = string.Empty,
                    ProviderId = 0,
                    MonthlyInstallment = 10,
                    NumberOfInstallments = 1,
                    CompletionAmount = 50,
                    CourseLevel = 0,
                }
            }, Moqer.GetMock<IEventPublisher>().Object,
            new CommitmentValidator());
            employerCommitments.AddCommitment(2, 3, "test apprentice", "test course", 1, 4, "test provider", DateTime.Today,
                DateTime.Today.AddDays(1), null, 87.27m, 240, 12);
            Assert.AreEqual(1, employerCommitments.Commitments.Count);
            var commitment = employerCommitments.Commitments.First();
            Assert.AreEqual(commitment.ApprenticeName, "test apprentice");
            Assert.AreEqual(commitment.ProviderName, "test provider");
            Assert.AreEqual(commitment.CourseName, "test course");
            Assert.AreEqual(commitment.ProviderId, 4);
            Assert.AreEqual(commitment.LearnerId, 3);
            Assert.AreEqual(commitment.CourseLevel, 1);
        }

        [Test]
        public void Includes_Installment_For_Each_Month_Including_Planned_End_Date()
        {
            var endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddYears(1);
            var startDate = endDate.AddYears(-1);
            var employerCommitments = new EmployerCommitments(1, new List<Commitment>
            {
                new Commitment
                {
                    EmployerAccountId = 1,
                    ApprenticeshipId = 2,
                    LearnerId = 3,
                    StartDate = startDate,
                    PlannedEndDate = endDate,
                    MonthlyInstallment = 1000,
                    NumberOfInstallments = 12,
                    CompletionAmount = 6000
                }
            }, Moqer.GetMock<IEventPublisher>().Object,
            new CommitmentValidator());

            var date = startDate.AddMonths(1);
            while (date <= endDate)
            {
                var costOfTraining = employerCommitments.GetTotalCostOfTraining(date);
                Assert.AreEqual(1000, costOfTraining.Item1, $"Invalid total cost of training for: {date:dd/MM/yyyy}, expected £1000 but was £{costOfTraining}");
                date = date.AddMonths(1);
            }

            var costOfTrainingCompletionMonth = employerCommitments.GetTotalCostOfTraining(startDate);
            Assert.AreEqual(0, costOfTrainingCompletionMonth.Item1, $"Invalid total cost of training for: {startDate:dd/MM/yyyy}, expected £0 but was £{costOfTrainingCompletionMonth}");
        }

        [Test]
        public void If_Last_Day_Of_Month_Last_Payment_Is_Made_In_Month_After_Planned_End_Date()
        {
            var endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
            var startDate = endDate.AddYears(-1);
            var employerCommitments = new EmployerCommitments(1, new List<Commitment>
                {
                    new Commitment
                    {
                        EmployerAccountId = 1,
                        ApprenticeshipId = 2,
                        LearnerId = 3,
                        StartDate = startDate,
                        PlannedEndDate = endDate,
                        MonthlyInstallment = 1000,
                        NumberOfInstallments = 12,
                        CompletionAmount = 6000
                    }
                }, Moqer.GetMock<IEventPublisher>().Object,
                new CommitmentValidator());

            var costOfTraining = employerCommitments.GetTotalCostOfTraining(endDate.AddMonths(1));
            Assert.AreEqual(1000, costOfTraining.Item1, $"Invalid total cost of training for: {endDate.AddMonths(1):dd/MM/yyyy}, expected £1000 but was £{costOfTraining}");
        }

        [Test]
        public void Aggregates_Correct_Installments_In_Month()
        {
            var employerCommitments = new EmployerCommitments(1, new List<Commitment>
            {
                new Commitment
                {
                    EmployerAccountId =1,
                    ApprenticeshipId = 2,
                    LearnerId = 3,
                    StartDate = DateTime.Today,
                    PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(2),
                    MonthlyInstallment = 10,
                    NumberOfInstallments = 2
                },
                new Commitment
                {
                    EmployerAccountId =1,
                    ApprenticeshipId = 3,
                    LearnerId = 4,
                    StartDate = DateTime.Today,
                    PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(5),
                    MonthlyInstallment = 15,
                    NumberOfInstallments = 5
                },
                new Commitment
                {
                    EmployerAccountId =1,
                    ApprenticeshipId = 4,
                    LearnerId = 5,
                    StartDate = DateTime.Today.AddMonths(3),
                    PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(12),
                    MonthlyInstallment = 20,
                    NumberOfInstallments = 10
                },
            }, Moqer.GetMock<IEventPublisher>().Object,
                new CommitmentValidator());

            Assert.AreEqual(0, employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth()).Item1);
            Assert.AreEqual(25, employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(1)).Item1);
            Assert.AreEqual(25, employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(2)).Item1);
            Assert.AreEqual(15, employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(3)).Item1);
            Assert.AreEqual(35, employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(4)).Item1);
            Assert.AreEqual(20, employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(6)).Item1);
        }

        [Test]
        public void Includes_Correct_Installment_Commitments_In_Month()
        {
            var employerCommitments = new EmployerCommitments(1, new List<Commitment>
            {
                new Commitment
                {
                    Id = 1,
                    EmployerAccountId =1,
                    ApprenticeshipId = 2,
                    LearnerId = 3,
                    StartDate = DateTime.Today,
                    PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(2),
                    MonthlyInstallment = 10,
                    NumberOfInstallments = 2
                },
                new Commitment
                {
                    Id = 2,
                    EmployerAccountId =1,
                    ApprenticeshipId = 3,
                    LearnerId = 4,
                    StartDate = DateTime.Today,
                    PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(5),
                    MonthlyInstallment = 15,
                    NumberOfInstallments = 5
                },
                new Commitment
                {
                    Id = 3,
                    EmployerAccountId =1,
                    ApprenticeshipId = 4,
                    LearnerId = 5,
                    StartDate = DateTime.Today.AddMonths(2),
                    PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(12),
                    MonthlyInstallment = 20,
                    NumberOfInstallments = 10
                },
            }, Moqer.GetMock<IEventPublisher>().Object,
            new CommitmentValidator());

            Assert.IsFalse(employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth()).Item2.Any());
            Assert.IsTrue(employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(1)).Item2.All(id => id == 1 || id == 2));
            Assert.IsTrue(employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(2)).Item2.All(id => id == 1 || id == 2));
            Assert.IsTrue(employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(3)).Item2.All(id => id == 2 || id == 3));
            Assert.IsTrue(employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(5)).Item2.All(id => id == 2 || id == 3));
            Assert.IsTrue(employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(6)).Item2.All(id => id == 3));
        }

        [Test]
        public void Get_Total_Cost_Of_Training_Includes_Is_Included_On_Planned_End_Date()
        {
            var employerCommitments = new EmployerCommitments(1, new List<Commitment>
            {
                new Commitment
                {
                    EmployerAccountId =1,
                    ApprenticeshipId = 2,
                    LearnerId = 3,
                    StartDate = DateTime.Today,
                    PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(2),
                    MonthlyInstallment = 10,
                    NumberOfInstallments = 2
                },
            }, Moqer.GetMock<IEventPublisher>().Object,
                new CommitmentValidator());
            Assert.AreEqual(10, employerCommitments.GetTotalCostOfTraining(DateTime.Today.AddMonths(2)).Item1);
        }

        [Test]
        public void Completion_Payments_Are_Aggregated_In_Month_After_Planed_End_Date()
        {
            var employerCommitments = new EmployerCommitments(1, new List<Commitment>
            {
                new Commitment
                {
                    EmployerAccountId =1,
                    ApprenticeshipId = 2,
                    LearnerId = 3,
                    StartDate = DateTime.Today,
                    PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(2),
                    MonthlyInstallment = 10,
                    NumberOfInstallments = 2,
                    CompletionAmount = 30
                },
                new Commitment
                {
                    EmployerAccountId =1,
                    ApprenticeshipId = 3,
                    LearnerId = 4,
                    StartDate = DateTime.Today,
                    PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(5),
                    MonthlyInstallment = 10,
                    NumberOfInstallments = 5,
                    CompletionAmount = 50
                }
            }, Moqer.GetMock<IEventPublisher>().Object,
            new CommitmentValidator());
            Assert.AreEqual(0, employerCommitments.GetTotalCompletionPayments(DateTime.Today).Item1);
            Assert.AreEqual(30, employerCommitments.GetTotalCompletionPayments(DateTime.Today.AddMonths(3)).Item1);
            Assert.AreEqual(50, employerCommitments.GetTotalCompletionPayments(DateTime.Today.AddMonths(6)).Item1);
        }
        [Test]
        public void Completion_Payments_Are_Included_In_Month_After_Planned_End_Date()
        {
            var employerCommitments = new EmployerCommitments(1, new List<Commitment>
            {
                new Commitment
                {
                    Id = 1,
                    EmployerAccountId =1,
                    ApprenticeshipId = 2,
                    LearnerId = 3,
                    StartDate = DateTime.Today,
                    PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(2),
                    MonthlyInstallment = 10,
                    NumberOfInstallments = 2,
                    CompletionAmount = 30
                },
                new Commitment
                {
                    Id = 2,
                    EmployerAccountId =1,
                    ApprenticeshipId = 3,
                    LearnerId = 4,
                    StartDate = DateTime.Today,
                    PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(5),
                    MonthlyInstallment = 10,
                    NumberOfInstallments = 5,
                    CompletionAmount = 50
                }
            }, Moqer.GetMock<IEventPublisher>().Object,
            new CommitmentValidator());

            Assert.IsFalse(employerCommitments.GetTotalCompletionPayments(DateTime.Today.AddMonths(2)).Item2.Any());
            Assert.IsTrue(employerCommitments.GetTotalCompletionPayments(DateTime.Today.AddMonths(3)).Item2.All(id => id == 1));
            Assert.IsTrue(employerCommitments.GetTotalCompletionPayments(DateTime.Today.AddMonths(6)).Item2.All(id => id == 2));
        }
    }
}