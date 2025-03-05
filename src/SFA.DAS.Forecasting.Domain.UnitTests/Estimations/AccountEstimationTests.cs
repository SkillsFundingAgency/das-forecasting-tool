using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Domain.Estimations.Validation.VirtualApprenticeships;
using SFA.DAS.Forecasting.Domain.Shared;
using SFA.DAS.Forecasting.Domain.Shared.Validation;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Models.Payments;
using AccountEstimation = SFA.DAS.Forecasting.Domain.Estimations.AccountEstimation;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Estimations;

public class AccountEstimationTests
{
    private AccountEstimationModel _model;
    private Mock<IVirtualApprenticeshipValidator> _virtualApprenticeshipValidator;
    private Mock<IAccountEstimationRepository> _accountEstimationRepository;
    private Mock<IDateTimeService> _dataTimeService;
    private AccountEstimation _accountEstimation;

    [SetUp]
    public void SetUp()
    {
        _model = new AccountEstimationModel
        {
            Id = Guid.NewGuid().ToString("N"),
            Apprenticeships = new List<VirtualApprenticeship>(),
            EmployerAccountId = 12345,
            EstimationName = "default"
        };
            
            
            
        _virtualApprenticeshipValidator = new Mock<IVirtualApprenticeshipValidator>();
        _virtualApprenticeshipValidator
            .Setup(x => x.Validate(It.IsAny<VirtualApprenticeship>()))
            .Returns(new List<ValidationResult>());
            
        _dataTimeService = new Mock<IDateTimeService>();
        _dataTimeService
            .Setup(x => x.GetCurrentDateTime())
            .Returns(new DateTimeService().GetCurrentDateTime());

        _accountEstimation = new AccountEstimation(_model, _virtualApprenticeshipValidator.Object, _dataTimeService.Object);
            
        _accountEstimationRepository = new Mock<IAccountEstimationRepository>();
        _accountEstimationRepository
            .Setup(x => x.Get(It.IsAny<long>()))
            .ReturnsAsync(_accountEstimation);
    }

    [Test]
    public void Add_Virtual_Apprenticeship_Assigns_Id_To_Apprenticeship()
    {
            
        var apprenticeship = _accountEstimation.AddVirtualApprenticeship("course-1", "test course", 1, 1, 2019, 5, 18, 1000, FundingSource.Transfer);
        Assert.IsNotNull(apprenticeship, "Invalid virtual apprenticeship generated.");
        Assert.IsNotNull(apprenticeship.Id, "Apprentieship id not populated.");
    }

    [Test]
    public void Throws_Exception_If_Validation_Fails()
    {
        _virtualApprenticeshipValidator
            .Setup(x => x.Validate(It.IsAny<VirtualApprenticeship>()))
            .Returns(new List<ValidationResult> { ValidationResult.Failed("test fail") });
        Assert.Throws<InvalidOperationException>(() => _accountEstimation.AddVirtualApprenticeship("course-1", "test course", 1, 1, 2019, 5, 18, 1000, FundingSource.Transfer), "Should throw an exception if the apprenticeship fails validation");
    }

    [Test]
    public void Valid_Apprenticeships_Are_Added_To_The_Model()
    {
        var apprenticeship = _accountEstimation.AddVirtualApprenticeship("course-1", "test course", 1, 1, 2019, 5, 18, 1000, FundingSource.Transfer);
        Assert.IsTrue(_accountEstimation.Apprenticeships.Any(x => x.Id == apprenticeship.Id));
    }

    [Test]
    public void Remove_Returns_False_If_Apprenticeship_Not_Found()
    {
        Assert.IsFalse(_accountEstimation.RemoveVirtualApprenticeship("apprenticeship-1"));
    }

    [Test]
    public void Remove_Returns_True_If_Apprenticeship_Removed()
    {
        _model.Apprenticeships.Add(new VirtualApprenticeship { Id = "apprenticeship-1" });
        Assert.IsTrue(_accountEstimation.RemoveVirtualApprenticeship("apprenticeship-1"));
    }

    [Test]
    public void Remove_Apprenticeship_Removes_Apprenticeship()
    {
        _model.Apprenticeships.Add(new VirtualApprenticeship { Id = "apprenticeship-1" });
            
        _accountEstimation.RemoveVirtualApprenticeship("apprenticeship-1");
        Assert.IsTrue(_accountEstimation.Apprenticeships.All(x => x.Id != "apprenticeship-1"));
    }

    [Test]
    public void Should_Update_Apprenticeship()
    {
        var a = new VirtualApprenticeship
        {
            Id = Guid.NewGuid().ToString("N"),
            CourseId = "ABBA12",
            CourseTitle = "ABBA 12",
            Level = 1,
            ApprenticesCount = 10,
            StartDate = new DateTime(DateTime.Today.Year + 1, 5, 1),
            TotalInstallments = 24,
            TotalCost = 2000,
        };

        _model.Apprenticeships.Add(a);

        _accountEstimation.UpdateApprenticeship(a.Id, 10, DateTime.Today.Year + 2, 6, 12, 1000);
        _accountEstimation.Apprenticeships.Count.Should().Be(1);

        var apprenticeship = _accountEstimation.Apprenticeships.First();
        apprenticeship.CourseId.Should().Be("ABBA12");
        apprenticeship.CourseTitle.Should().Be("ABBA 12");
        apprenticeship.Level.Should().Be(1);
        apprenticeship.ApprenticesCount.Should().Be(6);
        apprenticeship.StartDate.Year.Should().Be(DateTime.Today.Year + 2);
        apprenticeship.StartDate.Month.Should().Be(10);
        apprenticeship.TotalCost.Should().Be(1000);
        apprenticeship.TotalInstallments.Should().Be(12);

        apprenticeship.TotalCompletionAmount.Should().Be(200);
        Decimal.Round(apprenticeship.TotalInstallmentAmount, 1).Should().Be(66.7M);
    }

    [Test]
    public void Add_Virtual_Apprenticeship_Assigns_FundingSource_To_Apprenticeship()
    {
        var apprenticeship = _accountEstimation.AddVirtualApprenticeship("course-1", "test course", 1, 1, 2019, 5, 18, 1000, FundingSource.Transfer);
        Assert.AreEqual(apprenticeship.FundingSource, FundingSource.Transfer);

        var apprenticeship2 = _accountEstimation.AddVirtualApprenticeship("course-1", "test course", 1, 1, 2019, 5, 18, 1000, FundingSource.Levy);
        Assert.AreEqual(apprenticeship2.FundingSource, FundingSource.Levy);
    }

        
    [TestCase(-20, 12, false)]
    [TestCase(-15, 12, false)]
    [TestCase(-5, 10, true)]
    public void Has_Valid_Apprenticeships_Returns_False_When_No_Active_Apprenticeshp(int numberOfMonthsToAdd, int numberOfMonths, bool isValid)
    {
        // arrange
        var startDate = DateTime.Now.AddMonths(numberOfMonthsToAdd);
        _accountEstimation.AddVirtualApprenticeship("course-1", "test course", 1, startDate.Month, startDate.Year, 5, numberOfMonths, 1000, FundingSource.Transfer);

        //act
        var isAppreniceshipValid = _accountEstimation.HasValidApprenticeships;

        //assert
        Assert.AreEqual(isValid, isAppreniceshipValid);
    }
}