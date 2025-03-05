using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Levy.Handlers;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Domain.Levy;
using SFA.DAS.Forecasting.Domain.Shared;
using SFA.DAS.Forecasting.Models.Levy;
using LevyDeclaration = SFA.DAS.Forecasting.Domain.Levy.LevyDeclaration;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Levy;

[TestFixture]
public class LevyDeclarationHandlerTests
{
    private LevySchemeDeclarationUpdatedMessage _levySchemeDeclaration;
    private Mock<IPayrollDateService> _payrollDateService;
    private Mock<ILevyDeclarationRepository> _levyDeclarationRepository;

    [SetUp]
    public void SetUp()
    {
        _levySchemeDeclaration = new LevySchemeDeclarationUpdatedMessage
        {
            SubmissionId = 443,
            AccountId = 123456,
            PayrollMonth = 1,
            PayrollYear = "18-19",
            LevyDeclaredInMonth = 10000,
            EmpRef = "abcdef",
            CreatedDate = DateTime.Now
        };

        _payrollDateService = new Mock<IPayrollDateService>();
        _payrollDateService
            .Setup(x => x.GetPayrollDate(_levySchemeDeclaration.PayrollYear, (byte)_levySchemeDeclaration.PayrollMonth))
            .Returns(DateTime.UtcNow);

        _levyDeclarationRepository = new Mock<ILevyDeclarationRepository>();
        _levyDeclarationRepository.Setup(x =>x.Get(It.Is<LevyDeclarationModel>(c => c.SubmissionId.Equals(_levySchemeDeclaration.SubmissionId))))
            .ReturnsAsync(new LevyDeclaration(_payrollDateService.Object, new LevyDeclarationModel()));

    }

    [Test]
    public async Task Uses_Repository_To_Get_Levy_Period()
    {
        var handler = new StoreLevyDeclarationHandler(_levyDeclarationRepository.Object, Mock.Of<ILogger<StoreLevyDeclarationHandler>>(), Mock.Of<IQueueService>());
        await handler.Handle(_levySchemeDeclaration, "forecasting-levy-allow-projection");
        _levyDeclarationRepository
            .Verify(x => x.Get(It.Is<LevyDeclarationModel>(c=>c.SubmissionId.Equals(_levySchemeDeclaration.SubmissionId))), Times.Once());
    }

    [Test]
    public async Task Stores_Levy_Period()
    {
        var handler = new StoreLevyDeclarationHandler(_levyDeclarationRepository.Object, Mock.Of<ILogger<StoreLevyDeclarationHandler>>(), Mock.Of<IQueueService>());
        await handler.Handle(_levySchemeDeclaration, "forecasting-levy-allow-projection");
        _levyDeclarationRepository
            .Verify(x => x.Store(It.IsAny<LevyDeclaration>()), Times.Once());
    }
}