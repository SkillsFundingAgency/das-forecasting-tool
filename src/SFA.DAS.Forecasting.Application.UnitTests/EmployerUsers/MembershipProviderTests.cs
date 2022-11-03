using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Forecasting.Application.EmployerUsers;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.UnitTests.EmployerUsers
{
    public class MembershipProviderTests
    {
        private MembershipProvider _membershipProvider;
        private Mock<IAccountApiClient> _accountApiClient;
        private Mock<IApiClient> _apiClient;
        private const string HashedAccountId = "ABC123";

        [SetUp]
        public void Arrange()
        {
            _accountApiClient = new Mock<IAccountApiClient>();
            _apiClient = new Mock<IApiClient>();
            _membershipProvider = new MembershipProvider(_accountApiClient.Object, Mock.Of<ILog>(), _apiClient.Object);
        }

        [Test]
        public async Task Then_If_GovSignIn_Calls_OuterApi_To_Get_Membership_Details()
        {
            var fixture = new Fixture();
            var results = fixture.CreateMany<TeamMemberViewModel>().ToList();
            _accountApiClient.Setup(x => x.GetAccountUsers(HashedAccountId))
                .ReturnsAsync(results);

            var actual = await _membershipProvider.GetMemberships(HashedAccountId);

            actual.Should().BeEquivalentTo(results.Select(c=>new MembershipContext
            {
                HashedAccountId = HashedAccountId,
                UserRef = c.UserRef,
                UserEmail = c.Email
            }));
            _apiClient.Verify(x=>x.Get<GetUserAccountsResponse>(It.IsAny<GetEmployerAccountRequest>()), Times.Never);
        }

        [Test]
        public async Task Then_If_Not_GovSignIn_Calls_AccountsApi()
        {
            var fixture = new Fixture();
            var userId = fixture.Create<string>();
            var email = fixture.Create<string>();
            var response = fixture.Create<GetUserAccountsResponse>();
            var request = new GetEmployerAccountRequest(userId, email);
            _apiClient.Setup(x =>
                    x.Get<GetUserAccountsResponse>(
                        It.Is<GetEmployerAccountRequest>(c => c.GetUrl.Equals(request.GetUrl))))
                .ReturnsAsync(response);

            var actual = await _membershipProvider.GetUserAccounts(userId, email);

            actual.Should().BeEquivalentTo(response.UserAccounts);
        }
    }
}
