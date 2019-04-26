using System;
using System.Threading.Tasks;
using AutoMoq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Levy.Handlers;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Domain.Levy;
using SFA.DAS.Forecasting.Domain.Shared;
using SFA.DAS.Forecasting.Models.Levy;
using LevyDeclaration = SFA.DAS.Forecasting.Domain.Levy.LevyDeclaration;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Levy
{
    [TestFixture]
    public class LevyDeclarationHandlerTests
    {
        protected AutoMoqer Moqer;
        protected LevySchemeDeclarationUpdatedMessage LevySchemeDeclaration;
        
        [SetUp]
        public void SetUp()
        {
            Moqer = new AutoMoqer();
            LevySchemeDeclaration = new LevySchemeDeclarationUpdatedMessage
            {
                SubmissionId = 443,
                AccountId = 123456,
                PayrollMonth = 1,
                PayrollYear = "18-19",
                LevyDeclaredInMonth = 10000,
                EmpRef = "abcdef",
                CreatedDate = DateTime.Now
            };

            var payrollDateService = Moqer.GetMock<IPayrollDateService>();
            payrollDateService
                .Setup(x => x.GetPayrollDate(LevySchemeDeclaration.PayrollYear, (byte)LevySchemeDeclaration.PayrollMonth))
                .Returns(DateTime.UtcNow);

            var repo = Moqer.GetMock<ILevyDeclarationRepository>();
            repo.Setup(x =>x.Get(It.Is<LevyDeclarationModel>(c => c.SubmissionId.Equals(LevySchemeDeclaration.SubmissionId))))
                .ReturnsAsync(new LevyDeclaration(payrollDateService.Object, new LevyDeclarationModel()));

        }

        [Test]
        public async Task Uses_Repository_To_Get_Levy_Period()
        {
            var handler = Moqer.Resolve<StoreLevyDeclarationHandler>();
            await handler.Handle(LevySchemeDeclaration);
            Moqer.GetMock<ILevyDeclarationRepository>()
                .Verify(x => x.Get(It.Is<LevyDeclarationModel>(c=>c.SubmissionId.Equals(LevySchemeDeclaration.SubmissionId))), Times.Once());
        }

        [Test]
        public async Task Stores_Levy_Period()
        {
            var handler = Moqer.Resolve<StoreLevyDeclarationHandler>();
            await handler.Handle(LevySchemeDeclaration);
            Moqer.GetMock<ILevyDeclarationRepository>()
                .Verify(x => x.Store(It.IsAny<LevyDeclaration>()), Times.Once());
        }
    }
}