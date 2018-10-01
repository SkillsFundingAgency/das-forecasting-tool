using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using NUnit.Framework;
using SFA.DAS.Forecasting.AcceptanceTests.Levy;
using SFA.DAS.Forecasting.AcceptanceTests.Payments;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Payments;
using SFA.DAS.Forecasting.Models.Projections;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Forecasting.AcceptanceTests.Projections.Steps
{
    [Binding]
    public class GenerateLevyProjectionsCI_498Steps : StepsBase
    {



        [Scope(Feature = "Generate Levy Projections [CI-498]")]
        [BeforeFeature(Order = 1)]
        public static void StartLevyFunction()
        {
            StartFunction("SFA.DAS.Forecasting.StubApi.Functions");
            StartFunction("SFA.DAS.Forecasting.Projections.Functions");
        }

        [Given(@"the following levy declarations have been recorded")]
        public void GivenTheFollowingLevyDeclarationHasBeenRecorded(Table table)
        {
            var levySubmissions = table.CreateSet<LevySubmission>().ToList();
            DeleteLevyDeclarations();
            InsertLevyDeclarations(levySubmissions);
            LevySubmissions = levySubmissions;
        }

        [Given(@"the following commitments have been recorded")]
        public void GivenTheFollowingCommitmentsHaveBeenRecorded(Table table)
        {
            Commitments = table.CreateSet<TestCommitment>().ToList();
            DeleteCommitments(EmployerAccountId);
            DeleteCommitments(54321);
            InsertCommitments(Commitments);
        }

        [Given(@"the current balance is (.*)")]
        public async Task GivenTheCurrentBalanceIs(decimal balance)
        {
            Balance = balance;
            DeleteBalance();

            Console.WriteLine($"Setting balance. Uri: {Config.ApiInsertBalanceUrl}, balance: {balance}");
            var client = new HttpClient();
            await client.PostAsync(Config.ApiInsertBalanceUrl, new StringContent(balance.ToString()));
        }

        [Given(@"the start month should be last month rather than this month")]
        public void GivenTheStartMonthShouldBeThisMonthRatherThanNextMonth()
        {
            ProjectionsStartPeriod = new CalendarPeriod { Month = DateTime.Today.Month - 1, Year = DateTime.Today.Year };
        }

        [When(@"the account projection is triggered after levy has been declared")]
        public void WhenTheAccountProjectionIsGenerated()
        {
            var startPeriodJson = ScenarioContext.Current.ContainsKey("projections_start_period")
                ? ProjectionsStartPeriod.ToJson()
                : string.Empty;

            DeleteAccountProjections(Config.EmployerAccountId);
            var projectionUrl =
                Config.ProjectionLevyFunctionUrl.Replace("{employerAccountId}", Config.EmployerAccountId.ToString());
            Console.WriteLine($"Sending levy event to levy function: {projectionUrl}, payload: {startPeriodJson}");
            var response = HttpClient.PostAsync(projectionUrl, new StringContent(startPeriodJson, Encoding.UTF8, "application/json")).Result;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Then(@"there should be (.*) commitments")]
        public void ThenThereShouldBeCommitments(int commitmetnCount)
        {
            WaitForIt(() =>
            {
                var commitments = new List<CommitmentModel>();
                ExecuteSql(() =>
                {
                    commitments = DataContext.Commitments
                        .Where(c => c.EmployerAccountId == Config.EmployerAccountId || c.SendingEmployerAccountId == Config.EmployerAccountId)
                        .ToList();
                });

                if (commitments.Count() != commitmetnCount)
                {
                    Tuple.Create(false, $"Expected {commitments} but found ${commitments.Count()} commitments");
                }

                return Tuple.Create(true, "");
            }, "Failed getting commitments.");
        }


		[Then(@"the account projection should be generated")]
		public void ThenTheAccountProjectionShouldBeGenerated()
		{
			WaitForIt(() =>
			{
				var projections = new List<AccountProjectionModel>();
				ExecuteSql(() =>
				{
					projections = DataContext.AccountProjections.Where(projection =>
							projection.EmployerAccountId == Config.EmployerAccountId)
						.ToList();
				});
				if (!projections.Any())
					return false;
				projections = projections
					.OrderBy(projection => $"{projection.Year:0000}-{projection.Month:00}")
					.ToList();
				projections.ForEach(p => Console.WriteLine($"Month: {p.Month}, Year: {p.Year}, Funds In: {p.LevyFundsIn}, Cost of training: {p.LevyFundedCostOfTraining}, Completion Payments: {p.LevyFundedCompletionPayments}, Future funds: {p.FutureFunds}, Co-Investment: {p.CoInvestmentEmployer} / {p.CoInvestmentGovernment}"));
				AccountProjections = projections;
				return true;
			}, "Account projection failed.");
		}

	    [Then(@"the account projection should not be generated")]
	    public void ThenTheAccountProjectionShouldNotBeGenerated()
	    {
		    WaitForIt(() =>
		    {
			    var projections = new List<AccountProjectionModel>();
			    ExecuteSql(() =>
			    {
				    projections = DataContext.AccountProjections.Where(projection =>
						    projection.EmployerAccountId == Config.EmployerAccountId)
					    .ToList();
			    });
			    return !projections.Any();
		    }, "Account projection created.");
	    }

		[Then(@"calculated levy credit value should be the amount declared for the single linked PAYE scheme")]
        public void ThenCalculatedLevyCreditValueShouldBeTheAmountDeclaredForTheSingleLinkedPAYEScheme()
        {
            AccountProjections.ForEach(projection => Assert.AreEqual(projection.LevyFundsIn, LevySubmissions.FirstOrDefault()?.Amount, $"Expected the account projections to be {LevySubmissions.FirstOrDefault()?.Amount} but was {projection.LevyFundsIn}"));
        }

        [Then(@"calculated levy credit value should be the amount declared for the sum of the linked PAYE schemes")]
        public void ThenCalculatedLevyCreditValueShouldBeTheAmountDeclaredForTheSumOfTheLinkedPAYESchemes()
        {
            AccountProjections.ForEach(projection => Assert.AreEqual(projection.LevyFundsIn, LevySubmissions.Sum(levy => levy.Amount), $"Expected the account projections to be {LevySubmissions.FirstOrDefault()?.Amount} but was {projection.LevyFundsIn}"));
        }


        [Then(@"each future month's forecast levy credit should be the same")]
        public void ThenEachFutureMonthSForecastLevyCreditShouldBeTheSame()
        {
            var fundsIn = LevySubmissions.Sum(levy => levy.Amount);
            Assert.IsTrue(AccountProjections.All(projection => projection.LevyFundsIn == fundsIn));
        }

        [Then(@"the first month should be last month rather than this month")]
        public void ThenTheFirstMonthShouldBeThisMonthRatherThanNextMonth()
        {
            Assert.AreEqual(AccountProjections.First().Month, ProjectionsStartPeriod.Month, $"Expected the first month to be {ProjectionsStartPeriod.Month} but was {AccountProjections.First().Month}");
        }

        [Then(@"the total completion amount is (.*)")]
        public void ThenTheTotalCompletionAmountIs(int amount)
        {
            Assert.AreEqual(amount, AccountProjections.Sum(c=>c.LevyFundedCompletionPayments));
        }

    }
}
