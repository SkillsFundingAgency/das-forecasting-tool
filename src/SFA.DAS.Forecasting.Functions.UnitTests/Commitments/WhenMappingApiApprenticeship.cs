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
    }
}
