using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Commitments.Functions.Application;
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
        public async Task NotNull_ShouldMapToApprenticeshipMessage()
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
        public async Task NotNull_ShouldMapToApprenticeshipMessageLearnerId(string apprenticeshipUln, long expectedLearnerId)
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
    }
}
