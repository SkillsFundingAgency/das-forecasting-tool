using Microsoft.VisualBasic.FileIO;
using NUnit.Framework;
using SFA.DAS.Forecasting.ReadModel.Projections;
using SFA.DAS.Forecasting.Web.Automation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;


namespace SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition
{
    [Binding]
    public class DownloadForecastBalanceSheetSteps : StepsBase
    {
        private string[] downloadedFilesBefore;
        private string targetFilename;

        [Given(@"I'm on the Funding projection page")]
        public void GivenIMOnTheFundingProjectionPage()
        {
            this.downloadedFilesBefore = FileManager.getCurrentDownloadFiles();
            var page = WebSite.NavigateToFundingProjectionPage();
            Set(page);
        }

        [When(@"I select download as csv")]
        public void WhenISelectDownloadAsCsv()
        {
            var page = Get<FundingProjectionPage>();
            page.DownloadCSVButton.Click();
        }

        [Then(@"the csv should be downloaded")]
        public void ThenTheCsvShouldBeDownloaded()
        {
            Thread.Sleep(1000);
            var currentFiles = FileManager.getCurrentDownloadFiles();
            var newFilePath = currentFiles.Except(downloadedFilesBefore).FirstOrDefault();
            Assert.NotNull(newFilePath);
            this.targetFilename = Path.GetFileName(newFilePath);
        }

        [Then(@"the downloaded filename is in the format esfaforecast_yyyymmddhhmmss")]
        public void ThenTheDownloadedFilenameIsInTheFormatEsfaforecast_Yyyymmddhhmmss()
        {
            string pattern = @"esfaforecast_\d{4}\d{2}\d{2}\d{2}\d{2}\d{2}";
            Assert.IsTrue(Regex.IsMatch(this.targetFilename, pattern, RegexOptions.ECMAScript));

            if (File.Exists(this.targetFilename))
            {
                File.Delete(this.targetFilename);
            }
        }
        [Then(@"column headers are downloaded")]
        public void ThenColumnHeadersAreDownloaded()
        {
            using (TextFieldParser parser = new TextFieldParser(this.targetFilename))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                string[] fields = parser.ReadFields();
                fields = fields.Select((field) => field.Trim()).ToArray();
                var expected = new string[] { "Date", "LevyCredit", "CostOfTraining", "CompletionPayments", "Future Funds" };
                Assert.AreEqual(fields.Length, 5);
                foreach (var header in expected)
                {
                    Assert.Contains(header, fields);
                }

            }

        }


        [Then(@"all of the rows have been downloaded")]
        public void ThenAllOfTheRowsHaveBeenDownloaded()
        {
            //load the text file
            //ignore the header line
            //check that the file has the same number of rows as Projections
            //make sure each row in Projections exists in the file
            if (File.Exists(this.targetFilename))
            {
                File.Delete(this.targetFilename);
            }
            
            ScenarioContext.Current.Pending();
        }

        protected List<TestAccountProjection> Projections { get { return Get<List<TestAccountProjection>>(); } set { Set(value); } }

        [Given(@"I have generated the following projections")]
        public void GivenIHaveGeneratedTheFollowingProjections(Table table)
        {
            var projections = table.CreateSet<TestAccountProjection>().ToList();
            Projections = projections;
        }

        public class TestAccountProjection
        {
            public string Date { get; set; }
            public string FundsIn { get; set; }
            public decimal TotalCostOfTraining { get; set; }
            public decimal CompletionPayments { get; set; }
            public decimal FutureFunds { get; set; }
        }
    }
}
