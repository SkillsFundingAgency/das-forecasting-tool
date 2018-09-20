using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.AcceptanceTests.Payments;
using SFA.DAS.Forecasting.Core;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Forecasting.AcceptanceTests.Projections.Steps
{
    [Binding]
    public class GenerateTrainingCostProjectionsCI_499Steps : StepsBase
    {
        [Scope(Feature = "Generate Training Cost Projections [CI-499]")]
        [BeforeFeature(Order = 1)]
        public static void StartLevyFunction()
        {
            StartFunction("SFA.DAS.Forecasting.Projections.Functions");
            StartFunction("SFA.DAS.Forecasting.StubApi.Functions");
        }

        [Then(@"the training costs should be included in the correct months")]
        public void ThenTheTrainingCostsShouldBeIncludedInTheCorrectMonth()
        {
            AccountProjections.ForEach(projection =>
                {
                    var expectedCost = Commitments
                        .Where(c => IsIncluded(c, projection.Month, projection.Year))
                        .Sum(c => c.InstallmentAmount);
                    Assert.AreEqual(expectedCost, projection.LevyFundedCostOfTraining,
                        $"Total cost of training mismatch. Month: {projection.Month}, Year: {projection.Year}, Expected amount: {expectedCost} but was: {projection.LevyFundedCostOfTraining}");
                });
        }

        private bool IsIncluded(TestCommitment commitment, short month, int year)
        {
            var projectionDate = new DateTime(year, month, 1);
            if (commitment.StartDateValue.GetStartOfMonth() >= projectionDate)
                return false;
            var lastPaymentDate = DateTime.DaysInMonth(commitment.PlannedEndDate.Year, commitment.PlannedEndDate.Month) ==
                commitment.PlannedEndDate.Day
                    ? commitment.PlannedEndDate.AddMonths(1)
                    : commitment.PlannedEndDate;

            return lastPaymentDate.GetStartOfMonth() >= projectionDate;
        }

        [Then(@"should have the following transfers costs of training")]
        public void ThenShouldHaveFollowingProjections(Table table)
        {
            var expectedProjections = table.CreateSet<TestAccountProjection>().ToList();

            foreach (var p in expectedProjections)
            {
                var date = DateTime.Today.AddMonths(p.MonthsFromNow);
                var projection = AccountProjections.Single(m => m.Month == date.Month && m.Year == date.Year);
                if (table.ContainsColumn("Transfer In Total Cost Of Training"))
                    projection.TransferInCostOfTraining.Should().Be(p.TransferInTotalCostOfTraining);
                if (table.ContainsColumn("Transfer Out Total Cost Of Training"))
                    projection.TransferOutCostOfTraining.Should().Be(p.TransferOutTotalCostOfTraining);
            }
        }
    }
}
