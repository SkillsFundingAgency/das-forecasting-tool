using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.EmployerUsers;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Web.Authentication;

namespace SFA.DAS.Forecasting.Web.UnitTests.Authentication
{
    public class MembershipServiceTests
    {
        private Mock<IMembershipProvider> _membershipProvider;
        private Mock<HttpContextBase> _httpContext;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IApplicationConfiguration> _configuration;
        private Mock<HttpRequestBase> _request;
        private MembershipService _membershipService;
        private const string AccountId = "ABC123";
        private string _userId = "user-one";
        private string _email = "user-email";

        [SetUp]
        public void Setup()
        {
            _httpContext = new Mock<HttpContextBase>();
            _request = new Mock<HttpRequestBase>();
            var routeData = new RouteData();
            routeData.Values.Add(Constants.AccountHashedIdRouteKeyName, AccountId);
            _request.Setup(x => x.RequestContext).Returns(new RequestContext
            {
                RouteData = routeData
            });
            _httpContext.Setup(x => x.Request).Returns(_request.Object);
            _httpContext.Setup(x => x.Items).Returns(new Dictionary<string, object>());
            _membershipProvider = new Mock<IMembershipProvider>();
            _configuration = new Mock<IApplicationConfiguration>();
            
            _owinWrapper = new Mock<IOwinWrapper>();
            _owinWrapper.Setup(x => x.IsUserAuthenticated()).Returns(true);
            _membershipService =
                new MembershipService(_membershipProvider.Object, _httpContext.Object, _owinWrapper.Object, _configuration.Object);
        }

        [Test]
        public async Task Then_If_Not_Authenticated_Returns_Null()
        {
            _owinWrapper.Setup(x => x.IsUserAuthenticated()).Returns(false);

            var actual = await _membershipService.GetMembershipContext();

            Assert.IsNull(actual);
        }

        [Test]
        public async Task Then_If_No_Matching_Claim_Returns_Null()
        {
            var value = "";
            _owinWrapper.Setup(x => x.TryGetClaimValue(Constants.UserExternalIdClaimKeyName, out value)).Returns(false);

            var actual = await _membershipService.GetMembershipContext();

            Assert.IsNull(actual);
        }

        [Test]
        public async Task Then_If_Matching_Claim_Looks_Up_Account_In_Membership_Provider()
        {
            _owinWrapper.Setup(x => x.TryGetClaimValue(Constants.UserExternalIdClaimKeyName, out _userId)).Returns(true);
            _membershipProvider.Setup(x => x.GetMemberships(AccountId)).ReturnsAsync(new List<MembershipContext>
            {
                new MembershipContext
                {
                    UserRef = _userId
                }
            });

            var actual = await _membershipService.GetMembershipContext();

            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task Then_If_GovSignIn_Then_Checks_Against_Returned_Accounts()
        {
            _configuration.Setup(x=>x.UseGovSignIn).Returns(true);
            _owinWrapper.Setup(x => x.TryGetClaimValue(ClaimTypes.NameIdentifier, out _userId)).Returns(true);
            _owinWrapper.Setup(x => x.TryGetClaimValue(ClaimTypes.Email, out _email)).Returns(true);
            _membershipProvider.Setup(x => x.GetUserAccounts(_userId, _email)).ReturnsAsync(new List<EmployerUserAccounts>
            {
                new EmployerUserAccounts
                {
                    AccountId = AccountId
                }
            });

            var actual = await _membershipService.GetMembershipContext();

            Assert.IsNotNull(actual);
            actual.HashedAccountId.Should().Be(AccountId);
            actual.UserRef.Should().Be(_userId);
            actual.UserEmail.Should().Be(_email);
            Assert.IsTrue(_httpContext.Object.Items.Contains(typeof(MembershipContext).FullName));
        }

        [Test]
        public async Task Then_If_GovSignIn_Then_Checks_Against_Returned_Accounts_And_Returns_Null_If_None_Matched()
        {
            _configuration.Setup(x => x.UseGovSignIn).Returns(true);
            _owinWrapper.Setup(x => x.TryGetClaimValue(ClaimTypes.NameIdentifier, out _userId)).Returns(true);
            _owinWrapper.Setup(x => x.TryGetClaimValue(ClaimTypes.Email, out _email)).Returns(true);
            _membershipProvider.Setup(x => x.GetUserAccounts(_userId, _email)).ReturnsAsync(new List<EmployerUserAccounts>
            {
                new EmployerUserAccounts
                {
                    AccountId = "Different"
                }
            });

            var actual = await _membershipService.GetMembershipContext();

            Assert.IsNull(actual);
        }

    }
}
