using System;
using AutoMoq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Commitments.Validation;
using SFA.DAS.Forecasting.Models.Commitments;

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
            _moqer.SetInstance<CommitmentModel>(_existingCommitment);
            _moqer.GetMock<ICommitmentValidator>()
                .Setup(validator => validator.IsValid(It.IsAny<CommitmentModel>()))
                .Returns(true);
        }

        [Test]
        public void Updates_Existing_Commitment()
        {
            var commitment = _moqer.Resolve<EmployerCommitment>();
            Assert.IsTrue( commitment.RegisterCommitment(3, "test apprentice", "test course", 1, 4, "test provider", DateTime.Today,
                DateTime.Today.AddDays(1), null, 87.27m, 240, 12, 55501));

            Assert.AreEqual(commitment.ApprenticeName, "test apprentice");
            Assert.AreEqual(commitment.ProviderName, "test provider");
            Assert.AreEqual(commitment.CourseName, "test course");
            Assert.AreEqual(commitment.ProviderId, 4);
            Assert.AreEqual(commitment.LearnerId, 3);
            Assert.AreEqual(commitment.CourseLevel, 1);
            Assert.AreEqual(commitment.SendingEmployerAccountId, 55501);
        }
    }
}