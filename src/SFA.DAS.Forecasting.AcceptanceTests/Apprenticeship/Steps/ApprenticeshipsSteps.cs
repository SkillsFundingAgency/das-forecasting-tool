using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Forecasting.AcceptanceTests.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Forecasting.AcceptanceTests.Apprenticeship.Steps
{
    [Binding]
    public class ApprenticeshipsSteps : StepsBase
    {
        [Scope(Feature = "LoadApprenticeships")]
        [BeforeFeature(Order = 1)]
        public static void StartLevyFunction()
        {
            StartFunction("SFA.DAS.Forecasting.Commitments.Functions");
            StartFunction("SFA.DAS.Forecasting.StubApi.Functions");
        }

        [Given(@"there is following apprenticehsips in the commitments API")]
        public void GivenThereIsFollowingApprenticehsipsInTheCommitmentsAPI(Table table)
        {
            var url = Config.ApiInsertApprenticeshipsUrl.Replace("{employerAccountId}", Config.EmployerAccountId.ToString());
            TestApprenticeships = table.CreateSet<TestApprenticeship>().ToList();

            var model = new TestApprenticehsipSearchModel(TestApprenticeships);

            var data = JsonConvert.SerializeObject(model);

            var response = HttpClient.PostAsync(url, new StringContent(data, Encoding.UTF8, "application/json")).Result;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [When(@"I trigger load of apprenticeships")]
        public void WhenITriggerLoadOfApprenticeships()
        {

            var response = HttpClient.PostAsync(Config.GetApprenticeshipHttpTriggerUrl, new StringContent("", Encoding.UTF8, "application/json")).Result;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Thread.Sleep(1000);
        }

        [Then(@"there should be following commitments stored")]
        public void ThenThereShouldBeFollowingCommitmentsStored(Table table)
        {
            var expectedCommitments = table.CreateSet<TestCommitment>().ToList();
            List<Models.Commitments.CommitmentModel> commitmentsFromDb = new List<Models.Commitments.CommitmentModel>();

            WaitForIt(() =>
            {
                commitmentsFromDb = DataContext.Commitments
                        .Where(m => m.EmployerAccountId == Config.EmployerAccountId)
                        .ToList();
                return Tuple.Create(
                    commitmentsFromDb.Count() == expectedCommitments.Count, 
                    $"Found: {commitmentsFromDb.Count()} commitments in db with AccountId: {Config.EmployerAccountId}");
            }, "Not able to find any commitments");
                

            foreach(var expected in expectedCommitments)
            {
                var match = commitmentsFromDb.Single(m => m.ApprenticeshipId == expected.ApprenticeshipId);

                match.CompletionAmount.Should().Be((short)expected.CompletionAmount);
                match.MonthlyInstallment.Should().Be(expected.InstallmentAmount);
                match.NumberOfInstallments.Should().Be((short)expected.NumberOfInstallments);

                match.CourseLevel.Should().Be(expected.CourseLevel);
                match.HasHadPayment.Should().BeFalse();
                match.FundingSource.Should().Be(expected.FundingSource);
            }
        }
    }
}
