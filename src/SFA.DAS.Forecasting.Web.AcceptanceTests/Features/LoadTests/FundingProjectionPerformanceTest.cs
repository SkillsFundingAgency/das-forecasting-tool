using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
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
        private static int _numerOfThreadsNotYetCompleted = 20;
        private static int numberOfThreads = 20;
        private static ManualResetEvent _doneEvent = new ManualResetEvent(false);
        private int emailIndex = 001;
        private string siteUrl = "https://pp-eas.apprenticeships.sfa.bis.gov.uk/";
        StringBuilder csv = new StringBuilder();
        private string path = "C:/mySrc/FundingProjectionCsvLinksResults.csv";

        [Test]
        public void VerifyFundingProjectionCsvLinks()
        {
            ThreadPool.SetMinThreads(1, 0);
            ThreadPool.SetMaxThreads(11, 0);
            for (int threadNumber = 0; threadNumber < numberOfThreads; threadNumber++)
                ThreadPool.QueueUserWorkItem(new WaitCallback(MeasureResponseTimeForFundingProjection),
                    (object)threadNumber);

            _doneEvent.WaitOne();

            new FileInfo(path).Directory.Create();
            File.WriteAllText(path, csv.ToString());
        }

        private void MeasureResponseTimeForFundingProjection(object o)
        {
            string emailString = emailIndex.ToString("D3");
            emailIndex++;
            IWebDriver driver = new ChromeDriver();

            try
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();

                driver.Manage().Window.Maximize();
                driver.Navigate().GoToUrl(siteUrl);
                var homePage = new HomePage(driver);
                homePage.StartButton.ClickThisElement();
                homePage.UsedServiceBefore.CheckThisRadioButton();
                homePage.Continue.ClickThisElement();

                var loginPage = new LoginPage(driver);
                loginPage.LoginAsUser($"perfUser{emailString}@loadtest.local", "Pa55word");

                driver.FindElement(By.CssSelector("[title*='SAINSBURY']")).Click();

                driver.FindElement(By.CssSelector("h2>a[href*='finance']")).Click();

                driver.FindElement(By.CssSelector("h2>a[href*='projections']")).Click();

                Assert.IsTrue(driver.FindElement(By.Id("apprenticeship_csvdownload")).Displayed);
                Assert.IsTrue(driver.FindElement(By.Id("projections_csvdownload")).Displayed);

                timer.Stop();
                decimal time = decimal.Parse($"{timer.Elapsed.TotalSeconds}");
                time = decimal.Round(time, 2);
                csv.AppendLine($"perfUser{emailString}@loadtest.local, {time}");
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
