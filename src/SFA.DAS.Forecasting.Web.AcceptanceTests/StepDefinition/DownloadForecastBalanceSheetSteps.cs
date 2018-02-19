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
        private string newFilePath;

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
            newFilePath = currentFiles.Except(downloadedFilesBefore).FirstOrDefault();
            Assert.NotNull(newFilePath);
            this.targetFilename = Path.GetFileName(newFilePath);
        }

        [Then(@"the downloaded filename is in the format esfaforecast_yyyymmddhhmmss")]
        public void ThenTheDownloadedFilenameIsInTheFormatEsfaforecast_Yyyymmddhhmmss()
        {
            string pattern = @"esfaforecast_\d{4}\d{2}\d{2}\d{2}\d{2}\d{2}";
            Assert.IsTrue(Regex.IsMatch(this.targetFilename, pattern, RegexOptions.ECMAScript));

           
        }
        [Then(@"column headers are downloaded")]
        public void ThenColumnHeadersAreDownloaded()
        {
            var readCsv = File.ReadLines(newFilePath);
            var readCsvHeader = readCsv.First();
            Assert.True(readCsvHeader.Contains("Date,LevyCredit,CostOfTraining,CompletionPayments,FutureFunds"),"ERROR: File header titles is {0}", readCsv.First());
                        
            if (File.Exists(newFilePath))
            {
                File.Delete(newFilePath);
            }
            
        }


        [Then(@"all of the rows have been downloaded")]
        public void ThenAllOfTheRowsHaveBeenDownloaded()
        {
            //load the text file
            //ignore the header line
            //check that the file has the same number of rows as Projections
            //make sure each row in Projections exists in the file
            
            
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
