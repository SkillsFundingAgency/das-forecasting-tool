using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition;
using SFA.DAS.Forecasting.Web.Automation;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests.Features.LoadTests
{
    public class FundingProjectionPerformanceTest : StepsBase
    {
        private static int _numerOfThreadsNotYetCompleted = 10;
        private static int numberOfThreads = 10;
        private static ManualResetEvent _doneEvent = new ManualResetEvent(false);
        private int emailIndex = 001;
        private string siteUrl = "https://pp-eas.apprenticeships.sfa.bis.gov.uk/";
        StringBuilder csv = new StringBuilder();
        private string path = "C:/mySrc/FundingProjectionPerformanceTestResults.csv";
        private string htmlPath = "C:/mySrc/FundingProjectionPerformanceTestResults.html";
        private float errorsCount = 0;
        private int throughput = 6;

        private decimal timeToLoadHomePage;
        private decimal timeToLoadUsedThisServiceBeforePage;
        private decimal timeToLoadLoginPage;
        private decimal timeToLoadYourAccountsPage;
        private decimal timeToLoadSainsburyPage;
        private decimal timeToLoadFinancePage;
        private decimal timeToLoadProjectionsPage;

        private List<decimal> timeToLoadHomePages = new List<decimal>();
        private List<decimal> timeToLoadUsedThisServiceBeforePages = new List<decimal>();
        private List<decimal> timeToLoadLoginPages = new List<decimal>();
        private List<decimal> timeToLoadYourAccountsPages = new List<decimal>();
        private List<decimal> timeToLoadSainsburyPages = new List<decimal>();
        private List<decimal> timeToLoadFinancePages = new List<decimal>();
        private List<decimal> timeToLoadProjectionsPages = new List<decimal>();

        private float averageTimeToLoadHomePage = 0;
        private float averageTimeToLoadUsedThisServiceBeforePage = 0;
        private float averageTimeToLoadLoginPage = 0;
        private float averageTimeToLoadYourAccountsPage = 0;
        private float averageTimeToLoadSainsburyPage = 0;
        private float averageTimeToLoadFinancePage = 0;
        private float averageTimeToLoadProjectionsPage = 0;

        [Test]
        public void VerifyFundingProjectionCsvLinks()
        {
            ThreadPool.SetMinThreads(1, 0);
            ThreadPool.SetMaxThreads(throughput, 0);
            for (int threadNumber = 0; threadNumber < numberOfThreads; threadNumber++)
                ThreadPool.QueueUserWorkItem(new WaitCallback(MeasureResponseTimeForFundingProjection),
                    (object)threadNumber);

            _doneEvent.WaitOne();

            foreach (var time in timeToLoadHomePages)
            {
                averageTimeToLoadHomePage = averageTimeToLoadHomePage + float.Parse(time.ToString());
            }
            averageTimeToLoadHomePage = averageTimeToLoadHomePage / (numberOfThreads - errorsCount);

            foreach (var time in timeToLoadUsedThisServiceBeforePages)
            {
                averageTimeToLoadUsedThisServiceBeforePage = averageTimeToLoadUsedThisServiceBeforePage + float.Parse(time.ToString());
            }
            averageTimeToLoadUsedThisServiceBeforePage = averageTimeToLoadUsedThisServiceBeforePage / (numberOfThreads - errorsCount);

            foreach (var time in timeToLoadLoginPages)
            {
                averageTimeToLoadLoginPage = averageTimeToLoadLoginPage + float.Parse(time.ToString());
            }
            averageTimeToLoadLoginPage = averageTimeToLoadLoginPage / (numberOfThreads - errorsCount);

            foreach (var time in timeToLoadYourAccountsPages)
            {
                averageTimeToLoadYourAccountsPage = averageTimeToLoadYourAccountsPage + float.Parse(time.ToString());
            }
            averageTimeToLoadYourAccountsPage = averageTimeToLoadYourAccountsPage / (numberOfThreads - errorsCount);

            foreach (var time in timeToLoadSainsburyPages)
            {
                averageTimeToLoadSainsburyPage = averageTimeToLoadSainsburyPage + float.Parse(time.ToString());
            }
            averageTimeToLoadSainsburyPage = averageTimeToLoadSainsburyPage / (numberOfThreads - errorsCount);

            foreach (var time in timeToLoadFinancePages)
            {
                averageTimeToLoadFinancePage = averageTimeToLoadFinancePage + float.Parse(time.ToString());
            }
            averageTimeToLoadFinancePage = averageTimeToLoadFinancePage / (numberOfThreads - errorsCount);

            foreach (var time in timeToLoadProjectionsPages)
            {
                averageTimeToLoadProjectionsPage = averageTimeToLoadProjectionsPage + float.Parse(time.ToString());
            }
            averageTimeToLoadProjectionsPage = averageTimeToLoadProjectionsPage / (numberOfThreads - errorsCount);

            csv.AppendLine("Funding Projection Performance Test Results");
            csv.AppendLine(
                $"Total number of users - {numberOfThreads}");
            csv.AppendLine($"Throughput - {throughput - 1}");
            csv.AppendLine($"Errors count - {errorsCount}");
            csv.AppendLine($"Average Time To Load Home Page - {averageTimeToLoadHomePage} seconds");
            csv.AppendLine($"Average Time To Load Used This Service Before Page - {averageTimeToLoadUsedThisServiceBeforePage} seconds");
            csv.AppendLine($"Average Time To Load Login Page - {averageTimeToLoadLoginPage} seconds");
            csv.AppendLine($"Average Time To Load Your Accounts Page - {averageTimeToLoadYourAccountsPage} seconds");
            csv.AppendLine($"Average Time To Load Sainsbury Page - {averageTimeToLoadSainsburyPage} seconds");
            csv.AppendLine($"Average Time To Load Finance Page - {averageTimeToLoadFinancePage} seconds");
            csv.AppendLine($"Average Time To Load Projections Page - {averageTimeToLoadProjectionsPage} seconds");

            new FileInfo(path).Directory.Create();
            File.WriteAllText(path, csv.ToString());

            string _filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            _filePath = Directory.GetParent(Directory.GetParent(_filePath).FullName).FullName;
            _filePath += @"\Templates\html.html";
            string htmlTemplate = new StreamReader(_filePath).ReadToEnd();
            string report = htmlTemplate
                .Replace("%ReportName%", "Funding Projection Performance Test")
                .Replace("%numberOfThreads%", numberOfThreads.ToString())
                .Replace("%throughput%", throughput.ToString())
                .Replace("%errorsCount%", errorsCount.ToString())
                .Replace("%1%", averageTimeToLoadHomePage.ToString())
                .Replace("%2%", averageTimeToLoadUsedThisServiceBeforePage.ToString())
                .Replace("%3%", averageTimeToLoadLoginPage.ToString())
                .Replace("%4%", averageTimeToLoadYourAccountsPage.ToString())
                .Replace("%5%", averageTimeToLoadSainsburyPage.ToString())
                .Replace("%6%", averageTimeToLoadFinancePage.ToString())
                .Replace("%7%", averageTimeToLoadProjectionsPage.ToString());

            new FileInfo(htmlPath).Directory.Create();
            File.WriteAllText(htmlPath, report);
        }

        private void MeasureResponseTimeForFundingProjection(object o)
        {
            string emailString = emailIndex.ToString("D3");
            emailIndex++;
            IWebDriver driver = new ChromeDriver();

            try
            {
                driver.Manage().Window.Maximize();

                Stopwatch timer = new Stopwatch();
                timer.Start();
                driver.Navigate().GoToUrl(siteUrl);
                timer.Stop();
                timeToLoadHomePage = decimal.Parse($"{timer.Elapsed.TotalSeconds}");
                timeToLoadHomePage = decimal.Round(timeToLoadHomePage, 2);

                timer.Reset();
                timer.Start();
                var homePage = new HomePage(driver);
                homePage.StartButton.ClickThisElement();
                timer.Stop();
                timeToLoadUsedThisServiceBeforePage = decimal.Parse($"{timer.Elapsed.TotalSeconds}");
                timeToLoadUsedThisServiceBeforePage = decimal.Round(timeToLoadUsedThisServiceBeforePage, 2);

                homePage.UsedServiceBefore.CheckThisRadioButton();

                timer.Reset();
                timer.Start();
                homePage.Continue.ClickThisElement();
                timer.Stop();
                timeToLoadLoginPage = decimal.Parse($"{timer.Elapsed.TotalSeconds}");
                timeToLoadLoginPage = decimal.Round(timeToLoadLoginPage, 2);

                timer.Reset();
                timer.Start();
                var loginPage = new LoginPage(driver);
                loginPage.LoginAsUser($"perfUser{emailString}@loadtest.local", "Pa55word");
                timer.Stop();
                timeToLoadYourAccountsPage = decimal.Parse($"{timer.Elapsed.TotalSeconds}");
                timeToLoadYourAccountsPage = decimal.Round(timeToLoadYourAccountsPage, 2);

                timer.Reset();
                timer.Start();
                driver.FindElement(By.CssSelector("[title*='SAINSBURY']")).Click();
                timer.Stop();
                timeToLoadSainsburyPage = decimal.Parse($"{timer.Elapsed.TotalSeconds}");
                timeToLoadSainsburyPage = decimal.Round(timeToLoadSainsburyPage, 2);

                timer.Reset();
                timer.Start();
                driver.FindElement(By.CssSelector("h2>a[href*='finance']")).Click();
                timer.Stop();
                timeToLoadFinancePage = decimal.Parse($"{timer.Elapsed.TotalSeconds}");
                timeToLoadFinancePage = decimal.Round(timeToLoadFinancePage, 2);


                timer.Reset();
                timer.Start();
                driver.FindElement(By.CssSelector("h2>a[href*='projections']")).Click();
                timer.Stop();
                timeToLoadProjectionsPage = decimal.Parse($"{timer.Elapsed.TotalSeconds}");
                timeToLoadProjectionsPage = decimal.Round(timeToLoadProjectionsPage, 2);

                Assert.IsTrue(driver.FindElement(By.Id("apprenticeship_csvdownload")).Displayed);
                Assert.IsTrue(driver.FindElement(By.Id("projections_csvdownload")).Displayed);

                timeToLoadHomePages.Add(timeToLoadHomePage);
                timeToLoadUsedThisServiceBeforePages.Add(timeToLoadUsedThisServiceBeforePage);
                timeToLoadLoginPages.Add(timeToLoadLoginPage);
                timeToLoadYourAccountsPages.Add(timeToLoadYourAccountsPage);
                timeToLoadSainsburyPages.Add(timeToLoadSainsburyPage);
                timeToLoadFinancePages.Add(timeToLoadFinancePage);
                timeToLoadProjectionsPages.Add(timeToLoadProjectionsPage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                errorsCount++;
            }
            finally
            {
                driver.Dispose();
                if (Interlocked.Decrement(ref _numerOfThreadsNotYetCompleted) == 0)
                    _doneEvent.Set();
            }
        }
    }
}
