using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.UnitTests.ApprovalsService
{
    [TestFixture]
    public class GetAccountIdsTests
    {
        private Application.ApprenticeshipCourses.Services.ApprovalsService _approvalsService;
        private Mock<IApiClient> _apiClient;
        private readonly Fixture _fixture = new Fixture();
        private GetAccountIdsResponse _apiResponse;

        [SetUp]
        public void Setup()
        {
            _apiClient = new Mock<IApiClient>();

            _apiResponse = _fixture.Create<GetAccountIdsResponse>();

            _apiClient.Setup(x => x.Get<GetAccountIdsResponse>(It.IsAny<GetAccountIdsApiRequest>()))
                .ReturnsAsync(_apiResponse);

            _approvalsService =
                new Application.ApprenticeshipCourses.Services.ApprovalsService(_apiClient.Object, Mock.Of<ILog>());
        }

        [Test]
        public async Task GetAccountIds_Returns_Employer_Account_Ids()
        {
            var result = await _approvalsService.GetEmployerAccountIds();
            CollectionAssert.AreEqual(_apiResponse.AccountIds, result);
        }
    }
}
