using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Forecasting.Application.Levy.Services;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Application.UnitTests.Levy
{
    [TestFixture]
    public class LevyEmployerDataServiceTests
    {
        private EmployerDataService _levyEmployerDataService;
        

        [SetUp]
        public void SetUp()
        {
            var accountApiClient = new Mock<IAccountApiClient>();
            var hashingService = new Mock<IHashingService>();

            accountApiClient.Setup(m => m.GetLevyDeclarations("ABBA12"))
                .Returns(LevyDeclarations());

            hashingService.Setup(m => m.DecodeValue("ABBA12")).Returns(112233);
            _levyEmployerDataService = new EmployerDataService(accountApiClient.Object, hashingService.Object, Mock.Of<ILog>());
        }


        [Test]
        public async Task Should_generate_levy_declaration()
        {
            var result = await _levyEmployerDataService.LevyForPeriod("ABBA12", "2018-19", 2);

            result.AccountId.Should().Be(112233, because: "Hashed employer id should be decoded to the acctual id");

            result.PayrollYear.Should().Be("2018-19");
            result.PayrollMonth.Should().Be(2);
            result.LevyDeclaredInMonth.Should().Be(2300);
        }

        [Test]
        public async Task Should_be_null_if_multiple_matches()
        {
            var result = await _levyEmployerDataService.LevyForPeriod("ABBA12", "2018-19", 3);

            result.Should().Be(null);
        }

        [Test]
        public async Task Should_be_null_if_no_matches()
        {
            var result = await _levyEmployerDataService.LevyForPeriod("ABBA12", "2018-19", 4);

            result.Should().Be(null);
        }

        private async Task<ICollection<LevyDeclarationViewModel>> LevyDeclarations()
        {
            var l = new List<LevyDeclarationViewModel>
            {
                new LevyDeclarationViewModel
            {
                HashedAccountId = "ABBA12",
                PayrollYear = "2018-19",
                PayrollMonth = 2,
                PayeSchemeReference = "2018-19-2",
                TotalAmount = 2300
            },
                new LevyDeclarationViewModel
            {
                HashedAccountId = "ABBA12",
                PayrollYear = "2018-19",
                PayrollMonth = 3,
                PayeSchemeReference = "2018-19-2",
                TotalAmount = 2300
            },
            new LevyDeclarationViewModel
            {
                HashedAccountId = "ABBA12",
                PayrollYear = "2018-19",
                PayrollMonth = 3,
                PayeSchemeReference = "2018-19-2",
                TotalAmount = 3400
            }

        };

            return await Task.FromResult(l);
        }


    }
}
