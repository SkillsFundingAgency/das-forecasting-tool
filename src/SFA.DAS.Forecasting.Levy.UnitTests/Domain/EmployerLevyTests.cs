using System;
using System.Threading.Tasks;
using AutoMoq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Levy.Domain.Aggregates;
using SFA.DAS.Forecasting.Levy.Domain.Entities;
using SFA.DAS.Forecasting.Levy.Domain.Repositories;
using SFA.DAS.Forecasting.Levy.Domain.Services;

namespace SFA.DAS.Forecasting.Levy.UnitTests
{
    [TestFixture]
    public class EmployerLevyTests
    {
        protected AutoMoq.AutoMoqer Moqer { get; private set; }
        protected string EmployerAccountId { get; set; }
        [SetUp]
        public void SetUp()
        {
            Moqer = new AutoMoqer();
            EmployerAccountId = Guid.NewGuid().ToString("D");
            Moqer.GetMock<ILevyPeriodService>()
                .Setup(svc => svc.GetCurrentPeriod()).Returns(Task.FromResult<string>("R01"));
        }

        [Test]
        public void Stores_Valid_Levy_Declaration()
        {
            var service = new EmployerLevy();

            service.AddDeclaration(EmployerAccountId,"R01", 1000, "ABCD", DateTime.Now).Wait();
            Moqer.GetMock<IEmployerLevyRepository>()
                .Verify(repo => repo.StoreLevyDeclaration(It.Is<LevyDeclaration>(x => x.Amount == 1000 && x.EmployerAccountId == EmployerAccountId)));
        }
    }
}