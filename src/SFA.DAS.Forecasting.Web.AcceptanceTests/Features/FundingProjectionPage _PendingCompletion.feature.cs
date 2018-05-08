﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:2.3.0.0
//      SpecFlow Generator Version:2.3.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace SFA.DAS.Forecasting.Web.AcceptanceTests.Features
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.3.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("FundingProjectionPage _PendingCompletion")]
    public partial class FundingProjectionPage_PendingCompletionFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "FundingProjectionPage _PendingCompletion.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "FundingProjectionPage _PendingCompletion", "As a Levy Employer logged into my Apprenticeship Account\r\nI want to be able to se" +
                    "e any pending completion payments", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 5
#line 6
 testRunner.Given("that I am an employer", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 7
 testRunner.And("I have logged into my Apprenticeship Account", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("AC1: Reading completion payment")]
        public virtual void AC1ReadingCompletionPayment()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("AC1: Reading completion payment", ((string[])(null)));
#line 9
this.ScenarioSetup(scenarioInfo);
#line 5
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "EmployerAccountId",
                        "LearnerId",
                        "ProviderId",
                        "ProviderName",
                        "ApprenticeshipId",
                        "ApprenticeName",
                        "CourseName",
                        "CourseLevel",
                        "StartDate",
                        "PlannedEndDate",
                        "ActualEndDate",
                        "CompletionAmount",
                        "MonthlyInstallment",
                        "NumberOfInstallments"});
            table1.AddRow(new string[] {
                        "12345",
                        "1",
                        "2",
                        "Test Provider 2",
                        "222",
                        "Apprentice Name 1",
                        "Test Provider\tTest Course 2",
                        "1",
                        "2018-03-20",
                        "2020-03-20",
                        "NULL",
                        "3000.00",
                        "500.00",
                        "24"});
            table1.AddRow(new string[] {
                        "12345",
                        "2",
                        "3",
                        "Test Provider 3",
                        "333",
                        "Apprentice Name 2",
                        "Test Provider\tTest Course 3",
                        "1",
                        "2016-03-21",
                        "2018-03-21",
                        "NULL",
                        "2000.00",
                        "250.00",
                        "24"});
#line 10
 testRunner.Given("I have generated the following commitments", ((string)(null)), table1, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "Date",
                        "Funds in",
                        "Cost Of Training",
                        "Completion Payments",
                        "Your Contribution",
                        "Government Contribution",
                        "Future Funds"});
            table2.AddRow(new string[] {
                        "Apr 18",
                        "14000",
                        "880",
                        "32200",
                        "0",
                        "0",
                        "31000"});
            table2.AddRow(new string[] {
                        "May 18",
                        "15000",
                        "880",
                        "32200",
                        "0",
                        "0",
                        "17000"});
            table2.AddRow(new string[] {
                        "Jun 18",
                        "91000",
                        "1800",
                        "10000",
                        "0",
                        "0",
                        "23000"});
#line 16
 testRunner.And("I have generated the following projections", ((string)(null)), table2, "And ");
#line 23
 testRunner.And("I\'m on the Funding projection page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 24
 testRunner.Then("Pending completion payments should be £2,000", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
