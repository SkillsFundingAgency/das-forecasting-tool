using System;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Commitments
{
    [TestFixture]
    public class EmployerCommitmentsPledgeTests
    {
        private EmployerCommitments _employerCommitments;
        private EmployerCommitmentsModel _employerCommitmentsModel;

        private DateTime _startDate;
        private DateTime _endDate;

        [SetUp]
        public void Setup()
        {
            _endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddYears(1);
            _startDate = _endDate.AddYears(-1);
            _employerCommitmentsModel = new EmployerCommitmentsModel();
            _employerCommitments = new EmployerCommitments(1, _employerCommitmentsModel);
        }

        private EmployerCommitments GetEmployerCommitments(long employerAccountId = 1) => new EmployerCommitments(employerAccountId, _employerCommitmentsModel);

        [Test]
        public void ApprovedPledgeApplicationCost_Is_Calculated_Correctly()
        {
            _employerCommitmentsModel.SendingEmployerTransferCommitments.Add(new CommitmentModel
            {
                StartDate = _startDate,
                PlannedEndDate = _endDate,
                MonthlyInstallment = 1000,
                NumberOfInstallments = 12,
                CompletionAmount = 6000,
                FundingSource = Models.Payments.FundingSource.ApprovedPledgeApplication
            });

            var employerCommitments = GetEmployerCommitments();
            var date = _startDate.AddMonths(1);
            while (date <= _endDate)
            {
                var costOfTraining = employerCommitments.GetTotalCostOfPledges(date);
                Assert.AreEqual(1000, costOfTraining.ApprovedPledgeApplicationCost, $"Invalid total cost of training for: {date:dd/MM/yyyy}, expected £1000 but was £{costOfTraining}");
                date = date.AddMonths(1);
            }

            var costOfTrainingCompletionMonth = employerCommitments.GetTotalCostOfPledges(date);
            Assert.AreEqual(6000, costOfTrainingCompletionMonth.ApprovedPledgeApplicationCompletionPayments, $"Invalid total cost of training for: {date.AddMonths(1):dd/MM/yyyy}, expected £0 but was £{costOfTrainingCompletionMonth}");
        }

        [Test]
        public void AcceptedPledgeApplicationCost_Is_Calculated_Correctly()
        {
            _employerCommitmentsModel.SendingEmployerTransferCommitments.Add(new CommitmentModel
            {
                StartDate = _startDate,
                PlannedEndDate = _endDate,
                MonthlyInstallment = 1000,
                NumberOfInstallments = 12,
                CompletionAmount = 6000,
                FundingSource = Models.Payments.FundingSource.AcceptedPledgeApplication
            });

            var employerCommitments = GetEmployerCommitments();
            var date = _startDate.AddMonths(1);
            while (date <= _endDate)
            {
                var costOfTraining = employerCommitments.GetTotalCostOfPledges(date);
                Assert.AreEqual(1000, costOfTraining.AcceptedPledgeApplicationCost, $"Invalid total cost of training for: {date:dd/MM/yyyy}, expected £1000 but was £{costOfTraining}");
                date = date.AddMonths(1);
            }

            var costOfTrainingCompletionMonth = employerCommitments.GetTotalCostOfPledges(date);
            Assert.AreEqual(6000, costOfTrainingCompletionMonth.AcceptedPledgeApplicationCompletionPayments, $"Invalid total cost of training for: {date.AddMonths(1):dd/MM/yyyy}, expected £0 but was £{costOfTrainingCompletionMonth}");
        }

        [Test]
        public void PledgeOriginatedPledgeApplicationCost_Is_Calculated_Correctly()
        {
            _employerCommitmentsModel.SendingEmployerTransferCommitments.Add(new CommitmentModel
            {
                StartDate = _startDate,
                PlannedEndDate = _endDate,
                MonthlyInstallment = 1000,
                NumberOfInstallments = 12,
                CompletionAmount = 6000,
                FundingSource = Models.Payments.FundingSource.Transfer,
                PledgeApplicationId = 123
            });

            var employerCommitments = GetEmployerCommitments();
            var date = _startDate.AddMonths(1);
            while (date <= _endDate)
            {
                var costOfTraining = employerCommitments.GetTotalCostOfPledges(date);
                Assert.AreEqual(1000, costOfTraining.PledgeOriginatedCommitmentCost, $"Invalid total cost of training for: {date:dd/MM/yyyy}, expected £1000 but was £{costOfTraining}");
                date = date.AddMonths(1);
            }

            var costOfTrainingCompletionMonth = employerCommitments.GetTotalCostOfPledges(date);
            Assert.AreEqual(6000, costOfTrainingCompletionMonth.PledgeOriginatedCommitmentCompletionPayments, $"Invalid total cost of training for: {date.AddMonths(1):dd/MM/yyyy}, expected £0 but was £{costOfTrainingCompletionMonth}");
        }
    }
}
