using System;
using System.Threading.Tasks;
using AutoMoq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Levy.Handlers;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Domain.Levy;

namespace SFA.DAS.Forecasting.Application.UnitTests.Levy
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
                AccountId = 123456,
                PayrollMonth = 1,
                PayrollYear = "18-19",
                LevyDeclaredInMonth = 10000,
                EmpRef = "abcdef",
                CreatedDate = DateTime.Now
            };
            var repo = Moqer.GetMock<ILevyDeclarationRepository>();
            repo.Setup(x => x.Get(It.IsAny<long>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<byte>()))
                .Returns(Task.FromResult(Moqer.Resolve<LevyDeclaration>()));
            repo.Setup(x => x.Store(It.IsAny<LevyDeclaration>()))
                .Returns(Task.CompletedTask);
            
        }

        [Test]
        public async Task Uses_Repository_To_Get_Levy_Period()
        {
            var handler = Moqer.Resolve<StoreLevyDeclarationHandler>();
            await handler.Handle(LevySchemeDeclaration, "some queue");
            Moqer.GetMock<ILevyDeclarationRepository>()
                .Verify(x => x.Get(
                It.Is<long>(id => id == LevySchemeDeclaration.AccountId),
                It.Is<string>(scheme => scheme == LevySchemeDeclaration.EmpRef),
                It.Is<string>(year => year == LevySchemeDeclaration.PayrollYear),
                It.Is<byte>(month => month == LevySchemeDeclaration.PayrollMonth)), Times.Once());
        }

        [Test]
        public async Task Stores_Levy_Period()
        {
            var handler = Moqer.Resolve<StoreLevyDeclarationHandler>();
            await handler.Handle(LevySchemeDeclaration, "some queue");
            Moqer.GetMock<ILevyDeclarationRepository>()
                .Verify(x => x.Store(It.IsAny<LevyDeclaration>()), Times.Once());
        }
    }
}