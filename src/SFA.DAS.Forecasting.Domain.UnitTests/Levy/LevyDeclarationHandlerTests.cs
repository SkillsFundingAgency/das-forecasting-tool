using System;
using System.Threading.Tasks;
using AutoMoq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Levy.Handlers;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Domain.Levy.Aggregates;
using SFA.DAS.Forecasting.Domain.Levy.Repositories;

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
                AccountId = 123456,
                PayrollMonth = 1,
                PayrollYear = "18-19",
                LevyDeclaredInMonth = 10000,
                EmpRef = "abcdef",
                CreatedDate = DateTime.Now
            };
            var repo = Moqer.GetMock<ILevyPeriodRepository>();
            repo.Setup(x => x.Get(It.IsAny<long>(),
                    It.IsAny<string>(),
                    It.IsAny<short>()))
                .Returns(Task.FromResult(Moqer.GetMock<LevyPeriod>().Object));
            repo.Setup(x => x.StoreLevyPeriod(It.IsAny<LevyPeriod>()))
                .Returns(Task.CompletedTask);
        }

        [Test]
        public async Task Uses_Repository_To_Get_Levy_Period()
        {
            var handler = Moqer.Resolve<ProcessLevyDeclarationHandler>();
            await handler.Handle(LevySchemeDeclaration);
            Moqer.GetMock<ILevyPeriodRepository>().Verify(x => x.Get(
                It.Is<long>(id => id == LevySchemeDeclaration.AccountId),
                It.Is<string>(year => year == LevySchemeDeclaration.PayrollYear),
                It.Is<short>(month => month == LevySchemeDeclaration.PayrollMonth)), Times.Once());
        }

        [Test]
        public async Task Adds_Levy_Declaration_To_Levy_Period()
        {
            var handler = Moqer.Resolve<ProcessLevyDeclarationHandler>();
            await handler.Handle(LevySchemeDeclaration);
            Moqer.GetMock<LevyPeriod>().Verify(x => x.AddDeclaration(
                It.Is<long>(id => id == LevySchemeDeclaration.AccountId),
                It.Is<string>(year => year == LevySchemeDeclaration.PayrollYear),
                It.Is<byte>(month => month == LevySchemeDeclaration.PayrollMonth),
                It.Is<decimal>(amount => amount == LevySchemeDeclaration.LevyDeclaredInMonth),
                It.Is<string>(scheme => scheme == LevySchemeDeclaration.EmpRef),
                It.Is<DateTime>(transactionDate => transactionDate == LevySchemeDeclaration.CreatedDate)), Times.Once());
        }

        [Test]
        public async Task Stores_Levy_Period()
        {
            var handler = Moqer.Resolve<ProcessLevyDeclarationHandler>();
            await handler.Handle(LevySchemeDeclaration);
            Moqer.GetMock<ILevyPeriodRepository>()
                .Verify(x => x.StoreLevyPeriod(It.IsAny<LevyPeriod>()), Times.Once());
        }
    }
}