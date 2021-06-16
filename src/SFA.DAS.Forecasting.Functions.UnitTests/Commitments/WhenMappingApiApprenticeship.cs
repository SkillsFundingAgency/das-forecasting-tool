using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Commitments.Functions.Application;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Models.Payments;
using System;
using System.Threading.Tasks;
using ApiApprenticeship = SFA.DAS.Commitments.Api.Types.Apprenticeship.Apprenticeship;

namespace SFA.DAS.Forecasting.Functions.UnitTests.Commitments
{
    [TestFixture]
    public class WhenMappingApiApprenticeship
    {
        private Mock<IApprenticeshipCourseDataService> _apprenticeshipCourseDataServiceMock;
        private Mapper _sut;
        private IFixture _fixture = new Fixture();

        [SetUp]
        public void SetUp()
        {
            // Temporary while divide by zero exists
            _fixture.Customize<ApiApprenticeship>(
                x => x
                .Without(y => y.EndDate)
                .Without(y => y.StartDate)
                .Do(o =>
            {
                o.StartDate = _fixture.Create<DateTime>();
                o.EndDate = o.StartDate.Value.AddMonths(13);
            }));

            _apprenticeshipCourseDataServiceMock = new Mock<IApprenticeshipCourseDataService>();
            _sut = new Mapper(_apprenticeshipCourseDataServiceMock.Object);
        }

        [Test]
        public async Task IfNull_ReturnEmptyMessage()
        {
            // Arrange
            var emptyResult = new ApprenticeshipMessage();
            ApiApprenticeship apiApprenticeship = null;

            // Act
            var result = await _sut.Map(apiApprenticeship);

            // Assert
            result.Should().BeEquivalentTo(emptyResult);
        }

        [Test]
        public async Task NotNull_ShouldGetApprenticeshipTrainingCourse()
        {
            // Arrange
            ApiApprenticeship apiApprenticeship = _fixture.Create<ApiApprenticeship>();

            // Act
            var result = await _sut.Map(apiApprenticeship);

            // Assert
            _apprenticeshipCourseDataServiceMock
                .Verify(mock => mock.GetApprenticeshipCourse(It.Is<string>(x => x == apiApprenticeship.TrainingCode)), Times.Once);
        }

        [Test]
        public async Task ShouldMapToApprenticeshipMessage()
        {
            // Arrange
            ApiApprenticeship apiApprenticeship = _fixture.Create<ApiApprenticeship>();

            // Act
            var result = await _sut.Map(apiApprenticeship);

            // Assert
            result.EmployerAccountId.Should().Be(apiApprenticeship.EmployerAccountId);
            result.SendingEmployerAccountId.Should().Be(apiApprenticeship.TransferSenderId);
            result.ProviderId.Should().Be(apiApprenticeship.ProviderId);
            result.ProviderName.Should().Be(apiApprenticeship.ProviderName);
            result.ApprenticeshipId.Should().Be(apiApprenticeship.Id);
            result.CourseName.Should().Be(apiApprenticeship.TrainingName);
            result.StartDate.Should().Be(apiApprenticeship.StartDate.Value);
            result.PlannedEndDate.Should().Be(apiApprenticeship.EndDate.Value);
            result.ActualEndDate.Should().Be(null);
        }

        [Test]
        [TestCase(null, 0)]
        [TestCase("string", 0)]
        [TestCase("1234", 1234)]
        public async Task ShouldMapTApprenticeshipMessage_LearnerId(string apprenticeshipUln, long expectedLearnerId)
        {
            // Arrange
            ApiApprenticeship apiApprenticeship = _fixture
                .Build<ApiApprenticeship>()
                .With(x => x.ULN, apprenticeshipUln)
                .Create();

            // Act
            var result = await _sut.Map(apiApprenticeship);

            // Assert
            result.LearnerId.Should().Be(expectedLearnerId);
        }

        [Test]
        public async Task ShouldMapToApprenticeshipMessage_ApprenticeName()
        {
            // Arrange
            var firstName = "foo";
            var lastName = "bar";
            var expectedApprenticeName = "foo bar";

            ApiApprenticeship apiApprenticeship = _fixture
                .Build<ApiApprenticeship>()
                .With(x => x.FirstName, firstName)
                .With(x => x.LastName, lastName)
                .Create();

            // Act
            var result = await _sut.Map(apiApprenticeship);

            // Assert
            result.ApprenticeName.Should().Be(expectedApprenticeName);
        }

        [Test]
        public async Task NullApprenticeshipCourse_ShouldMapToApprenticeshipMessage_CourseLevel_Zero()
        {
            // Arrange
            ApiApprenticeship apiApprenticeship = _fixture.Create<ApiApprenticeship>();

            // Act
            var result = await _sut.Map(apiApprenticeship);

            // Assert
            result.CourseLevel.Should().Be(0);
        }

        [Test]
        [TestCase(99, 99)]
        [TestCase(null, 0)]
        public async Task ShouldMapToApprenticeshipMessage_CourseLevel(int? apiCourseLevel, int expectedMappedCourseLevel)
        {
            // Arrange
            ApiApprenticeship apiApprenticeship = _fixture.Create<ApiApprenticeship>();
            var apiCourse = _fixture
                .Build<ApprenticeshipCourse>()
                .With(x => x.Level, apiCourseLevel)
                .Create();

            _apprenticeshipCourseDataServiceMock
                .Setup(mock => mock.GetApprenticeshipCourse(It.Is<string>(x => x == apiApprenticeship.TrainingCode)))
                .ReturnsAsync(apiCourse);

            // Act
            var result = await _sut.Map(apiApprenticeship);

            // Assert
            result.CourseLevel.Should().Be(expectedMappedCourseLevel);
        }

        [Test]
        [TestCase(100, 20)]
        [TestCase(5000, 1000)]
        public async Task ShouldMapToApprenticeshipMessage_CompletionAmount(decimal? apprenticeshipCost, int expectedCompletionAmount)
        {
            // Arrange
            ApiApprenticeship apiApprenticeship = _fixture
                .Build<ApiApprenticeship>()
                .With(o => o.Cost, apprenticeshipCost)
                .Create();

            // Act
            var result = await _sut.Map(apiApprenticeship);

            // Assert
            result.CompletionAmount.Should().Be(expectedCompletionAmount);
        }

        [Test]
        [TestCase(100, 6, 13.33)]
        [TestCase(100, 18, 4.44)]
        [TestCase(100, 36, 2.22)]
        [TestCase(5000, 5, 800)]
        [TestCase(5000, 17, 235.29)]
        [TestCase(5000, 35, 114.29)]
        public async Task ShouldMapToApprenticeshipMessage_MonthlyInstallment(decimal? apprenticeshipCost, int duration, decimal expectedMonthlyInstallments)
        {
            // Arrange
            _fixture.Customizations.Clear();

            ApiApprenticeship apiApprenticeship = _fixture
                .Build<ApiApprenticeship>()
                .With(y => y.Cost, apprenticeshipCost)
                .Without(y => y.EndDate)
                .Without(y => y.StartDate)
                .Do(o =>
                {
                    o.StartDate = _fixture.Create<DateTime>();
                    o.EndDate = o.StartDate.Value.AddMonths(duration);
                })
                .Create();
            // Act
            var result = await _sut.Map(apiApprenticeship);

            // Assert
            result.MonthlyInstallment.Should().BeApproximately(expectedMonthlyInstallments, 0.01m);
        }

        [Test]
        [TestCase(6)]
        [TestCase(18)]
        [TestCase(36)]
        [TestCase(5)]
        [TestCase(17)]
        [TestCase(35)]
        public async Task ShouldMapToApprenticeshipMessage_NumberOfInstallments(int duration)
        {
            // Arrange
            _fixture.Customizations.Clear();

            ApiApprenticeship apiApprenticeship = _fixture
                .Build<ApiApprenticeship>()
                .Without(y => y.EndDate)
                .Without(y => y.StartDate)
                .Do(o =>
                {
                    o.StartDate = _fixture.Create<DateTime>();
                    o.EndDate = o.StartDate.Value.AddMonths(duration);
                })
                .Create();
            // Act
            var result = await _sut.Map(apiApprenticeship);

            // Assert
            result.NumberOfInstallments.Should().Be(duration);
        }

        [Test]
        [TestCase(null, FundingSource.Levy)]
        [TestCase(1234, FundingSource.Transfer)]
        public async Task ShouldMapToApprenticeshipMessage_FundingSource(long? transferSenderId, FundingSource fundingSource)
        {
            // Arrange
            ApiApprenticeship apiApprenticeship = _fixture
                .Build<ApiApprenticeship>()
                .With(o => o.TransferSenderId, transferSenderId)
                .Create();

            // Act
            var result = await _sut.Map(apiApprenticeship);

            // Assert
            result.FundingSource.Should().Be(fundingSource);
        }
    }
}
