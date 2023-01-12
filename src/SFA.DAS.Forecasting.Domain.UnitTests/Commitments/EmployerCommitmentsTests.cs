using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Core;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Extensions;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Commitments
{
    [TestFixture]
    public class EmployerCommitmentsTests
    {
        private EmployerCommitmentsModel _commitments;
        private DateTime _startDate;
        private DateTime _endDate;

        [SetUp]
        public void SetUp()
        {
            _endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddYears(1);
            _startDate = _endDate.AddYears(-1);
            _commitments = new EmployerCommitmentsModel(); 
        }

        private EmployerCommitments GetEmployerCommitments(long employerAccountId = 1) =>
            new EmployerCommitments(employerAccountId, _commitments);

        [Test]
        public void Includes_Installment_For_Each_Month_Including_Planned_End_Date()
        {
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = _startDate,
                PlannedEndDate = _endDate,
                MonthlyInstallment = 1000,
                NumberOfInstallments = 12,
                CompletionAmount = 6000,
                FundingSource = Models.Payments.FundingSource.Levy
            });
            var employerCommitments = GetEmployerCommitments();
            var date = _startDate.AddMonths(1);
            while (date <= _endDate)
            {
                var costOfTraining = employerCommitments.GetTotalCostOfTraining(date);
                Assert.AreEqual(1000, costOfTraining.LevyFunded, $"Invalid total cost of training for: {date:dd/MM/yyyy}, expected £1000 but was £{costOfTraining}");
                date = date.AddMonths(1);
            }

            var costOfTrainingCompletionMonth = employerCommitments.GetTotalCostOfTraining(_startDate);
            Assert.AreEqual(0, costOfTrainingCompletionMonth.LevyFunded, $"Invalid total cost of training for: {_startDate:dd/MM/yyyy}, expected £0 but was £{costOfTrainingCompletionMonth}");
        }

        [Test]
        public void If_Last_Day_Of_Month_Last_Payment_Is_Made_In_Month_After_Planned_End_Date()
        {
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = _startDate,
                PlannedEndDate = _endDate,
                MonthlyInstallment = 1000,
                NumberOfInstallments = 12,
                CompletionAmount = 6000,
                FundingSource = Models.Payments.FundingSource.Levy
            });
            
            _endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
            _startDate = _endDate.AddYears(-1);
            var employerCommitments = GetEmployerCommitments();
            var costOfTraining = employerCommitments.GetTotalCostOfTraining(_endDate.AddMonths(1));
            Assert.AreEqual(1000, costOfTraining.LevyFunded, $"Invalid total cost of training for: {_endDate.AddMonths(1):dd/MM/yyyy}, expected £1000 but was £{costOfTraining}");
        }

        [Test]
        public void Aggregates_Correct_Installments_In_Month()
        {
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(2),
                MonthlyInstallment = 10,
                NumberOfInstallments = 2,
                FundingSource = Models.Payments.FundingSource.Levy
            });
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 3,
                LearnerId = 4,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(5),
                MonthlyInstallment = 15,
                NumberOfInstallments = 5,
                FundingSource = Models.Payments.FundingSource.Levy
            });
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 4,
                LearnerId = 5,
                StartDate = DateTime.Today.AddMonths(3),
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(12),
                MonthlyInstallment = 20,
                NumberOfInstallments = 10,
                FundingSource = Models.Payments.FundingSource.Levy
            });
            _commitments.SendingEmployerTransferCommitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 4,
                LearnerId = 5,
                StartDate = DateTime.Today.AddMonths(3),
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(12),
                MonthlyInstallment = 20,
                NumberOfInstallments = 10,
                FundingSource = Models.Payments.FundingSource.Transfer
            });

            var employerCommitments = GetEmployerCommitments();

            Assert.AreEqual(0, employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth()).LevyFunded);
            Assert.AreEqual(25, employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(1)).LevyFunded);
            Assert.AreEqual(25, employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(2)).LevyFunded);
            Assert.AreEqual(15, employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(3)).LevyFunded);
            Assert.AreEqual(35, employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(4)).LevyFunded);
            Assert.AreEqual(20, employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(6)).LevyFunded);
        }

        [Test]
        public void Aggregates_Correct_Installments_In_Month_For_Transfer()
        {
            _commitments.ReceivingEmployerTransferCommitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1,
                SendingEmployerAccountId = 999,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(2),
                MonthlyInstallment = 10,
                NumberOfInstallments = 2,
                FundingSource = Models.Payments.FundingSource.Transfer
            });
            _commitments.ReceivingEmployerTransferCommitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1,
                SendingEmployerAccountId = 999,
                ApprenticeshipId = 3,
                LearnerId = 4,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(5),
                MonthlyInstallment = 15,
                NumberOfInstallments = 5,
                FundingSource = Models.Payments.FundingSource.Transfer
            });
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 4,
                LearnerId = 5,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(12),
                MonthlyInstallment = 20,
                NumberOfInstallments = 10,
                FundingSource = Models.Payments.FundingSource.Levy
            });

            var employerCommitments = GetEmployerCommitments();

            employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(2))
                .TransferIn.Should().Be(25);

            employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(5))
                .TransferIn.Should().Be(15);

            employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(12))
                .TransferIn.Should().Be(0);

            employerCommitments.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(12))
                .LevyFunded.Should().Be(20);
        }

        [Test]
        public void Includes_Correct_Installment_Commitments_In_Month()
        {
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
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
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
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
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
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
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(2),
                MonthlyInstallment = 10,
                NumberOfInstallments = 2,
                FundingSource = Models.Payments.FundingSource.Levy
            });

            _commitments.ReceivingEmployerTransferCommitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(2),
                MonthlyInstallment = 15,
                NumberOfInstallments = 2,
                FundingSource = Models.Payments.FundingSource.Transfer
            });

            var employerCommitments = GetEmployerCommitments();
            Assert.AreEqual(10, employerCommitments.GetTotalCostOfTraining(DateTime.Today.AddMonths(2)).LevyFunded);
            Assert.AreEqual(15, employerCommitments.GetTotalCostOfTraining(DateTime.Today.AddMonths(2)).TransferIn);
        }

        [Test]
        public void Completion_Payments_Are_Aggregated_In_Month_After_Planned_End_Date()
        {
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(2),
                MonthlyInstallment = 10,
                NumberOfInstallments = 2,
                CompletionAmount = 30,
                FundingSource = Models.Payments.FundingSource.Levy
            });
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 3,
                LearnerId = 4,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(5),
                MonthlyInstallment = 10,
                NumberOfInstallments = 5,
                CompletionAmount = 50,
                FundingSource = Models.Payments.FundingSource.Levy
            });

            var employerCommitments = GetEmployerCommitments();
            Assert.AreEqual(0, employerCommitments.GetTotalCompletionPayments(DateTime.Today).LevyFundedCompletionPayment);
            Assert.AreEqual(30, employerCommitments.GetTotalCompletionPayments(DateTime.Today.AddMonths(3)).LevyFundedCompletionPayment);
            Assert.AreEqual(50, employerCommitments.GetTotalCompletionPayments(DateTime.Today.AddMonths(6)).LevyFundedCompletionPayment);
        }

        [Test]
        public void Completion_Payments_Are_Included_In_Month_After_Planned_End_Date()
        {
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
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
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
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

            Assert.IsFalse(employerCommitments.GetTotalCompletionPayments(DateTime.Today.AddMonths(2)).CommitmentIds.Any());
            Assert.IsTrue(employerCommitments.GetTotalCompletionPayments(DateTime.Today.AddMonths(3)).CommitmentIds.All(id => id == 1));
            Assert.IsTrue(employerCommitments.GetTotalCompletionPayments(DateTime.Today.AddMonths(6)).CommitmentIds.All(id => id == 2));
        }

        [Test]
        public void Get_Unallocated_Completion_Amount_Ignores_Completions_Included_In_The_Projection()
        {
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
            {
                Id = 1,
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = DateTime.Today.AddMonths(-10),
                PlannedEndDate = DateTime.Today.GetStartOfMonth(),
                MonthlyInstallment = 10,
                NumberOfInstallments = 2,
                CompletionAmount = 30
            });
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
            {
                Id = 2,
                EmployerAccountId = 1,
                ApprenticeshipId = 3,
                LearnerId = 4,
                StartDate = DateTime.Today.AddMonths(-10),
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(-1),
                MonthlyInstallment = 10,
                NumberOfInstallments = 5,
                CompletionAmount = 50
            });

            var employerCommitments = GetEmployerCommitments();
            Assert.AreEqual(0, employerCommitments.GetUnallocatedCompletionAmount());
        }

        [Test]
        public void Get_Unallocated_Completion_Amount_Includes_Completions_That_Have_Ended_A_Month_Or_More_Before_The_Start_Of_The_Projection_And_Unpaid_Last_Month()
        {
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
            {
                Id = 1,
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                HasHadPayment = false,
                StartDate = DateTime.Today.AddMonths(-10),
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(-1),
                MonthlyInstallment = 10,
                NumberOfInstallments = 2,
                FundingSource = FundingSource.Levy,
                CompletionAmount = 100
            });
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
            {
                Id = 2,
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                HasHadPayment = true,
                StartDate = DateTime.Today.AddMonths(-10),
                ActualEndDate = DateTime.Today.GetStartOfMonth().AddMonths(-1),
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(-1),
                MonthlyInstallment = 10,
                NumberOfInstallments = 2,
                FundingSource = FundingSource.Levy,
                CompletionAmount = 100
            });
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
            {
                Id = 2,
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                HasHadPayment = true,
                StartDate = DateTime.Today.AddMonths(-10),
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(-1),
                MonthlyInstallment = 10,
                NumberOfInstallments = 2,
                FundingSource = FundingSource.Levy,
                CompletionAmount = 100
            });
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
            {
                Id = 3,
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                HasHadPayment = true,
                StartDate = DateTime.Today.AddMonths(-10),
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(1),
                MonthlyInstallment = 10,
                NumberOfInstallments = 2,
                FundingSource = FundingSource.Levy,
                CompletionAmount = 100
            });
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
            {
                Id = 4,
                EmployerAccountId = 1,
                ApprenticeshipId = 3,
                LearnerId = 4,
                HasHadPayment = true,
                StartDate = DateTime.Today.AddMonths(-10),
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(-2),
                MonthlyInstallment = 10,
                NumberOfInstallments = 5,
                FundingSource = FundingSource.Levy,
                CompletionAmount = 100
            });
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
            {
                Id = 5,
                EmployerAccountId = 1,
                ApprenticeshipId = 3,
                LearnerId = 4,
                HasHadPayment = true,
                StartDate = DateTime.Today.AddMonths(-10),
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(-3),
                MonthlyInstallment = 10,
                NumberOfInstallments = 5,
                CompletionAmount = 100, 
                FundingSource = FundingSource.Levy
            });
           
            var employerCommitments = GetEmployerCommitments();
            Assert.AreEqual(100m, employerCommitments.GetUnallocatedCompletionAmount(true));
        }
        [Test]
        public void Get_Unallocated_Completion_Amount_Includes_Completions_That_Have_Ended_A_Month_Or_More_Before_The_Start_Of_The_Projection()
        {
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
            {
                Id = 1,
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = DateTime.Today.AddMonths(-10),
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(-1),
                MonthlyInstallment = 10,
                NumberOfInstallments = 2,
                FundingSource = FundingSource.Levy,
                CompletionAmount = 100
            });
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
            {
                Id = 2,
                EmployerAccountId = 1,
                ApprenticeshipId = 3,
                LearnerId = 4,
                StartDate = DateTime.Today.AddMonths(-10),
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(-2),
                MonthlyInstallment = 10,
                NumberOfInstallments = 5,
                FundingSource = FundingSource.Levy,
                CompletionAmount = 100
            });
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
            {
                Id = 2,
                EmployerAccountId = 1,
                ApprenticeshipId = 3,
                LearnerId = 4,
                StartDate = DateTime.Today.AddMonths(-10),
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(-3),
                MonthlyInstallment = 10,
                NumberOfInstallments = 5,
                CompletionAmount = 100,
                FundingSource = FundingSource.Levy
            });
            var employerCommitments = GetEmployerCommitments();
            Assert.AreEqual(200m, employerCommitments.GetUnallocatedCompletionAmount());
        }

        [Test]
        public void Gets_Correct_Unallocated_Completion_Amount()
        {
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
            {
                Id = 1,
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = DateTime.Today.AddMonths(-10),
                PlannedEndDate = DateTime.Today.GetStartOfMonth(),
                MonthlyInstallment = 10,
                NumberOfInstallments = 2,
                CompletionAmount = 30,
                FundingSource = FundingSource.Levy
            });
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
            {
                Id = 2,
                EmployerAccountId = 1,
                ApprenticeshipId = 3,
                LearnerId = 4,
                StartDate = DateTime.Today.AddMonths(-10),
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(-2),
                MonthlyInstallment = 10,
                NumberOfInstallments = 5,
                CompletionAmount = 50,
                FundingSource = FundingSource.Levy
            });
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
            {
                Id = 3,
                EmployerAccountId = 1,
                ApprenticeshipId = 4,
                LearnerId = 5,
                StartDate = DateTime.Today.AddMonths(-10),
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(-5),
                MonthlyInstallment = 10,
                NumberOfInstallments = 5,
                CompletionAmount = 50,
                FundingSource = FundingSource.Levy
            });
            _commitments.LevyFundedCommitments.Add(new CommitmentModel
            {
                Id = 4,
                EmployerAccountId = 1,
                ApprenticeshipId = 5,
                LearnerId = 6,
                StartDate = DateTime.Today.AddMonths(-10),
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(-1),
                MonthlyInstallment = 10,
                NumberOfInstallments = 2,
                CompletionAmount = 30,
                FundingSource = FundingSource.Levy
            });

            var employerCommitments = GetEmployerCommitments();
            Assert.AreEqual(100m, employerCommitments.GetUnallocatedCompletionAmount());
        }
    }
}