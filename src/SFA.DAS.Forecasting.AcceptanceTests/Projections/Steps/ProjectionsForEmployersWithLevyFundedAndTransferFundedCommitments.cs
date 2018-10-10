using FluentAssertions;
using SFA.DAS.Forecasting.AcceptanceTests.Payments;
using System;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Forecasting.AcceptanceTests.Projections.Steps
{
    [Binding]
    public class ProjectionsForEmployersWithLevyFundedAndTransferFundedCommitments : StepsBase
    {

        [Scope(Feature = "Projections For Employers With Levy Funded And Transfer Funded Commitments")]
        [Scope(Feature = "Projections For Employers With Transfer")]
        [BeforeFeature(Order = 1)]
        public static void StartLevyFunctionFundsInReceiving()
        {
            StartFunction("SFA.DAS.Forecasting.Projections.Functions");
            StartFunction("SFA.DAS.Forecasting.StubApi.Functions");
        }

        [Then(@"should have the following projected values")]
        public void ThenShouldHaveFollowingProjectionsFromCompletion(Table table)
        {
            var expectedProjections= table.CreateSet<TestAccountProjection>().ToList();

            foreach (var p in expectedProjections)
            {

                var date = DateTime.Today.AddMonths(p.MonthsFromNow);
                var projection = AccountProjections.Single(m => m.Month == date.Month && m.Year == date.Year);

                string becauseMessage(string propName, object expected, object actual) { return $"Date: {date.Month}-{date.Year}, " +
                        $"Expected {propName} to be {expected} but was {actual}."; };

                projection.LevyFundedCostOfTraining.Should().Be(p.TotalCostOfTraining, 
                    becauseMessage(nameof(p.TotalCostOfTraining), p.TotalCostOfTraining, projection.LevyFundedCostOfTraining));

                projection.TransferInCostOfTraining.Should().Be(p.TransferInTotalCostOfTraining,
                    becauseMessage(nameof(p.TransferInTotalCostOfTraining), p.TransferInTotalCostOfTraining, projection.TransferInCostOfTraining));

                projection.TransferOutCostOfTraining.Should().Be(p.TransferOutTotalCostOfTraining,
                    becauseMessage(nameof(p.TransferOutTotalCostOfTraining), p.TransferOutTotalCostOfTraining, projection.TransferOutCostOfTraining));

                projection.LevyFundedCompletionPayments.Should().Be(p.CompletionPayments,
                    becauseMessage(nameof(p.CompletionPayments), p.CompletionPayments, projection.LevyFundedCompletionPayments));
                
                projection.TransferInCompletionPayments.Should().Be(p.TransferInCompletionPayments,
                    becauseMessage(nameof(p.TransferInCompletionPayments), p.TransferInCompletionPayments, projection.TransferInCompletionPayments));
                
                projection.TransferOutCompletionPayments.Should().Be(p.TransferOutCompletionPayments,
                    becauseMessage(nameof(p.TransferOutCompletionPayments), p.TransferOutCompletionPayments, projection.TransferOutCompletionPayments));

                projection.FutureFunds.Should().Be(p.FutureFunds,
                    becauseMessage(nameof(p.FutureFunds), p.FutureFunds, projection.FutureFunds));

                projection.ExpiredFunds.Should().Be(p.ExpiredFunds,
                    becauseMessage(nameof(p.ExpiredFunds), p.ExpiredFunds, projection.ExpiredFunds));
            }
        }
    }
}
