using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;

namespace SFA.DAS.Forecasting.Application.UnitTests.PledgesService
{
    [TestFixture]
    public class GetApplicationsTests
    {
        private Application.ApprenticeshipCourses.Services.PledgesService _pledgesService;
        private Mock<IApiClient> _apiClient;
        private readonly Fixture _fixture = new Fixture();
        private int _pledgeId;
        private GetApplicationsResponse _apiResponse;

        [SetUp]
        public void Setup()
        {
            _pledgeId = _fixture.Create<int>();

            _apiClient = new Mock<IApiClient>();

            _apiResponse = _fixture.Create<GetApplicationsResponse>();
            _apiClient.Setup(x => x.Get<GetApplicationsResponse>(It.Is<GetApplicationsApiRequest>(r => r.PledgeId == _pledgeId)))
                .ReturnsAsync(_apiResponse);

            _pledgesService =
                new Application.ApprenticeshipCourses.Services.PledgesService(_apiClient.Object, Mock.Of<ILogger<Application.ApprenticeshipCourses.Services.PledgesService>>());
        }

        [Test]
        public async Task GetApplications_Returns_Applications_For_Pledge()
        {
            var result = await _pledgesService.GetApplications(_pledgeId);

            Assert.AreEqual(_apiResponse.Applications.Count, result.Count);

            var i = 0;
            foreach (var expectedApplication in _apiResponse.Applications)
            {
                var actualApplication = _apiResponse.Applications[i];
                AssertEquality(expectedApplication, actualApplication);
                i++;
            }
        }

        private void AssertEquality(GetApplicationsResponse.Application expected, GetApplicationsResponse.Application actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.PledgeId, actual.PledgeId);
            Assert.AreEqual(expected.EmployerAccountId, actual.EmployerAccountId);
            Assert.AreEqual(expected.StandardId, actual.StandardId);
            Assert.AreEqual(expected.StandardTitle, actual.StandardTitle);
            Assert.AreEqual(expected.StandardLevel, actual.StandardLevel);
            Assert.AreEqual(expected.StandardDuration, actual.StandardDuration);
            Assert.AreEqual(expected.StandardMaxFunding, actual.StandardMaxFunding);
            Assert.AreEqual(expected.StartDate, actual.StartDate);
            Assert.AreEqual(expected.NumberOfApprentices, actual.NumberOfApprentices);
            Assert.AreEqual(expected.NumberOfApprenticesUsed, actual.NumberOfApprenticesUsed);
        }
    }
}
