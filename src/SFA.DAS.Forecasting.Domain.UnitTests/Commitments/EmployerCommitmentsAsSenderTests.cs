using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Core;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Extensions;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Commitments
{
    [TestFixture]
    public class EmployerCommitmentsAsSenderTests
    {
        protected  EmployerCommitmentsModel Commitments;
        private DateTime startDate;
        private DateTime endDate;

        [SetUp]
        public void SetUp()
        {
            endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddYears(1);
            startDate = endDate.AddYears(-1);
            Commitments = new EmployerCommitmentsModel();
        }

        private EmployerCommitments GetEmployerCommitments(long employerAccountId = 1) =>
            new EmployerCommitments(employerAccountId, Commitments);

        Func<EmployerCommitments, int, CostOfTraining> TotalCostOfTraining = (cmts, m) =>
                cmts.GetTotalCostOfTraining(DateTime.Today.GetStartOfMonth().AddMonths(m));

        Func<EmployerCommitments, int, CompletionPayments> TotalCompletionPayments = (cmts, m) =>
                cmts.GetTotalCompletionPayments(DateTime.Today.GetStartOfMonth().AddMonths(m));

        [Test]
        public void Includes_Installments_for_each_month()
        {
            Commitments.SendingEmployerTransferCommitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1003,
                SendingEmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = startDate,
                PlannedEndDate = endDate,
                MonthlyInstallment = 800,
                NumberOfInstallments = 12,
                CompletionAmount = 6000,
                FundingSource = Models.Payments.FundingSource.Transfer
            });

            var employerCommitments = GetEmployerCommitments();
            var date = startDate.AddMonths(1);
            while (date <= endDate)
            {
                var costOfTraining = employerCommitments.GetTotalCostOfTraining(date);
                costOfTraining.TransferOut.Should().Be(800);
                date = date.AddMonths(1);
            }
        }

        [Test]
        public void If_Last_Day_Of_Month_Last_Payment_Is_Made_In_Month_After_Planned_End_Date()
        {
            endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
            startDate = endDate.AddYears(-1);

            Commitments.SendingEmployerTransferCommitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1003,
                SendingEmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = startDate,
                PlannedEndDate = endDate,
                MonthlyInstallment = 1000,
                NumberOfInstallments = 12,
                CompletionAmount = 6000,
                FundingSource = Models.Payments.FundingSource.Transfer
            });

            var employerCommitments = GetEmployerCommitments();
            
            TotalCostOfTraining(employerCommitments, 1).TransferOut.Should().Be(1000, "Should calculate for next month");
            TotalCostOfTraining(employerCommitments, 2).TransferOut.Should().Be(0, "No cost after commitment finished");
        }

        [Test]
        public void Aggregates_Correct_Installments_In_Month()
        {
            Commitments.SendingEmployerTransferCommitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1003,
                SendingEmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(2),
                MonthlyInstallment = 10,
                NumberOfInstallments = 2,
                FundingSource = Models.Payments.FundingSource.Transfer
            });
            Commitments.SendingEmployerTransferCommitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1003,
                SendingEmployerAccountId = 1,
                ApprenticeshipId = 3,
                LearnerId = 4,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(5),
                MonthlyInstallment = 15,
                NumberOfInstallments = 5,
                FundingSource = Models.Payments.FundingSource.Transfer
            });
            Commitments.SendingEmployerTransferCommitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1003,
                SendingEmployerAccountId = 1,
                ApprenticeshipId = 4,
                LearnerId = 5,
                StartDate = DateTime.Today.AddMonths(3),
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(12),
                MonthlyInstallment = 20,
                NumberOfInstallments = 10,
                FundingSource = Models.Payments.FundingSource.Transfer
            });

            var employerCommitments = GetEmployerCommitments();
            

            TotalCostOfTraining(employerCommitments, 0).TransferOut.Should().Be(0);
            TotalCostOfTraining(employerCommitments, 1).TransferOut.Should().Be(25);
            TotalCostOfTraining(employerCommitments, 2).TransferOut.Should().Be(25);
            TotalCostOfTraining(employerCommitments, 3).TransferOut.Should().Be(15);
            TotalCostOfTraining(employerCommitments, 4).TransferOut.Should().Be(35);
            TotalCostOfTraining(employerCommitments, 6).TransferOut.Should().Be(20);
        }

        [Test]
        public void Get_Total_Cost_Of_Training_Includes_Is_Included_On_Planned_End_Date()
        {
            Commitments.SendingEmployerTransferCommitments.Add(new CommitmentModel
            {
                EmployerAccountId = 1003,
                SendingEmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = DateTime.Today.AddMonths(-1),
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(2),
                MonthlyInstallment = 10,
                NumberOfInstallments = 2,
                FundingSource = Models.Payments.FundingSource.Transfer
            });
            var employerCommitments = GetEmployerCommitments();

            TotalCostOfTraining(employerCommitments, 2).TransferOut.Should().Be(10);
        }

        // Completion Payments

        [Test]
        public void Completion_Payments_Are_Aggregated_In_Month_After_Planned_End_Date()
        {
            Commitments.SendingEmployerTransferCommitments.Add(new CommitmentModel
            {
                EmployerAccountId = 999,
                SendingEmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(2),
                MonthlyInstallment = 10,
                NumberOfInstallments = 2,
                CompletionAmount = 30,
                FundingSource = Models.Payments.FundingSource.Transfer
            });
            Commitments.SendingEmployerTransferCommitments.Add(new CommitmentModel
            {
                EmployerAccountId = 999,
                SendingEmployerAccountId = 1,
                ApprenticeshipId = 3,
                LearnerId = 4,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(5),
                MonthlyInstallment = 10,
                NumberOfInstallments = 5,
                CompletionAmount = 50,
                FundingSource = Models.Payments.FundingSource.Transfer
            });
            var employerCommitments = GetEmployerCommitments();

            TotalCompletionPayments(employerCommitments, 0).TransferOutCompletionPayment.Should().Be(0);
            TotalCompletionPayments(employerCommitments, 3).TransferOutCompletionPayment.Should().Be(30);
            TotalCompletionPayments(employerCommitments, 6).TransferOutCompletionPayment.Should().Be(50);
        }
    }
}