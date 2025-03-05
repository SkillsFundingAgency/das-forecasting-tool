using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Levy;
using SFA.DAS.Forecasting.Domain.Shared;
using SFA.DAS.Forecasting.Models.Levy;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Levy;

[TestFixture]
public class LevyDeclarationTests
{
    private LevyDeclarationModel _model;
    private Mock<IPayrollDateService> _payrollDateService;

    [SetUp]
    public void SetUp()
    {
        _payrollDateService = new Mock<IPayrollDateService>();
        _payrollDateService
            .Setup(svc => svc.GetPayrollDate(It.IsAny<string>(), It.IsAny<short>()))
            .Returns(DateTime.Today);
        _model = new LevyDeclarationModel
        {
            EmployerAccountId = 12345,
            Id = 99,
            Scheme = "ABC/123",
            PayrollMonth = 1,
            PayrollYear = "18-19",
        };
    }

    [Test]
    public void Rejects_Levy_Declarations_Over_2_Years_Old()
    {
        var levyDeclaration = new LevyDeclaration(_payrollDateService.Object, _model);
        Assert.Catch(() => levyDeclaration.RegisterLevyDeclaration(10000, DateTime.Today.AddYears(-3)));
    }

    [Test]
    public void Declaration_For_Same_Scheme_And_Period_Should_Overwrite_Previous_Declaration()
    {
        _model.LevyAmountDeclared = 1;
        var levyDeclaration = new LevyDeclaration(_payrollDateService.Object, _model);
        levyDeclaration.RegisterLevyDeclaration(10000, DateTime.Today);
        Assert.AreEqual(10000, levyDeclaration.LevyAmountDeclared);
    }
}