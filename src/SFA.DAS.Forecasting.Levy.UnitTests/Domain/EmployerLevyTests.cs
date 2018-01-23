using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

using SFA.DAS.Forecasting.Levy.Domain.Aggregates;
using SFA.DAS.Forecasting.Levy.Domain.Entities;
using SFA.DAS.Forecasting.Levy.Domain.Repositories;

namespace SFA.DAS.Forecasting.Levy.UnitTests
{
    [TestFixture]
    public class EmployerLevyTests
    {
        private Mock<IEmployerLevyRepository> _levyRepository;
        private EmployerLevy _service;
        protected long EmployerAccountId { get; set; }

        [SetUp]
        public void SetUp()
        {
            
            EmployerAccountId = 123456;

            _levyRepository = new Mock<IEmployerLevyRepository>();
            
            _service = new EmployerLevy(_levyRepository.Object);
        }

        [Test]
        public async Task Stores_Valid_Levy_Declaration()
        {
            var year = DateTime.Now.Year;
            var payrollDate = new DateTime(year, 02, 04);

            LevyDeclaration levy = null;
            _levyRepository.Setup(m => m.StoreLevyDeclaration(It.IsAny<LevyDeclaration>()))
                .Callback<LevyDeclaration>(l => levy = l)
                .Returns(Task.Run(() => 1));

            await _service.AddDeclaration(EmployerAccountId, payrollDate, 1000, "ABCD", DateTime.Now);

            Assert.AreEqual(EmployerAccountId, levy.EmployerAccountId);
            Assert.AreEqual(1000, levy.Amount);
            Assert.AreEqual($"{year}-{payrollDate.Month}", levy.Period);


            _levyRepository.Verify(repo => repo.StoreLevyDeclaration(
                    It.IsAny<LevyDeclaration>()));
        }
    }
}