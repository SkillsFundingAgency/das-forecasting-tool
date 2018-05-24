using AutoMoq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Commitments.Validation;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Commitments
{
    [TestFixture]
    public class EmployerCommitmentTests
    {
        private AutoMoqer _moqer;
        private CommitmentModel _existingCommitment;
        [SetUp]
        public void SetUp()
        {
            _moqer = new AutoMoqer();
            _existingCommitment = new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 0,
                ApprenticeName = "Test apprentice 12",
                ProviderName = "test provider 12",
                ProviderId = 1,
                MonthlyInstallment = 10,
                NumberOfInstallments = 1,
                CompletionAmount = 50,
                CourseLevel = 3,
            };
            _moqer.SetInstance(_existingCommitment);
            _moqer.GetMock<ICommitmentValidator>()
                .Setup(validator => validator.IsValid(It.IsAny<CommitmentModel>()))
                .Returns(true);
        }

        [Test]
        public void Updates_Existing_Commitment()
        {
            var commitment = _moqer.Resolve<EmployerCommitment>();

            var newCommitment = BuildCommitmentModel();

            commitment.RegisterCommitment(newCommitment).Should().BeTrue();

            commitment.ApprenticeName.Should().Be("test apprentice");
            commitment.ProviderName.Should().Be("test provider");
            commitment.CourseName.Should().Be("test course");
            commitment.ProviderId.Should().Be(4);
            commitment.LearnerId.Should().Be(3);
            commitment.CourseLevel.Should().Be(1);
            commitment.CompletionAmount.Should().Be(50);
            commitment.SendingEmployerAccountId.Should().Be(55501);
            commitment.FundingSource.Should().Be(FundingSource.Levy);
        }

        [Test]
        public void Should_not_update_if_id_0()
        {
            var commitment = _moqer.Resolve<EmployerCommitment>();

            _existingCommitment.EmployerAccountId = 0;

            var newCommitment = BuildCommitmentModel();

            commitment.RegisterCommitment(newCommitment).Should().BeFalse();

            commitment.ApprenticeName.Should().Be("Test apprentice 12");
        }

        private static CommitmentModel BuildCommitmentModel()
        {
            return new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                ApprenticeName = "test apprentice",
                ProviderName = "test provider",
                CourseName = "test course",
                ProviderId = 4,
                MonthlyInstallment = 10,
                NumberOfInstallments = 1,
                CompletionAmount = 50,
                CourseLevel = 1,
                FundingSource = FundingSource.Levy,
                SendingEmployerAccountId = 55501
            };
        }
    }
}