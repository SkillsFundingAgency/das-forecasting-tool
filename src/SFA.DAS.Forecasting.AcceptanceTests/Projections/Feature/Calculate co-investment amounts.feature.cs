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
namespace SFA.DAS.Forecasting.AcceptanceTests.Projections.Feature
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.3.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Calculate co-investment amounts [CI-570]")]
    public partial class CalculateCo_InvestmentAmountsCI_570Feature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Calculate co-investment amounts.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Calculate co-investment amounts [CI-570]", "\tAs an employer with a pay bill over £3 million each year and therefore must now " +
                    "pay the apprenticeship levy\r\n\tI want my completion costs to be forecast for the " +
                    "next 4 years\r\n\tSo that I can effectively forecast my account balance", ProgrammingLanguage.CSharp, ((string[])(null)));
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
#line 6
#line 7
 testRunner.Given("I\'m a levy paying employer", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "Payroll Year",
                        "Payroll Month"});
            table1.AddRow(new string[] {
                        "18-19",
                        "1"});
#line 8
 testRunner.And("the payroll period is", ((string)(null)), table1, "And ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "Scheme",
                        "Amount",
                        "Created Date"});
            table2.AddRow(new string[] {
                        "ABC-1234",
                        "3000",
                        "Today"});
#line 11
 testRunner.And("the following levy declarations have been recorded", ((string)(null)), table2, "And ");
#line 14
 testRunner.And("the current balance is 5000", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Calculate co-investment after payment run")]
        public virtual void CalculateCo_InvestmentAfterPaymentRun()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Calculate co-investment after payment run", ((string[])(null)));
#line 16
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "Apprentice Name",
                        "Course Name",
                        "Course Level",
                        "Provider Name",
                        "Start Date",
                        "Installment Amount",
                        "Completion Amount",
                        "Number Of Installments",
                        "FundingSource"});
            table3.AddRow(new string[] {
                        "Test Apprentice",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "Yesterday",
                        "2000",
                        "1200",
                        "6",
                        "CoInvestedSfa"});
            table3.AddRow(new string[] {
                        "Test Apprentice 1",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "Yesterday",
                        "2000",
                        "1200",
                        "6",
                        "CoInvestedSfa"});
            table3.AddRow(new string[] {
                        "Test Apprentice 2",
                        "Test Course 2",
                        "1",
                        "Test Provider",
                        "Yesterday",
                        "2000",
                        "1200",
                        "6",
                        "CoInvestedSfa"});
            table3.AddRow(new string[] {
                        "Test Apprentice 3",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "Yesterday",
                        "2000",
                        "1200",
                        "6",
                        "CoInvestedSfa"});
            table3.AddRow(new string[] {
                        "Test Apprentice 4",
                        "Test Course 2",
                        "1",
                        "Test Provider",
                        "Yesterday",
                        "2000",
                        "1200",
                        "6",
                        "CoInvestedSfa"});
            table3.AddRow(new string[] {
                        "Test Apprentice 5",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "Yesterday",
                        "2000",
                        "1200",
                        "6",
                        "CoInvestedSfa"});
#line 17
 testRunner.Given("the following commitments have been recorded", ((string)(null)), table3, "Given ");
#line 26
 testRunner.When("the account projection is triggered after a payment run", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 27
 testRunner.Then("the account projection should be generated", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 28
 testRunner.And("the balance should be 0", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 29
 testRunner.And("the employer co-investment amount is 10% of the negative balance", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 30
 testRunner.And("the government co-investment amount is 90% of the negative value", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
