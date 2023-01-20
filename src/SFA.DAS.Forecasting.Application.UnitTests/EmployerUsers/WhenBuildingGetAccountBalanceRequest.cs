using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;

namespace SFA.DAS.Forecasting.Application.UnitTests.EmployerUsers;

public class WhenBuildingGetAccountBalanceRequest
{
    [Test, AutoData]
    public void Then_The_Url_Is_Correctly_Formatted_With_Encoded_Email(string accountId)
    {
        var actual = new GetAccountBalanceRequest(accountId);

        actual.GetUrl.Should().Be($"accounts/{accountId}/balance");
    }
}