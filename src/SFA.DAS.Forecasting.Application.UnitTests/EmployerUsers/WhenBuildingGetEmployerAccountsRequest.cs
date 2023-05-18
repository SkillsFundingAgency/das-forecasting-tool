using System.Web;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;

namespace SFA.DAS.Forecasting.Application.UnitTests.EmployerUsers;

public class WhenBuildingGetEmployerAccountsRequest
{
    [Test, AutoData]
    public void Then_The_Url_Is_Correctly_Formatted_With_Encoded_Email(string email, string userId)
    {
        email = email + "'test @+£@$" + email; 
        
        var actual = new GetEmployerAccountsRequest(email, userId);

        actual.GetUrl.Should().Be($"accountusers/{userId}/accounts?email={HttpUtility.UrlEncode(email)}");
    }
}