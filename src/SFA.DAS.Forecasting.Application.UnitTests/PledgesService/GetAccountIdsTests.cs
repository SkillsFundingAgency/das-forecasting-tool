using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;

namespace SFA.DAS.Forecasting.Application.UnitTests.PledgesService
{
    [TestFixture]
    public class GetAccountIdsTests
    {
        private Application.ApprenticeshipCourses.Services.PledgesService _pledgesService;
        private Mock<IApiClient> _apiClient;
        private readonly Fixture _fixture = new Fixture();
        private GetPledgeAccountIdsResponse _apiResponse;

        [SetUp]
        public void Setup()
        {
            _apiClient = new Mock<IApiClient>();

            _apiResponse = _fixture.Create<GetPledgeAccountIdsResponse>();

            _apiClient.Setup(x => x.Get<GetPledgeAccountIdsResponse>(It.IsAny<GetPledgeAccountIdsApiRequest>()))
                .ReturnsAsync(_apiResponse);

            _pledgesService =
                new Application.ApprenticeshipCourses.Services.PledgesService(_apiClient.Object, Mock.Of<ILogger<Application.ApprenticeshipCourses.Services.PledgesService>>());
        }

        [Test]
        public async Task GetAccountIds_Returns_Employer_Account_Ids()
        {
            var result = await _pledgesService.GetAccountIds();
            CollectionAssert.AreEqual(_apiResponse.AccountIds, result);
        }
    }
}
