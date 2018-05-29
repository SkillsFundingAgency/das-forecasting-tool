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
                var projection = AccountProjections.Single(m => m.Month == date.Month && m.Year == date.Year);

                projection.LevyFundedCostOfTraining.Should().Be(p.TotalCostOfTraining,
                    $"Date: {date.Month}-{date.Year},  Expected levy funded cost of training to be {p.TotalCostOfTraining} but was {projection.LevyFundedCostOfTraining}.");
                projection.TransferInCostOfTraining.Should().Be(p.TransferInTotalCostOfTraining,
                    $"Date: {date.Month}-{date.Year},  Expected transfer in cost of training to be {p.TransferInTotalCostOfTraining} but was {projection.TransferInCostOfTraining}.");
                projection.TransferOutCostOfTraining.Should().Be(p.TransferOutTotalCostOfTraining,
                    $"Date: {date.Month}-{date.Year},  Expected transfer out cost of training to be {p.TransferOutTotalCostOfTraining} but was {projection.TransferOutCostOfTraining}.");

                projection.LevyFundedCompletionPayments.Should().Be(p.CompletionPayments,
                    $"Date: {date.Month}-{date.Year}, expected levy funded completion payments to be {p.CompletionPayments} but was {projection.LevyFundedCompletionPayments}.");
                projection.TransferInCompletionPayments.Should().Be(p.TransferInCompletionPayments,
                    $"Date: {date.Month}-{date.Year}, expected transfer in completion payments to be {p.TransferInCompletionPayments} but was {projection.TransferInCompletionPayments}.");
                projection.TransferOutCompletionPayments.Should().Be(p.TransferOutCompletionPayments,
                    $"Date: {date.Month}-{date.Year}, expected transfer out completion payments to be {p.TransferOutCompletionPayments} but was {projection.TransferOutCompletionPayments}.");
                projection.FutureFunds.Should().Be(p.FutureFunds,
                    $"Date: {date.Month}-{date.Year}, expected future funds to be {p.FutureFunds} but was {projection.FutureFunds}.");
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
