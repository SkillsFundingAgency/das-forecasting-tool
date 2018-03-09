using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.AcceptanceTests.Projections.Steps
{
    [Binding]
    public class GenerateCompletionPaymentProjectionsCI_506Steps: StepsBase
    {
        [Scope(Feature = "Generate Completion Payment Projections [CI-506]")]
        [BeforeFeature(Order = 1)]
        public static void StartLevyFunction()
        {
            StartFunction("SFA.DAS.Forecasting.Projections.Functions");
            StartFunction("SFA.DAS.Forecasting.StubApi.Functions");
        }

        [When(@"the account projection is triggered after a payment run")]
        public void WhenTheAccountProjectionIsGeneratedAfterAPaymentRun()
        {
            DeleteAccountProjections();
            var projectionUrl =
                Config.ProjectionPaymentFunctionUrl.Replace("{employerAccountId}", Config.EmployerAccountId.ToString());
            Console.WriteLine($"Sending payment event to payment projection function: {projectionUrl}");
            var response = HttpClient.PostAsync(projectionUrl, new StringContent("", Encoding.UTF8, "application/json")).Result;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        
        [Then(@"the completion payments should be included in the correct month")]
        public void ThenTheCompletionPaymentsShouldBeIncludedInTheCorrectMonth()
        {
            Commitments.GroupBy(commitment => commitment.PlannedEndDate.AddMonths(1))
                .Select( g => new { Date = g.Key, CompletionAmount = g.Sum(commitment => commitment.CompletionAmount)})
                .ToList()
                .ForEach(completionAmount => Assert.IsTrue(AccountProjections.Any(ac => ac.Year == completionAmount.Date.Year && ac.Month == completionAmount.Date.Month && ac.CompletionPayments == completionAmount.CompletionAmount),$"Completion amount not found. Date: {completionAmount.Date:MMMM yyyy}, Completion Amount: {completionAmount}") );
        }

        [Then(@"the completion payments should not be included in the projection")]
        public void ThenTheCompletionPaymentsShouldNotBeIncludedInTheProjection()
        {
            Commitments.GroupBy(commitment => commitment.PlannedEndDate.AddMonths(1))
                .Select(g => new { Date = g.Key, CompletionAmount = g.Sum(commitment => commitment.CompletionAmount) })
                .ToList()
                .ForEach(completionAmount => Assert.IsFalse(AccountProjections.Any(ac => ac.Year == completionAmount.Date.Year && ac.Month == completionAmount.Date.Month && ac.CompletionPayments == completionAmount.CompletionAmount), $"Completion amount not found. Date: {completionAmount.Date:MMMM yyyy}, Completion Amount: {completionAmount}"));
        }

    }
}
