using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

using SFA.DAS.Forecasting.Levy.Domain.Aggregates;
using SFA.DAS.Forecasting.Levy.Domain.Entities;
using SFA.DAS.Forecasting.Levy.Domain.Repositories;

namespace SFA.DAS.Forecasting.Levy.UnitTests.Domain
{
    [TestFixture]
    public class EmployerLevyTests
    {
        private Mock<IEmployerLevyRepository> _levyRepository;
        private EmployerLevy _service;

        [SetUp]
        public void SetUp()
        {
            _levyRepository = new Mock<IEmployerLevyRepository>();
            
            _service = new EmployerLevy(_levyRepository.Object);
        }

        [Test]
        public async Task Stores_Valid_Levy_Declaration()
        {
	        var employerAccountId = 123456;
			var year = DateTime.Now.Year;
            var payrollDate = new DateTime(year, 02, 04);

            LevyDeclaration levy = null;
            _levyRepository
				.Setup(m => m.StoreLevyDeclaration(It.IsAny<LevyDeclaration>()))
                .Callback<LevyDeclaration>(l => levy = l)
                .Returns(Task.Run(() => 1));

            await _service.AddDeclaration(employerAccountId, payrollDate, 1000, "ABCD", DateTime.Now);

            Assert.AreEqual(employerAccountId, levy.EmployerAccountId);
            Assert.AreEqual(1000, levy.Amount);
            Assert.AreEqual($"{year}-{payrollDate.Month}", levy.Period);
			
            _levyRepository.Verify(repo => repo.StoreLevyDeclaration(
                    It.IsAny<LevyDeclaration>()));
        }
    }
}