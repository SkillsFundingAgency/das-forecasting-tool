using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.EmployerUsers;
using SFA.DAS.Forecasting.Application.EmployerUsers.ApiResponse;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Forecasting.Application.UnitTests.EmployerUsers;

public class WhenGettingAccountUsers
{
    [Test, MoqAutoData]
    public async Task Then_The_Outer_Api_Is_Called_And_Accounts_Returned(
        string email,
        string userId,
        GetUserAccountsResponse apiResponse,
        [Frozen] Mock<IApiClient> outerApiClient,
        EmployerAccountService service)
    {
        //Arrange
        var request = new GetEmployerAccountsRequest(email, userId);
        outerApiClient
            .Setup(x => x.Get<GetUserAccountsResponse>(
                It.Is<GetEmployerAccountsRequest>(c => c.GetUrl.Equals(request.GetUrl)))).ReturnsAsync(apiResponse);
        
        //Act
        var actual = await service.GetUserAccounts(userId, email);

        //Assert
        actual.Should().BeEquivalentTo((EmployerUserAccounts)apiResponse);
    }
}