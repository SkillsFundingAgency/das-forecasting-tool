using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.UnitTests.ApprovalsService
{
    [TestFixture]
    public class GetApprenticeshipsTests
    {
        private Application.ApprenticeshipCourses.Services.ApprovalsService _approvalsService;
        private Mock<IApiClient> _apiClient;
        private readonly Fixture _fixture = new Fixture();
        private GetApprenticeshipsResponse _apiResponse;
        private long _employerAccountId;

        [SetUp]
        public void Setup()
        {
            _apiClient = new Mock<IApiClient>();

            _employerAccountId = _fixture.Create<long>();
            _apiResponse = _fixture.Create<GetApprenticeshipsResponse>();

            _apiClient.Setup(x => x.Get<GetApprenticeshipsResponse>(It.IsAny<GetApprenticeshipsApiRequest>()))
                .ReturnsAsync(_apiResponse);

            _approvalsService =
                new Application.ApprenticeshipCourses.Services.ApprovalsService(_apiClient.Object, Mock.Of<ILog>());
        }

        [Test]
        public async Task GetApprenticeships_Returns_Apprenticeships_For_Given_EmployerAccountId()
        {
            await _approvalsService.GetApprenticeships(_employerAccountId);
            _apiClient.Verify(x => x.Get<GetApprenticeshipsResponse>(It.Is<GetApprenticeshipsApiRequest>(r => r.EmployerAccountId == _employerAccountId)), Times.Exactly(2));
        }


        [Test]
        public async Task GetApprenticeships_Returns_Apprenticeships_For_WaitingToStart_And_Live_Statuses()
        {
            var waitingToStartResponse = _fixture.Create<GetApprenticeshipsResponse>();
            var liveResponse = _fixture.Create<GetApprenticeshipsResponse>();

            waitingToStartResponse.TotalApprenticeshipsFound = 1;
            liveResponse.TotalApprenticeshipsFound = 1;

            _apiClient = new Mock<IApiClient>();
            _apiClient.Setup(x => x.Get<GetApprenticeshipsResponse>(It.Is<GetApprenticeshipsApiRequest>(r => r.Status == GetApprenticeshipsApiRequest.ApprenticeshipStatus.WaitingToStart)))
                .ReturnsAsync(waitingToStartResponse);
            _apiClient.Setup(x => x.Get<GetApprenticeshipsResponse>(It.Is<GetApprenticeshipsApiRequest>(r => r.Status == GetApprenticeshipsApiRequest.ApprenticeshipStatus.Live)))
                .ReturnsAsync(liveResponse);
            _approvalsService =
                new Application.ApprenticeshipCourses.Services.ApprovalsService(_apiClient.Object, Mock.Of<ILog>());


            var result = await _approvalsService.GetApprenticeships(_employerAccountId);

            var expectedResult = waitingToStartResponse.Apprenticeships.Union(liveResponse.Apprenticeships).ToList();

            Assert.AreEqual(expectedResult.Count, result.Count);

            var i = 0;
            foreach (var expectedApprenticeship in expectedResult)
            {
                AssertEquality(expectedApprenticeship, result[i]);
                i++;
            }
        }

        [TestCase(GetApprenticeshipsApiRequest.ApprenticeshipStatus.Live)]
        [TestCase(GetApprenticeshipsApiRequest.ApprenticeshipStatus.WaitingToStart)]
        public async Task GetApprenticeships_Returns_Multiple_Pages_Of_Data_Where_Available(short status)
        {
            _apiClient = new Mock<IApiClient>();
            var empty = new GetApprenticeshipsResponse
            {
                Apprenticeships = new List<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>(),
                PageNumber = 1
            };

            _apiClient.Setup(x => x.Get<GetApprenticeshipsResponse>(It.Is<GetApprenticeshipsApiRequest>(r => r.Status != status)))
                .ReturnsAsync(empty);

            _approvalsService =
                new Application.ApprenticeshipCourses.Services.ApprovalsService(_apiClient.Object, Mock.Of<ILog>());

            var totalApprenticeships = Application.ApprenticeshipCourses.Services.ApprovalsService.PageSize * 3;

            var pages = new List<GetApprenticeshipsResponse>();
            for (var pageNumber = 1; pageNumber <= 3 ; pageNumber++)
            {
                var page = new GetApprenticeshipsResponse
                {
                    PageNumber = pageNumber,
                    TotalApprenticeshipsFound = totalApprenticeships,
                    Apprenticeships = _fixture.CreateMany<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>()
                };

                pages.Add(page);

                var number = pageNumber;
                _apiClient.Setup(x => x.Get<GetApprenticeshipsResponse>(It.Is<GetApprenticeshipsApiRequest>(r => r.Status == status && r.Page == number)))
                    .ReturnsAsync(page);
            }

            var result = await _approvalsService.GetApprenticeships(_employerAccountId);

            var expectedResult = pages.SelectMany(p => p.Apprenticeships).ToList();

            Assert.AreEqual(expectedResult.Count, result.Count);

            var i = 0;
            foreach (var expectedApprenticeship in expectedResult)
            {
                AssertEquality(expectedApprenticeship, result[i]);
                i++;
            }
        }


        private void AssertEquality(GetApprenticeshipsResponse.ApprenticeshipDetailsResponse expected,
            Models.Approvals.Apprenticeship actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.TransferSenderId, actual.TransferSenderId);
            Assert.AreEqual(expected.Uln, actual.Uln);
            Assert.AreEqual(expected.ProviderId, actual.ProviderId);
            Assert.AreEqual(expected.ProviderName, actual.ProviderName);
            Assert.AreEqual(expected.FirstName, actual.FirstName);
            Assert.AreEqual(expected.LastName, actual.LastName);
            Assert.AreEqual(expected.CourseCode, actual.CourseCode);
            Assert.AreEqual(expected.CourseName, actual.CourseName);
            Assert.AreEqual(expected.StartDate, actual.StartDate);
            Assert.AreEqual(expected.EndDate, actual.EndDate);
            Assert.AreEqual(expected.Cost, actual.Cost);
            Assert.AreEqual(expected.HasHadDataLockSuccess, actual.HasHadDataLockSuccess);
            Assert.AreEqual(expected.PledgeApplicationId, actual.PledgeApplicationId);
        }
    }
}
