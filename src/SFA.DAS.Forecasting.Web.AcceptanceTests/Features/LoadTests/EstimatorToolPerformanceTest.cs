using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Sfa.Automation.Framework.Extensions;
using SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition;
using SFA.DAS.Forecasting.Web.Automation;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests.Features.LoadTests
{
    public class EstimatorToolPerformanceTest : StepsBase
    {
<<<<<<< HEAD
        private static int _numerOfThreadsNotYetCompleted = 1;
        private static int numberOfThreads = 1;
=======
        private static int _numerOfThreadsNotYetCompleted = 10;
        private static int numberOfThreads = 10;
>>>>>>> 91bc7306d1caed700cafd29a268f3d5b3b1da673
        private static ManualResetEvent _doneEvent = new ManualResetEvent(false);
        private int emailIndex = 001;
        private string siteUrl = "https://pp-eas.apprenticeships.sfa.bis.gov.uk/";
        StringBuilder csv = new StringBuilder();
        private string path = "C:/mySrc/EstimatorToolPerformanceTestResults.csv";
<<<<<<< HEAD
        private string htmlPath = "C:/mySrc/EstimatorToolPerformanceTestResults.html";
=======
>>>>>>> 91bc7306d1caed700cafd29a268f3d5b3b1da673
        private float errorsCount = 0;
        private int throughput = 6;

        private decimal timeToLoadHomePage;
        private decimal timeToLoadUsedThisServiceBeforePage;
        private decimal timeToLoadLoginPage;
        private decimal timeToLoadYourAccountsPage;
        private decimal timeToLoadSainsburyPage;
        private decimal timeToLoadFinancePage;
        private decimal timeToLoadTransfersPage;
        private decimal timeToLoadStartTransfersPage;
        private decimal timeToLoadAddApprenticeshipsPage;
        private decimal timeToLoadEstimateCostsPage;
        private decimal timeToLoadEditApprenticeshipsPage;


        private List<decimal> timeToLoadHomePages = new List<decimal>();
        private List<decimal> timeToLoadUsedThisServiceBeforePages = new List<decimal>();
        private List<decimal> timeToLoadLoginPages = new List<decimal>();
        private List<decimal> timeToLoadYourAccountsPages = new List<decimal>();
        private List<decimal> timeToLoadSainsburyPages = new List<decimal>();
        private List<decimal> timeToLoadFinancePages = new List<decimal>();
        private List<decimal> timeToLoadTransfersPages = new List<decimal>();
        private List<decimal> timeToLoadStartTransfersPages = new List<decimal>();
        private List<decimal> timeToLoadAddApprenticeshipsPages = new List<decimal>();
        private List<decimal> timeToLoadEstimateCostsPages = new List<decimal>();
        private List<decimal> timeToLoadEditApprenticeshipsPages = new List<decimal>();

        private float averageTimeToLoadHomePage = 0;
        private float averageTimeToLoadUsedThisServiceBeforePage = 0;
        private float averageTimeToLoadLoginPage = 0;
        private float averageTimeToLoadYourAccountsPage = 0;
        private float averageTimeToLoadSainsburyPage = 0;
        private float averageTimeToLoadFinancePage = 0;
        private float averageTimeToLoadTransfersPage = 0;
        private float averageTimeToLoadStartTransfersPage = 0;
        private float averageTimeToLoadAddApprenticeshipsPage = 0;
        private float averageTimeToLoadEstimateCostsPage = 0;
<<<<<<< HEAD
        private float averageTimeToLoadEditApprenticeshipsPage = 0;
=======
        private float averagetimeToLoadEditApprenticeshipsPage = 0;
>>>>>>> 91bc7306d1caed700cafd29a268f3d5b3b1da673

        [Test]
        public void VerifyAddAndRemoveApprenticeships()
        {
            ThreadPool.SetMinThreads(1, 0);
            ThreadPool.SetMaxThreads(throughput, 0);
            for (int threadNumber = 0; threadNumber < numberOfThreads; threadNumber++)
                ThreadPool.QueueUserWorkItem(new WaitCallback(MeasureResponseTimeForAddingApprenticeships),
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

            foreach (var time in timeToLoadTransfersPages)
            {
                averageTimeToLoadTransfersPage = averageTimeToLoadTransfersPage + float.Parse(time.ToString());
            }
            averageTimeToLoadTransfersPage = averageTimeToLoadTransfersPage / (numberOfThreads - errorsCount);

            foreach (var time in timeToLoadStartTransfersPages)
            {
                averageTimeToLoadStartTransfersPage = averageTimeToLoadStartTransfersPage + float.Parse(time.ToString());
            }
            averageTimeToLoadStartTransfersPage = averageTimeToLoadStartTransfersPage / (numberOfThreads - errorsCount);

            foreach (var time in timeToLoadAddApprenticeshipsPages)
            {
                averageTimeToLoadAddApprenticeshipsPage = averageTimeToLoadAddApprenticeshipsPage + float.Parse(time.ToString());
            }
            averageTimeToLoadAddApprenticeshipsPage = averageTimeToLoadAddApprenticeshipsPage / (numberOfThreads - errorsCount);

            foreach (var time in timeToLoadEstimateCostsPages)
            {
                averageTimeToLoadEstimateCostsPage = averageTimeToLoadEstimateCostsPage + float.Parse(time.ToString());
            }
            averageTimeToLoadEstimateCostsPage = averageTimeToLoadEstimateCostsPage / (numberOfThreads - errorsCount);

            foreach (var time in timeToLoadEditApprenticeshipsPages)
            {
<<<<<<< HEAD
                averageTimeToLoadEditApprenticeshipsPage = averageTimeToLoadEditApprenticeshipsPage + float.Parse(time.ToString());
            }
            averageTimeToLoadEditApprenticeshipsPage = averageTimeToLoadEditApprenticeshipsPage / (numberOfThreads - errorsCount);
=======
                averagetimeToLoadEditApprenticeshipsPage = averagetimeToLoadEditApprenticeshipsPage + float.Parse(time.ToString());
            }
            averagetimeToLoadEditApprenticeshipsPage = averagetimeToLoadEditApprenticeshipsPage / (numberOfThreads - errorsCount);
>>>>>>> 91bc7306d1caed700cafd29a268f3d5b3b1da673

            csv.AppendLine("Funding Projection Performance Test Results");
            csv.AppendLine($"Total number of users - {numberOfThreads}");
            csv.AppendLine($"Throughput - {throughput - 1}");
            csv.AppendLine($"Errors count - {errorsCount}");
            csv.AppendLine($"Average Time To Load Home Page - {averageTimeToLoadHomePage} seconds");
            csv.AppendLine($"Average Time To Load Used This Service Before Page - {averageTimeToLoadUsedThisServiceBeforePage} seconds");
            csv.AppendLine($"Average Time To Load Login Page - {averageTimeToLoadLoginPage} seconds");
            csv.AppendLine($"Average Time To Load Your Accounts Page - {averageTimeToLoadYourAccountsPage} seconds");
            csv.AppendLine($"Average Time To Load Sainsbury Page - {averageTimeToLoadSainsburyPage} seconds");
            csv.AppendLine($"Average Time To Load Finance Page - {averageTimeToLoadFinancePage} seconds");
            csv.AppendLine($"Average Time To Load Transfers Page - {averageTimeToLoadTransfersPage} seconds");
            csv.AppendLine($"Average Time To Load Start Transfers Page - {averageTimeToLoadStartTransfersPage} seconds");
            csv.AppendLine($"Average Time To Load Add Apprenticeships Page - {averageTimeToLoadAddApprenticeshipsPage} seconds");
            csv.AppendLine($"Average Time To Load Estimate Costs Page - {averageTimeToLoadEstimateCostsPage} seconds");
<<<<<<< HEAD
            csv.AppendLine($"Average Time To Load Edit Apprenticeships - {averageTimeToLoadEditApprenticeshipsPage} seconds");

            new FileInfo(path).Directory.Create();
            File.WriteAllText(path, csv.ToString());

            string _filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            _filePath = Directory.GetParent(Directory.GetParent(_filePath).FullName).FullName;
            _filePath += @"\Templates\html2.html";
            string htmlTemplate = new StreamReader(_filePath).ReadToEnd();
            string report = htmlTemplate
                .Replace("%ReportName%", "Estimator Tool Performance Test")
                .Replace("%numberOfThreads%", numberOfThreads.ToString())
                .Replace("%throughput%", throughput.ToString())
                .Replace("%errorsCount%", errorsCount.ToString())
                .Replace("%1%", averageTimeToLoadHomePage.ToString())
                .Replace("%2%", averageTimeToLoadUsedThisServiceBeforePage.ToString())
                .Replace("%3%", averageTimeToLoadLoginPage.ToString())
                .Replace("%4%", averageTimeToLoadYourAccountsPage.ToString())
                .Replace("%5%", averageTimeToLoadSainsburyPage.ToString())
                .Replace("%6%", averageTimeToLoadFinancePage.ToString())
                .Replace("%7%", averageTimeToLoadTransfersPage.ToString())
                .Replace("%8%", averageTimeToLoadStartTransfersPage.ToString())
                .Replace("%9%", averageTimeToLoadAddApprenticeshipsPage.ToString())
                .Replace("%10%", averageTimeToLoadEstimateCostsPage.ToString())
                .Replace("%11%", averageTimeToLoadEditApprenticeshipsPage.ToString());

        new FileInfo(htmlPath).Directory.Create();
            File.WriteAllText(htmlPath, report);
=======
            csv.AppendLine($"Average Time To Load Edit Apprenticeships - {averagetimeToLoadEditApprenticeshipsPage} seconds");

            new FileInfo(path).Directory.Create();
            File.WriteAllText(path, csv.ToString());
>>>>>>> 91bc7306d1caed700cafd29a268f3d5b3b1da673
        }

        private void MeasureResponseTimeForAddingApprenticeships(object o)
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
                driver.FindElement(By.CssSelector("h2>a[href*='transfers']")).Click();
                timer.Stop();
                timeToLoadTransfersPage = decimal.Parse($"{timer.Elapsed.TotalSeconds}");
                timeToLoadTransfersPage = decimal.Round(timeToLoadTransfersPage, 2);

                timer.Reset();
                timer.Start();
                driver.FindElement(By.CssSelector("a[href*='start-transfer']")).Click();
                timeToLoadStartTransfersPage = decimal.Parse($"{timer.Elapsed.TotalSeconds}");
                timeToLoadStartTransfersPage = decimal.Round(timeToLoadStartTransfersPage, 2);

                timer.Reset();
                timer.Start();
                driver.FindElement(By.CssSelector("a[href*='start-redirect']")).Click();
                timeToLoadAddApprenticeshipsPage = decimal.Parse($"{timer.Elapsed.TotalSeconds}");
                timeToLoadAddApprenticeshipsPage = decimal.Round(timeToLoadAddApprenticeshipsPage, 2);

<<<<<<< HEAD
                if (!driver.Url.Contains("apprenticeship/add"))
                {
                    var secondEstimateCostsPage = new EstimateCostsPage(driver);
                    var secondIsAnyapprenticeshipExist = secondEstimateCostsPage.IsApprenticeshipsTableVisible();
                    while (secondIsAnyapprenticeshipExist)
                    {
                        secondEstimateCostsPage.RemoveApprenticeshipButton.ClickThisElement();
                        var removalPage = new RemoveApprenticeshipPage(driver);
                        removalPage.ConfirmRemoval();
                        secondIsAnyapprenticeshipExist = secondEstimateCostsPage.IsApprenticeshipsTableVisible();
                    }

                    driver.FindElement(By.CssSelector("a[href*='apprenticeship/add']")).Click();
                }
                
=======
>>>>>>> 91bc7306d1caed700cafd29a268f3d5b3b1da673
                var addApprenticeshipPage = new AddApprenticeshipsToEstimateCostPage(driver);
                addApprenticeshipPage.SelectApprenticeshipDropdown.SelectDropDown(driver,
                    "Actuary, Level: 7 (Standard)");
                addApprenticeshipPage.PageHeader.ClickThisElement();
                addApprenticeshipPage.NumberOfApprenticesInput.EnterTextInThisElement("1");
                addApprenticeshipPage.StartDateMonthInput.EnterTextInThisElement("10");
                addApprenticeshipPage.StartDateYearInput.EnterTextInThisElement("2019");

                timer.Reset();
                timer.Start();
                addApprenticeshipPage.ContinueButton.ClickThisElement();
                timeToLoadEstimateCostsPage = decimal.Parse($"{timer.Elapsed.TotalSeconds}");
                timeToLoadEstimateCostsPage = decimal.Round(timeToLoadEstimateCostsPage, 2);

                timer.Reset();
                timer.Start();
                driver.FindElement(By.CssSelector("a[href*='EditApprenticeships']")).Click();
                timeToLoadEditApprenticeshipsPage = decimal.Parse($"{timer.Elapsed.TotalSeconds}");
                timeToLoadEditApprenticeshipsPage = decimal.Round(timeToLoadEditApprenticeshipsPage, 2);

                var editApprenticeshipPage = new AddApprenticeshipsToEstimateCostPage(driver);
                editApprenticeshipPage.NumberOfApprenticesInput.Clear();
                editApprenticeshipPage.NumberOfApprenticesInput.EnterTextInThisElement("2");
                editApprenticeshipPage.ContinueButton.ClickThisElement();

                driver.FindElement(By.CssSelector("a[href*='apprenticeship/add']")).Click();

                var secondAddApprenticeshipPage = new AddApprenticeshipsToEstimateCostPage(driver);

                secondAddApprenticeshipPage.SelectApprenticeshipDropdown.SelectDropDown(driver,
                    "Abattoir Worker, Level: 2 (Standard)");
                secondAddApprenticeshipPage.PageHeader.ClickThisElement();
                secondAddApprenticeshipPage.NumberOfApprenticesInput.EnterTextInThisElement("3");
                secondAddApprenticeshipPage.StartDateMonthInput.EnterTextInThisElement("11");
                secondAddApprenticeshipPage.StartDateYearInput.EnterTextInThisElement("2020");
                secondAddApprenticeshipPage.ContinueButton.ClickThisElement();

                var estimateCostsPage = new EstimateCostsPage(driver);
                var isAnyapprenticeshipExist = estimateCostsPage.IsApprenticeshipsTableVisible();
                while (isAnyapprenticeshipExist)
                {
                    estimateCostsPage.RemoveApprenticeshipButton.ClickThisElement();
                    var removalPage = new RemoveApprenticeshipPage(driver);
                    removalPage.ConfirmRemoval();
                    isAnyapprenticeshipExist = estimateCostsPage.IsApprenticeshipsTableVisible();
                }

                timeToLoadHomePages.Add(timeToLoadHomePage);
                timeToLoadUsedThisServiceBeforePages.Add(timeToLoadUsedThisServiceBeforePage);
                timeToLoadLoginPages.Add(timeToLoadLoginPage);
                timeToLoadYourAccountsPages.Add(timeToLoadYourAccountsPage);
                timeToLoadSainsburyPages.Add(timeToLoadSainsburyPage);
                timeToLoadFinancePages.Add(timeToLoadFinancePage);
                timeToLoadTransfersPages.Add(timeToLoadTransfersPage);
                timeToLoadStartTransfersPages.Add(timeToLoadStartTransfersPage);
                timeToLoadAddApprenticeshipsPages.Add(timeToLoadAddApprenticeshipsPage);
                timeToLoadEstimateCostsPages.Add(timeToLoadEstimateCostsPage);
                timeToLoadEditApprenticeshipsPages.Add(timeToLoadEditApprenticeshipsPage);
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