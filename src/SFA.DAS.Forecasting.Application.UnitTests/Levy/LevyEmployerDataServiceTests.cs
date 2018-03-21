using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMoq;

namespace SFA.DAS.Forecasting.Application.UnitTests.Levy
{
    [TestFixture]
    public class LevyEmployerDataServiceTests
    {
        private EmployerDataService _levyEmployerDataService;
        private AutoMoqer _moqer;

        [SetUp]
        public void SetUp()
        {
            _moqer = new AutoMoqer();

            var accountApiClient = new Mock<IAccountApiClient>();
            var hashingService = new Mock<IHashingService>();

            accountApiClient.Setup(m => m.GetLevyDeclarations("ABBA12"))
                .Returns(LevyDeclarations());
            var levyDeclarations = new LevyDeclarations();
            levyDeclarations.AddRange(LevyDeclarations().Result);
            accountApiClient.Setup(m => m.GetResource<LevyDeclarations>(It.IsAny<string>()))
                .Returns(Task.FromResult(levyDeclarations));

            hashingService.Setup(m => m.DecodeValue("ABBA12")).Returns(112233);
            _levyEmployerDataService = new EmployerDataService(accountApiClient.Object, hashingService.Object, Mock.Of<ILog>());
        }

        [Test]
        public async Task Should_get_levy_declaration_for_period()
        {
            var results = await _levyEmployerDataService.LevyForPeriod("ABBA12", "18-19", 2);
            results.Count.Should().Be(3, "Failed to return expected number of results.");
            results.All(res => LevyDeclarations().Result.Any(levy =>
                res.AccountId == 112233 && res.PayrollYear == levy.PayrollYear &&
                res.PayrollMonth == levy.PayrollMonth &&
                res.LevyDeclaredInMonth == levy.LevyDeclaredInMonth && res.TotalAmount == levy.TotalAmount &&
                res.EmpRef == levy.PayeSchemeReference)).Should().BeTrue("Did not return the expected results");
        }

        //[Test]
        //public async Task Constructs_Correct_Get_Levy_Resource_Path()
        //{
        //    var results = await _levyEmployerDataService.LevyForPeriod("ABBA12", "18-19", 2);
        //    accountApiClient.Setup(m => m.GetResource<LevyDeclarations>(It.IsAny<string>()))
        //        .Returns(Task.FromResult(levyDeclarations));

        //    results.Count.Should().Be(3, "Failed to return expected number of results.");
        //    results.All(res => LevyDeclarations().Result.Any(levy =>
        //        res.AccountId == 112233 && res.PayrollYear == levy.PayrollYear &&
        //        res.PayrollMonth == levy.PayrollMonth &&
        //        res.LevyDeclaredInMonth == levy.LevyDeclaredInMonth && res.TotalAmount == levy.TotalAmount &&
        //        res.EmpRef == levy.PayeSchemeReference)).Should().BeTrue("Did not return the expected results");
        //}

        private async Task<ICollection<LevyDeclarationViewModel>> LevyDeclarations()
        {
            var l = new List<LevyDeclarationViewModel>
            {
                new LevyDeclarationViewModel
                {
                    HashedAccountId = "ABBA12",
                    PayrollYear = "2018-19",
                    PayrollMonth = 2,
                    PayeSchemeReference = "ABC123",
                    TotalAmount = 2300
                },
                    new LevyDeclarationViewModel
                {
                    HashedAccountId = "ABBA12",
                    PayrollYear = "2018-19",
                    PayrollMonth = 2,
                    PayeSchemeReference = "DEF456",
                    TotalAmount = 2300
                },
                new LevyDeclarationViewModel
                {
                    HashedAccountId = "ABBA12",
                    PayrollYear = "2018-19",
                    PayrollMonth = 2,
                    PayeSchemeReference = "GHI789",
                    TotalAmount = 3400
                }
            };

            return await Task.FromResult(l);
        }
    }
}
