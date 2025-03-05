using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;

namespace SFA.DAS.Forecasting.Application.UnitTests.PledgesService;

[TestFixture]
public class GetPledgesTests
{
    private Application.ApprenticeshipCourses.Services.PledgesService _pledgesService;
    private Mock<IApiClient> _apiClient;
    private readonly Fixture _fixture = new();
    private long _accountId;
    private GetPledgesResponse _apiResponse;

    [SetUp]
    public void Setup()
    {
        _accountId = _fixture.Create<long>();

        _apiClient = new Mock<IApiClient>();

        _apiResponse = _fixture.Create<GetPledgesResponse>();
        _apiClient.Setup(x => x.Get<GetPledgesResponse>(It.Is<GetPledgesApiRequest>(r => r.AccountId == _accountId)))
            .ReturnsAsync(_apiResponse);

        _pledgesService =
            new Application.ApprenticeshipCourses.Services.PledgesService(_apiClient.Object, Mock.Of<ILogger<Application.ApprenticeshipCourses.Services.PledgesService>>());
    }

    [Test]
    public async Task GetPledges_Returns_Pledges_For_Employer_Account()
    {
        var result = await _pledgesService.GetPledges(_accountId);

        Assert.AreEqual(_apiResponse.Pledges.Count, result.Count);

        var index = 0;
        foreach (var expectedPledge in _apiResponse.Pledges)
        {
            var actualPledge = _apiResponse.Pledges[index];
            Assert.AreEqual(expectedPledge.Id, actualPledge.Id);
            Assert.AreEqual(expectedPledge.AccountId, actualPledge.AccountId);
            index++;
        }
    }
}