using FluentAssertions;
using SFA.DAS.Forecasting.AcceptanceTests.Payments;
using System;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Forecasting.AcceptanceTests.Projections.Steps
{
    [Binding]
    public class Transfers : StepsBase
    {
        [Scope(Feature = "Calculate monthly cost of actual transfer commitments (Sending Employer - CI-619)")]
        [BeforeFeature(Order = 1)]
        public static void StartLevyFunction()
        {
            StartFunction("SFA.DAS.Forecasting.Projections.Functions");
            StartFunction("SFA.DAS.Forecasting.StubApi.Functions");
        }


        [Scope(Feature = "Calculate actual transfer completion payments (CI-620)")]
        [BeforeFeature(Order = 1)]
        public static void StartLevyFunction2()
        {
            StartFunction("SFA.DAS.Forecasting.Projections.Functions");
            StartFunction("SFA.DAS.Forecasting.StubApi.Functions");
        }

        [Scope(Feature = "Calculate funds in for receiving employer (CI-644)")]
        [BeforeFeature(Order = 1)]
        public static void StartLevyFunctionFundsInReceiving()
        {
            StartFunction("SFA.DAS.Forecasting.Projections.Functions");
            StartFunction("SFA.DAS.Forecasting.StubApi.Functions");
        }


        [Then(@"should have following projections")]
        public void ThenShouldHaveFollowingProjections(Table table)
        {
            var expectedProjections = table.CreateSet<TestAccountProjection>().ToList();

            foreach (var p in expectedProjections)
            {
                var date = DateTime.Today.AddMonths(p.MonthsFromNow);
                var projections = AccountProjections.Single(m => m.Month == date.Month && m.Year == date.Year);
                projections.TransferOutCostOfTraining.Should().Be(p.TransferOutTotalCostOfTraining);
            }
        }

        [Then(@"should have following projections from completion")]
        public void ThenShouldHaveFollowingProjectionsFromCompletion(Table table)
        {
            var expectedProjections= table.CreateSet<TestAccountProjection>().ToList();

            foreach (var p in expectedProjections)
            {
                var date = DateTime.Today.AddMonths(p.MonthsFromNow);
                var projections = AccountProjections.Single(m => m.Month == date.Month && m.Year == date.Year);

                projections.LevyFundedCostOfTraining.Should().Be(p.TotalCostOfTraining);

                projections.TransferInCostOfTraining.Should().Be(p.TransferInTotalCostOfTraining);
                projections.TransferOutCostOfTraining.Should().Be(p.TransferOutTotalCostOfTraining);

                projections.TransferInCompletionPayments.Should().Be(p.TransferInCompletionPayments);
                projections.TransferOutCompletionPayments.Should().Be(p.TransferOutCompletionPayments);
                projections.LevyFundedCompletionPayments.Should().Be(p.CompletionPayments);
                projections.FutureFunds.Should().Be(p.FutureFunds);
            }
        }

        [Then(@"should have no payments with TransferOutCompletionPayments")]
        public void ThenShouldHaveNoPaymentsWithTransferOutCompletionPayments()
        {
            AccountProjections.Any().Should().BeTrue();
            AccountProjections.Any(m => m.TransferOutCompletionPayments > 0)
                .Should().BeFalse();
        }
    }
}
