using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NUnit.Framework;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Web.AcceptanceTests.Helpers;
using SFA.DAS.Forecasting.Web.Automation;
using SFA.DAS.Forecasting.Web.Automation.Estimation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition.Estimation
{
    public class EditApprenticehipSteps : StepsBase
    {
        [Given(@"I have an estimated apprenticeships record")]
        public async Task GivenIHaveAnEstimatedApprenticeshipsRecord()
        {
            await UpsertStandard();
            await UpsertApprenticeshipModel();
        }

        [When(@"I navigate to the Estimated costs page")]
        public void WhenINavigateToTheEstimatedCostsPage()
        {
            var costsPage = WebSite.NavigateToEstimageCostsPage();
            Set(costsPage);
        }

        [When(@"I click on the '(.*)' link")]
        public void WhenIClickOnTheLink(string p0)
        {
            var page = Get<EstimateCostsPage>();
            var editPage = page.EditApprenticeships();
            Set(editPage);
        }

        [Then(@"I am on the edit apprenticeship page")]
        public void ThenIAmOnTheEditApprenticeshipPage()
        {
            var page = Get<EditApprenticeshipsPage>();
            Assert.AreEqual("Edit apprenticeships in your current estimate", page.Heading.Text);
        }


        [When(@"I edit number of apprenticeship to be (.*)")]
        public void WhenIEditNumberOfApprenticeshipToBe(int noOfApprenticeships)
        {
            var page = Get<EditApprenticeshipsPage>();
            page.NumberOfMonthsInput.Clear();
            page.NumberOfMonthsInput.EnterTextInThisElement(noOfApprenticeships.ToString());
            page.Heading.Click();
        }

        [Then(@"total cost will be (.*)")]
        public void ThenTotalCostWillBe(string p0)
        {
            var page = Get<EditApprenticeshipsPage>();

            var totalCostText  = page.TotalCostInput.GetAttribute("value");

            Assert.AreEqual(p0,  totalCostText);
        }

        [When(@"I change the start date to be one year later")]
        public void WhenIChangeTheStartDateToBeOneYearLater()
        {
            var page = Get<EditApprenticeshipsPage>();
            if(int.TryParse(page.YearInput.GetAttribute("value"), out int y))
            {
                page.YearInput.Clear();
                page.YearInput.EnterTextInThisElement($"{y + 1}");
            }
            else
            {
                Assert.Fail($"Not able to parse year input field. -- ({page.YearInput.GetAttribute("value")})");
            }
        }


        //private 

        private async Task UpsertApprenticeshipModel()
        {
            var model = new AccountEstimationModel
            {
                Id = "12345",
                EstimationName = "default",
                EmployerAccountId = 12345,
                Apprenticeships = new List<VirtualApprenticeship>
                {
                    new VirtualApprenticeship
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        ApprenticesCount = 2,
                        CourseId = "rockstar-999",
                        CourseTitle = "Rockstar Developer",
                        Level = 3,
                        StartDate = new DateTime(DateTime.Now.Year + 1, 2, 1),
                        TotalCost = 10000,
                        TotalCompletionAmount = 800,
                        TotalInstallments = 24,
                        TotalInstallmentAmount = 700,
                        FundingSource = Models.Payments.FundingSource.Transfer
                    }
                }

            };

            var document = new
            {
                id = $"{nameof(AccountEstimationModel)}-{model.Id}",
                type = $"{typeof(AccountEstimationModel)}",
                Document = model
            };

            await Run(async (c, dc) => 
            {
                await c.UpsertDocumentAsync(dc.SelfLink, document);
            });
        }

        private async Task UpsertStandard()
        {
            var course = 
                new ApprenticeshipCourse
                {
                    Id = "rockstar-999",
                    Title = "Rockstar Developer",
                    CourseType = ApprenticeshipCourseType.Standard,
                    Duration = 12,
                    FundingCap = 10000,
                    FundingPeriods = new List<FundingPeriod>
                    {
                        new FundingPeriod{ EffectiveFrom = DateTime.Now.AddYears(-10), EffectiveTo = new DateTime(DateTime.Now.Year + 1, 4, 30), FundingCap = 5000 },
                        new FundingPeriod{ EffectiveFrom = new DateTime(DateTime.Now.Year + 1, 5, 1), EffectiveTo = null, FundingCap = 10000 }
                    },
                    Level = 5
                };

            var document = new
            {
                id = $"ApprenticeshipCourse-{course.Id}",
                type = $"{typeof(ApprenticeshipCourse)}",
                Document = course
            };

            await Run(async (client, documentCollection) => 
            {
                await client.UpsertDocumentAsync(documentCollection.SelfLink, document);
            });
        }

        private async Task DeleteTestData()
        {
            await Run(async (c, dc) => {
                var link = UriFactory.CreateDocumentUri("Forecasting", "ForecastingDev", "9098dd33-aa91-44a0-b0cc-0665c3294073");
                await c.DeleteDocumentAsync(link);
            });
        }

        public async Task Run(Func<DocumentClient, DocumentCollection, Task> func)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["CosmosDbConnectionString"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("No 'DocumentConnectionString' connection string found.");
            var documentConnectionString = new DocumentSessionConnectionString { ConnectionString = connectionString };


            using (var client = new DocumentClient(new Uri(documentConnectionString.AccountEndpoint), documentConnectionString.AccountKey))
            {
                var documentCollection = client
                    .ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(documentConnectionString.Database, documentConnectionString.Collection))
                    .Result.Resource;
                await func(client, documentCollection);
            }
        }
    }
}
