using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.AcceptanceTests.Projections.Steps
{
    [Binding]
    public class ExpiredFundsSteps : StepsBase
    {
        [Scope(Feature = "ExpiredFunds - [CI-893]")]
        [BeforeFeature(Order = 1)]
        public static void StartLevyFunction()
        {
            StartFunction("SFA.DAS.Forecasting.Projections.Functions");
            StartFunction("SFA.DAS.Forecasting.StubApi.Functions");
        }

        [Given(@"I have no existing levy declarations")]
        public void GivenIHaveNoExistingLevyDeclarations()
        {
           DeleteAllLevyDeclarations();
        }

        [Given(@"At least one levy declaration which has expired")]
        public void GivenAtLeastOneLevyDeclarationWhichHasExpired()
        {
            var expiredFundsDate = DateTime.UtcNow;
          var ExpiredFunds =  DataContext.LevyDeclarations.Any(w => w.PayrollDate < expiredFundsDate);

            Assert.IsTrue(ExpiredFunds);
        }


    }
}
