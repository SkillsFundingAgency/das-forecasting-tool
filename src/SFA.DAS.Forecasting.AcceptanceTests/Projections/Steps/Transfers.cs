using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.AcceptanceTests.Projections.Steps
{
    [Binding]
    public class Transfers : StepsBase
    {
        [Given(@"I say hej")]
        public void GivenISayHej()
        {
            var x = 0;
        }

        [Then(@"transfer out should have (.*) month (.*) to (.*)")]
        public void ThenTransferOutShouldHaveFromMonth_(int expectedAmount, int firstMonth, int lastMonth)
        {
            AccountProjections
                .GetRange(firstMonth - 1, lastMonth - (firstMonth - 1))
                .All(m => m.TransferOutTotalCostOfTraining == expectedAmount)
                .Should().BeTrue();
        }
    }
}
