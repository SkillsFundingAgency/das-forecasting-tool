using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Commitments.Functions.Application;
using SFA.DAS.Forecasting.Models.Approvals;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Functions.UnitTests.Commitments;

[TestFixture]
public class WhenMappingApiApprenticeship
{
    private Mapper _sut = new();
    private IFixture _fixture = new Fixture();

    [SetUp]
    public void SetUp()
    {
        // Temporary while divide by zero exists
        _fixture.Customize<Apprenticeship>(
            x => x
                .Without(y => y.EndDate)
                .Without(y => y.StartDate)
                .Do(o =>
                {
                    o.StartDate = _fixture.Create<DateTime>();
                    o.EndDate = o.StartDate.AddMonths(13);
                }));
    }

    [Test]
    public void IfNull_ReturnEmptyMessage()
    {
        // Arrange
        var emptyResult = new ApprenticeshipMessage();
        Apprenticeship apiApprenticeship = null;

        // Act
        var result = _sut.Map(apiApprenticeship, 1);

        // Assert
        result.Should().BeEquivalentTo(emptyResult);
    }

    [Test]
    public void ShouldMapToApprenticeshipMessage()
    {
        // Arrange
        var apiApprenticeship = _fixture.Create<Apprenticeship>();

        // Act
        var result = _sut.Map(apiApprenticeship, 1);

        // Assert
        result.EmployerAccountId.Should().Be(1);
        result.SendingEmployerAccountId.Should().Be(apiApprenticeship.TransferSenderId);
        result.ProviderId.Should().Be(apiApprenticeship.ProviderId);
        result.ProviderName.Should().Be(apiApprenticeship.ProviderName);
        result.ApprenticeshipId.Should().Be(apiApprenticeship.Id);
        result.CourseName.Should().Be(apiApprenticeship.CourseName);
        result.StartDate.Should().Be(apiApprenticeship.StartDate);
        result.PlannedEndDate.Should().Be(apiApprenticeship.EndDate);
        result.ActualEndDate.Should().Be(null);
        result.CourseLevel.Should().Be(apiApprenticeship.CourseLevel);
    }

    [Test]
    [TestCase(null, 0)]
    [TestCase("string", 0)]
    [TestCase("1234", 1234)]
    public void ShouldMapTApprenticeshipMessage_LearnerId(string apprenticeshipUln, long expectedLearnerId)
    {
        // Arrange
        var apiApprenticeship = _fixture
            .Build<Apprenticeship>()
            .With(x => x.Uln, apprenticeshipUln)
            .Create();

        // Act
        var result = _sut.Map(apiApprenticeship, 1);

        // Assert
        result.LearnerId.Should().Be(expectedLearnerId);
    }

    [Test]
    public void ShouldMapToApprenticeshipMessage_ApprenticeName()
    {
        // Arrange
        const string firstName = "foo";
        const string lastName = "bar";
        const string expectedApprenticeName = "foo bar";

        var apiApprenticeship = _fixture
            .Build<Apprenticeship>()
            .With(x => x.FirstName, firstName)
            .With(x => x.LastName, lastName)
            .Create();

        // Act
        var result = _sut.Map(apiApprenticeship, 1);

        // Assert
        result.ApprenticeName.Should().Be(expectedApprenticeName);
    }

    [Test]
    [TestCase(100, 20)]
    [TestCase(5000, 1000)]
    public void ShouldMapToApprenticeshipMessage_CompletionAmount(decimal? apprenticeshipCost, int expectedCompletionAmount)
    {
        // Arrange
        var apiApprenticeship = _fixture
            .Build<Apprenticeship>()
            .With(o => o.Cost, apprenticeshipCost)
            .Create();

        // Act
        var result = _sut.Map(apiApprenticeship, 1);

        // Assert
        result.CompletionAmount.Should().Be(expectedCompletionAmount);
    }

    [Test]
    [TestCase(100, 6, 13.33)]
    [TestCase(100, 18, 4.44)]
    [TestCase(100, 36, 2.22)]
    [TestCase(5000, 5, 800)]
    [TestCase(5000, 17, 235.29)]
    [TestCase(5000, 35, 114.29)]
    public void ShouldMapToApprenticeshipMessage_MonthlyInstallment(decimal? apprenticeshipCost, int duration, decimal expectedMonthlyInstallments)
    {
        // Arrange
        _fixture.Customizations.Clear();

        var apiApprenticeship = _fixture
            .Build<Apprenticeship>()
            .With(y => y.Cost, apprenticeshipCost)
            .Without(y => y.EndDate)
            .Without(y => y.StartDate)
            .Do(o =>
            {
                o.StartDate = _fixture.Create<DateTime>();
                o.EndDate = o.StartDate.AddMonths(duration);
            })
            .Create();
        // Act
        var result = _sut.Map(apiApprenticeship, 1);

        // Assert
        result.MonthlyInstallment.Should().BeApproximately(expectedMonthlyInstallments, 0.01m);
    }

    [Test]
    [TestCase(6)]
    [TestCase(18)]
    [TestCase(36)]
    [TestCase(5)]
    [TestCase(17)]
    [TestCase(35)]
    public void ShouldMapToApprenticeshipMessage_NumberOfInstallments(int duration)
    {
        // Arrange
        _fixture.Customizations.Clear();

        var apiApprenticeship = _fixture
            .Build<Apprenticeship>()
            .Without(y => y.EndDate)
            .Without(y => y.StartDate)
            .Do(o =>
            {
                o.StartDate = _fixture.Create<DateTime>();
                o.EndDate = o.StartDate.AddMonths(duration);
            })
            .Create();
        // Act
        var result = _sut.Map(apiApprenticeship, 1);

        // Assert
        result.NumberOfInstallments.Should().Be(duration);
    }

    [Test]
    [TestCase(null, FundingSource.Levy)]
    [TestCase(1234, FundingSource.Transfer)]
    public void ShouldMapToApprenticeshipMessage_FundingSource(long? transferSenderId, FundingSource fundingSource)
    {
        // Arrange
        var apiApprenticeship = _fixture
            .Build<Apprenticeship>()
            .With(o => o.TransferSenderId, transferSenderId)
            .Create();

        // Act
        var result = _sut.Map(apiApprenticeship, 1);

        // Assert
        result.FundingSource.Should().Be(fundingSource);
    }

    [TestCase(null)]
    [TestCase(1234)]
    public void ShouldMapToApprenticeshipMessage_PledgeApplicationId(int? pledgeApplicationId)
    {
        // Arrange
        var apiApprenticeship = _fixture
            .Build<Apprenticeship>()
            .With(o => o.PledgeApplicationId, pledgeApplicationId)
            .Create();

        // Act
        var result = _sut.Map(apiApprenticeship, 1);

        // Assert
        result.PledgeApplicationId.Should().Be(pledgeApplicationId);
    }
}