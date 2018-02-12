using NUnit.Framework;
using SFA.DAS.Forecasting.Web.Automation;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using TechTalk.SpecFlow;

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
        }
    }
}
