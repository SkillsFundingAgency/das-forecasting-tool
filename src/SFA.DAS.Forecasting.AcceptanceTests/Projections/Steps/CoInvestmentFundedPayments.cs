using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.AcceptanceTests.Projections.Steps
{
    [Binding]
    public class CoInvestmentFundedPayments : StepsBase
    {
        [Scope(Feature = "CoInvestmentFundedPayments")]
        [BeforeFeature(Order = 1)]
        public static void StartLevyFunctionFundsInReceiving()
        {
            StartFunction("SFA.DAS.Forecasting.Projections.Functions");
            StartFunction("SFA.DAS.Forecasting.StubApi.Functions");
        }

    }
}
