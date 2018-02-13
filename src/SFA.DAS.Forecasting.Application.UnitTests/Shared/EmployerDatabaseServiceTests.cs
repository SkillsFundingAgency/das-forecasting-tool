using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Application.UnitTests.Shared
{
    [TestFixture]
    public class EmployerDatabaseServiceTests
    {
        private EmployerDatabaseService _dbService;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            
        }

        [Test]
        public async Task Test1()
        {
            //var result = await _dbService.GetEmployerPayments(8509, 2017, 5);
        }
    }
}
