using System;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Commitments
{
    public class WhenRegisteringACommitment
    {
        [Test]
        public void Then_If_No_EmployerAccountId_Has_Been_Supplied_False_Is_Returned()
        {
            //Arrange
            var commitment = new CommitmentModel
            {
                EmployerAccountId = 0,
                ApprenticeName = "Mr Test Tester"
            };
            var employerCommitment = new EmployerCommitment(new CommitmentModel());

            //Act
            var actual = employerCommitment.RegisterCommitment(commitment);

            //Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void Then_If_It_Is_A_New_Commitment_And_An_ActualEndDate_Has_Been_Supplied_False_Is_Returned()
        {
            //Arrange
            var commitment = new CommitmentModel
            {
                EmployerAccountId = 12345,
                ApprenticeName = "Mr Test Tester",
                ActualEndDate = DateTime.Today
            };
            var employerCommitment = new EmployerCommitment(new CommitmentModel());

            //Act
            var actual = employerCommitment.RegisterCommitment(commitment);

            //Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void Then_If_It_Is_A_New_Commitment_With_An_AcutalEndDate_Then_False_Is_Returned()
        {
            //Arrange
            var commitment = new CommitmentModel
            {
                Id = 554433,
                ActualEndDate = DateTime.Today
            };
            var employerCommitment = new EmployerCommitment(new CommitmentModel());

            //Act
            var actual = employerCommitment.RegisterCommitment(commitment);

            //Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void Then_If_It_Is_An_Existing_Commitment_And_The_Allowed_Updated_Values_Are_Changed_True_Is_Retruned()
        {
            //Arrange
            var commitment = new CommitmentModel
            {
                EmployerAccountId = 12345,
                ApprenticeName = "Mr Test Tester",
                ApprenticeshipId = 88775544,
                ActualEndDate = DateTime.Today.AddMonths(11),
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.AddMonths(12),
                MonthlyInstallment = 10m,
                CompletionAmount = 100m,
                NumberOfInstallments = 12
            };
            var employerCommitment = new EmployerCommitment(new CommitmentModel{Id = 65421});

            //Act
            var actual = employerCommitment.RegisterCommitment(commitment);

            //Assert
            Assert.IsTrue(actual);
            Assert.AreEqual(commitment.ApprenticeName, employerCommitment.ApprenticeName);
            Assert.AreEqual(commitment.ApprenticeshipId, employerCommitment.ApprenticeshipId);
            Assert.AreEqual(commitment.ActualEndDate, employerCommitment.ActualEndDate);
            Assert.AreEqual(commitment.StartDate, employerCommitment.StartDate);
            Assert.AreEqual(commitment.PlannedEndDate, employerCommitment.PlannedEndDate);
            Assert.AreEqual(commitment.MonthlyInstallment, employerCommitment.MonthlyInstallment);
            Assert.AreEqual(commitment.CompletionAmount, employerCommitment.CompletionAmount);
            Assert.AreEqual(commitment.NumberOfInstallments, employerCommitment.NumberOfInstallments);
        }

        [Test]
        public void Then_If_The_AcutalEndDate_ApprenticeshipName_Or_ApprenticeshipId_Have_Not_Changed_False_Is_Returned()
        {
            //Arrange
            var commitment = new CommitmentModel
            {
                Id = 554433,
                EmployerAccountId = 12345,
                ApprenticeName = "Tester",
                ApprenticeshipId = 774411,
                ActualEndDate = DateTime.Today

            };
            var employerCommitment = new EmployerCommitment(commitment);

            //Act
            var actual = employerCommitment.RegisterCommitment(commitment);

            //Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void Then_The_Values_Are_Mapped_And_Updated_If_Valid()
        {
            //Arrange
            var commitment = new CommitmentModel
            {
                Id = 554433,
                ApprenticeName = "Tester",
                ApprenticeshipId = 774411,
                CompletionAmount = 5432m,
                CourseLevel = 3,
                CourseName = "Test"
,               EmployerAccountId = 55443,
                FundingSource = FundingSource.Transfer,
                LearnerId = 12345,
                MonthlyInstallment = 10m,
                NumberOfInstallments = 10,
                PlannedEndDate = DateTime.Today.AddMonths(12),
                StartDate = DateTime.Today,
                ProviderId = 443322,
                ProviderName = "Test Provider",
                SendingEmployerAccountId = 12233
            };
            var employerCommitment = new EmployerCommitment(new CommitmentModel());

            //Act
            var actual = employerCommitment.RegisterCommitment(commitment);

            //Assert
            Assert.IsTrue(actual);
            Assert.AreEqual(commitment.ActualEndDate, employerCommitment.ActualEndDate);
            Assert.AreEqual(commitment.ApprenticeName, employerCommitment.ApprenticeName);
            Assert.AreEqual(commitment.ApprenticeshipId, employerCommitment.ApprenticeshipId);
            Assert.AreEqual(commitment.CompletionAmount, employerCommitment.CompletionAmount);
            Assert.AreEqual(commitment.CourseLevel, employerCommitment.CourseLevel);
            Assert.AreEqual(commitment.CourseName, employerCommitment.CourseName);
            Assert.AreEqual(commitment.EmployerAccountId, employerCommitment.EmployerAccountId);
            Assert.AreEqual(commitment.FundingSource, employerCommitment.FundingSource);
            Assert.AreEqual(commitment.LearnerId, employerCommitment.LearnerId);
            Assert.AreEqual(commitment.MonthlyInstallment, employerCommitment.MonthlyInstallment);
            Assert.AreEqual(commitment.NumberOfInstallments, employerCommitment.NumberOfInstallments);
            Assert.AreEqual(commitment.PlannedEndDate, employerCommitment.PlannedEndDate);
            Assert.AreEqual(commitment.StartDate, employerCommitment.StartDate);
            Assert.AreEqual(commitment.ProviderId, employerCommitment.ProviderId);
            Assert.AreEqual(commitment.ProviderName, employerCommitment.ProviderName);
            Assert.AreEqual(commitment.SendingEmployerAccountId, employerCommitment.SendingEmployerAccountId);
            
        }
    }
}
