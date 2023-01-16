// using System;
// using System.Linq;
// using System.Net;
// using System.Net.Http;
// using System.Text;
// using Newtonsoft.Json;
// using NUnit.Framework;
// using SFA.DAS.Forecasting.AcceptanceTests.Payments;
// using SFA.DAS.Forecasting.Messages.Projections;
// using TechTalk.SpecFlow;
// using TechTalk.SpecFlow.Assist;
//
// namespace SFA.DAS.Forecasting.AcceptanceTests.Projections.Steps
// {
//     [Binding]
//     public class GenerateCompletionPaymentProjectionsCI_506Steps : StepsBase
//     {
//         [Scope(Feature = "Generate Completion Payment Projections [CI-506]")]
//         [BeforeFeature(Order = 1)]
//         public static void StartLevyFunction()
//         {
//             StartFunction("SFA.DAS.Forecasting.Projections.Functions");
//             StartFunction("SFA.DAS.Forecasting.StubApi.Functions");
//         }
//
//         [Given(@"I am a sending employer")]
//         public void GivenIAmASendingEmployer()
//         {
//             CommitmentType = CommitmentType.TransferSender;
//         }
//
//         [Given(@"I am a receiving employer")]
//         public void GivenIAmAReceivingEmployer()
//         {
//             CommitmentType = CommitmentType.TransferReceiver;
//         }
//
//
//         [When(@"the account projection is triggered after a payment run")]
//         public void WhenTheAccountProjectionIsGeneratedAfterAPaymentRun()
//         {
//             GenerateProjections(Config.EmployerAccountId, Config.ProjectionPaymentFunctionUrl, 8);
//         }
//
//         [When(@"the account projection is triggered after a levy run")]
//         public void WhenTheAccountProjectionIsGeneratedAfterALevyRun()
//         {
//             GenerateProjections(Config.EmployerAccountId, Config.ProjectionLevyFunctionUrl,23);
//         }
//
//         [When(@"the account projection is triggered for (.*) after a payment run")]
//         public void WhenTheAccountProjectionIsGeneratedForIdAfterAPaymentRun(long employerId)
//         {
//             GenerateProjections(employerId, Config.ProjectionPaymentFunctionUrl,8);
//         }
//
//         private void GenerateProjections(long id, string url, int dayOfMonth)
//         {
//             DeleteAccountProjections(id);
//             var projectionUrl = url.Replace("{employerAccountId}", id.ToString());
//             Console.WriteLine($"Sending payment event to payment projection function: {projectionUrl}");
//
//             var calendarPeriod = new CalendarPeriod
//             {
//                 Year = DateTime.Today.Year,
//                 Month = DateTime.Today.Month,
//                 Day = dayOfMonth
//             };
//
//             var response = HttpClient.PostAsync(projectionUrl, new StringContent(JsonConvert.SerializeObject(calendarPeriod), Encoding.UTF8, "application/json")).Result;
//             Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
//         }
//
//         [Then(@"the completion payments should be included in the correct month")]
//         public void ThenTheCompletionPaymentsShouldBeIncludedInTheCorrectMonth()
//         {
//             Commitments.GroupBy(commitment => commitment.PlannedEndDate.AddMonths(1))
//                 .Select(g => new { Date = g.Key, CompletionAmount = g.Sum(commitment => commitment.CompletionAmount) })
//                 .ToList()
//                 .ForEach(completionAmount => Assert.IsTrue(AccountProjections.Any(ac => ac.Year == completionAmount.Date.Year && ac.Month == completionAmount.Date.Month && ac.LevyFundedCompletionPayments == completionAmount.CompletionAmount), $"Completion amount not found. Date: {completionAmount.Date:MMMM yyyy}, Completion Amount: {completionAmount}"));
//         }
//
//         [Then(@"the completion payments should not be included in the projection")]
//         public void ThenTheCompletionPaymentsShouldNotBeIncludedInTheProjection()
//         {
//             Commitments.GroupBy(commitment => commitment.PlannedEndDate.AddMonths(1))
//                 .Select(g => new { Date = g.Key, CompletionAmount = g.Sum(commitment => commitment.CompletionAmount) })
//                 .ToList()
//                 .ForEach(completionAmount => Assert.IsFalse(AccountProjections.Any(ac => ac.Year == completionAmount.Date.Year && ac.Month == completionAmount.Date.Month && ac.LevyFundedCompletionPayments == completionAmount.CompletionAmount), $"Completion amount not found. Date: {completionAmount.Date:MMMM yyyy}, Completion Amount: {completionAmount}"));
//         }
//         [Then(@"the transfer completion payments should be recorded as follows")]
//         public void ThenTheTransferCompletionPaymentsShouldBeRecordedAsFollows(Table table)
//         {
//             var expectedProjections = table.CreateSet<TestAccountProjection>().ToList();
//             expectedProjections.ForEach(expected =>
//             {
//                 var projectionMonth = AccountProjections.Skip(expected.MonthsFromNow).FirstOrDefault();
//                 Assert.IsNotNull(projectionMonth, $"Month {expected.MonthsFromNow} not found.");
//                 Assert.AreEqual(expected.TransferInCompletionPayments, projectionMonth.TransferInCompletionPayments, $"Transfer in completion payments do not match.  Months from now: {expected.MonthsFromNow}, expected '{expected.TransferInCompletionPayments}' but generated amount was '{projectionMonth.TransferInCompletionPayments}'");
//                 Assert.AreEqual(expected.TransferOutCompletionPayments, projectionMonth.TransferOutCompletionPayments, $"Transfer out completion payments do not match.  Months from now: {expected.MonthsFromNow}, expected '{expected.TransferOutCompletionPayments}' but generated amount was '{projectionMonth.TransferOutCompletionPayments}'");
//             });
//         }
//
//         [Then(@"the unallocated completion amount is (.*)")]
//         public void ThenTheUnallocatedCompletionAmountIs(int unallocatedCompletionPayments)
//         {
//             var accountBalance = DataContext.Balances.FirstOrDefault(balance => balance.EmployerAccountId == Config.EmployerAccountId);
//             Assert.IsNotNull(accountBalance);
//             Assert.AreEqual(unallocatedCompletionPayments, accountBalance.UnallocatedCompletionPayments);
//         }
//     }
// }
