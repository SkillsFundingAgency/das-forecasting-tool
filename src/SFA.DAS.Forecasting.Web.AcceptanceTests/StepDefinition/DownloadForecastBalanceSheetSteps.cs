using Dapper;
using NUnit.Framework;
using SFA.DAS.Forecasting.Web.Automation;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Transactions;
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
        protected IDbConnection Connection => NestedContainer.GetInstance<IDbConnection>();

        [Given(@"I'm on the Funding projection page")]
        public void GivenIMOnTheFundingProjectionPage()
        {
            downloadedFilesBefore = FileManager.getCurrentDownloadFiles();

            if (!WebSite.IsLocalhost)
            {
                var accountHomepage = WebSite.NavigateToAccountHomePage();
                var financePage = accountHomepage.OpenFinance();
                var page = financePage.OpenFundingProjection();
                Set(page);
            }
            else
            {
                var page = WebSite.NavigateToFundingProjectionPage();
                Set(page);
            }
        }

        [When(@"I select download as csv")]
        public void WhenISelectDownloadAsCsv()
        {
            var page = Get<FundingProjectionPage>();
            page.DownloadProjectionsCSVButton.Click();
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
            Assert.AreEqual(readCsvHeader, "Date,Funds in,Cost of training,Completion payments,Your contribution,Government contribution,Balance");
        }

        [Then(@"column headers are downloaded for apprenticeship CSV")]
        public void ThenColumnHeadersAreDownloadedForApprenticeshipCsv()
        {
            var readCsv = File.ReadLines(newFilePath);
            var readCsvHeader = readCsv.First();
            Assert.AreEqual(readCsvHeader, "Start Date,End Date,Apprenticeship,Apprenticeship Level,Transfer to Employer,Uln,Apprentice Name,UKPRN,Provider Name,Total Cost,Monthly Training Cost,Completion Amount");
        }


        [Then(@"all of the rows have been downloaded")]
        public void ThenAllOfTheRowsHaveBeenDownloaded()
        {
            var readCsv = File.ReadLines(newFilePath);
            var lineCount = File.ReadAllLines(newFilePath).Length;
            Assert.AreEqual(7, lineCount);
        }

        protected List<TestAccountProjection> Projections { get { return Get<List<TestAccountProjection>>(); } set { Set(value); } }
        protected void DeleteAccountProjections()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId", long.Parse(Config.EmployerAccountID), DbType.Int64);
            Connection.Execute("Delete from AccountProjectionCommitment where AccountProjectionId in (Select id from AccountProjection where employerAccountId = @employerAccountId)", parameters, commandType: CommandType.Text);
            Connection.Execute("Delete from AccountProjection where employerAccountId = @employerAccountId", parameters, commandType: CommandType.Text);
        }

        public void Store(IEnumerable<TestAccountProjection> accountProjections)
        {
            var employerAccountId = long.Parse(Config.EmployerAccountID);
            using (var txScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var sql = @"Insert Into [dbo].[AccountProjection]
                           (EmployerAccountId,
                            ProjectionCreationDate,
                            ProjectionGenerationType,
                            Month,
                            Year,
                            FundsIn,
                            TotalCostOfTraining,
                            CompletionPayments,
                            CoInvestmentEmployer,
                            CoInvestmentGovernment,
                            FutureFunds)
                        Values 
                           (@employerAccountId,
                           @projectionCreationDate,
                           @projectionGenerationType,
                           @month,
                           @year,
                           @fundsIn,
                           @totalCostOfTraining,
                           @completionPayments,
                           @futureFunds,
                           @Yourcontribution,
                           @Governmentcontribution
                           )";

                foreach (var accountProjectionReadModel in accountProjections)
                {
                    var date = DateTime.Parse(accountProjectionReadModel.Date);

                    var parameters = new DynamicParameters();
                    parameters.Add("@employerAccountId", employerAccountId, DbType.Int64);
                    parameters.Add("@projectionCreationDate", DateTime.Today, DbType.DateTime);
                    parameters.Add("@projectionGenerationType", 1, DbType.Int16);
                    parameters.Add("@month", date.Month, DbType.Int16);
                    parameters.Add("@year", date.Year, DbType.Int32);
                    parameters.Add("@fundsIn", accountProjectionReadModel.FundsIn, DbType.Decimal);
                    parameters.Add("@totalCostOfTraining", accountProjectionReadModel.CostOfTraining, DbType.Decimal);
                    parameters.Add("@completionPayments", accountProjectionReadModel.CompletionPayments, DbType.Decimal);
                    parameters.Add("@Yourcontribution", accountProjectionReadModel.YourContribution, DbType.Decimal);
                    parameters.Add("@Governmentcontribution", accountProjectionReadModel.GovernmentContribution, DbType.Decimal);
                    parameters.Add("@futureFunds", accountProjectionReadModel.FutureFunds, DbType.Decimal);
                    Connection.Execute(sql, parameters, commandType: CommandType.Text);
                }
                txScope.Complete();
            }
        }    

        [Given(@"I have generated the following projections")]
        public void GivenIHaveGeneratedTheFollowingProjections(Table table)
        {
            var projections = table.CreateSet<TestAccountProjection>().ToList();
            Projections = projections;

            DeleteAccountProjections();
            Store(Projections);
        }
    }

    public class TestAccountProjection
    {
        public string Date { get; set; }
        public string FundsIn { get; set; }
        public decimal CostOfTraining { get; set; }
        public decimal CompletionPayments { get; set; }
        public decimal YourContribution { get; set; }
        public decimal GovernmentContribution { get; set; }
        public decimal FutureFunds { get; set; }
        public decimal TransferOutTotalCostOfTraining { get; set; }
    }
}
