using AutoFixture;
using NUnit.Framework;
using SFA.DAS.Forecasting.Commitments.Functions.Application;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Functions.UnitTests.Commitments
{
    [TestFixture]
    public class WhenMappingPledgeApplication
    {
        private Mapper _mapper;
        private Fixture _fixture;
        private long _employerAccountId;
        private Models.Pledges.Application _source;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _employerAccountId = _fixture.Create<long>();
            _source = _fixture.Create<Models.Pledges.Application>();
            _mapper = new Mapper();
        }

        [Test]
        public void Should_Map_SendingEmployerAccountId()
        {
            var result = _mapper.Map(_source, _employerAccountId);
            Assert.AreEqual(_employerAccountId, result.SendingEmployerAccountId);
        }

        [Test]
        public void Should_Map_EmployerAccountId()
        {
            var result = _mapper.Map(_source, _employerAccountId);
            Assert.AreEqual(_source.EmployerAccountId, result.EmployerAccountId);
        }

        [Test]
        public void Should_Map_LearnerId()
        {
            var result = _mapper.Map(_source, _employerAccountId);
            Assert.AreEqual(0, result.LearnerId);
        }

        [Test]
        public void Should_Map_ProviderId()
        {
            var result = _mapper.Map(_source, _employerAccountId);
            Assert.AreEqual(0, result.ProviderId);
        }

        [Test]
        public void Should_Map_ProviderName()
        {
            var result = _mapper.Map(_source, _employerAccountId);
            Assert.AreEqual(string.Empty, result.ProviderName);
        }

        [Test]
        public void Should_Map_ApprenticeshipId()
        {
            var result = _mapper.Map(_source, _employerAccountId);
            Assert.AreEqual(0, result.ApprenticeshipId);
        }

        [Test]
        public void Should_Map_ApprenticeshipName()
        {
            var result = _mapper.Map(_source, _employerAccountId);
            Assert.AreEqual(string.Empty, result.ApprenticeName);
        }

        [Test]
        public void Should_Map_CourseName()
        {
            var result = _mapper.Map(_source, _employerAccountId);
            Assert.AreEqual(_source.StandardTitle, result.CourseName);
        }

        [Test]
        public void Should_Map_CourseLevel()
        {
            var result = _mapper.Map(_source, _employerAccountId);
            Assert.AreEqual(_source.StandardLevel, result.CourseLevel);
        }

        [Test]
        public void Should_Map_StartDate()
        {
            var result = _mapper.Map(_source, _employerAccountId);
            Assert.AreEqual(_source.StartDate, result.StartDate);
        }

        [Test]
        public void Should_Map_PlannedEndDate()
        {
            var result = _mapper.Map(_source, _employerAccountId);
            Assert.AreEqual(_source.StartDate.AddMonths(_source.StandardDuration), result.PlannedEndDate);
        }

        [Test]
        public void Should_Map_ActualEndDate_As_Null()
        {
            var result = _mapper.Map(_source, _employerAccountId);
            Assert.IsNull(result.ActualEndDate);
        }

        [Test]
        public void Should_Map_CompletionAmount()
        {
            var result = _mapper.Map(_source, _employerAccountId);
            Assert.AreEqual(_source.StandardMaxFunding * 0.2M, result.CompletionAmount);
        }

        [Test]
        public void Should_Map_MonthlyInstallment()
        {
            var result = _mapper.Map(_source, _employerAccountId);
            var expected = (_source.StandardMaxFunding * 0.8M) / _source.StandardDuration;
            Assert.AreEqual(expected, result.MonthlyInstallment);
        }

        [Test]
        public void Should_Map_NumberOfInstallments()
        {
            var result = _mapper.Map(_source, _employerAccountId);
            Assert.AreEqual(_source.StandardDuration, result.NumberOfInstallments);
        }

        [TestCase(Models.Pledges.ApplicationStatus.Accepted, FundingSource.AcceptedPledgeApplication)]
        [TestCase(Models.Pledges.ApplicationStatus.Approved, FundingSource.ApprovedPledgeApplication)]
        public void Should_Map_FundingSource(string status, FundingSource expectedFundingSource)
        {
            _source.Status = status;
            var result = _mapper.Map(_source, _employerAccountId);
            Assert.AreEqual(expectedFundingSource, result.FundingSource);
        }

        [Test]
        public void Should_Map_PledgeApplicationId()
        {
            var result = _mapper.Map(_source, _employerAccountId);
            Assert.AreEqual(_source.Id, result.PledgeApplicationId);
        }
    }
}
