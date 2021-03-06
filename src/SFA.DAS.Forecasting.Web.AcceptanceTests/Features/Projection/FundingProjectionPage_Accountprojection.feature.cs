﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:2.4.0.0
//      SpecFlow Generator Version:2.4.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace SFA.DAS.Forecasting.Web.AcceptanceTests.Features.Projection
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.4.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("FundingProjectionPage _Accountprojection")]
    public partial class FundingProjectionPage_AccountprojectionFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "FundingProjectionPage_Accountprojection.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "FundingProjectionPage _Accountprojection", @"As a Levy Employer logged into my Apprenticeship Account
I want to be able to see my current levy balance, credit and apprenticeship commitments displayed as a forecast across multiple future periods.
So that I can explore a variety of possible future scenarios and better plan my future levy spend and apprenticeships", ProgrammingLanguage.CSharp, ((string[])(null)));
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
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 6
#line 7
 testRunner.Given("that I am an employer", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 8
 testRunner.And("I have logged into my Apprenticeship Account", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("FundingProjectionPageAC1: Forecast data is displayed correctly when forecast betw" +
            "een payments made and 23rd of month")]
        public virtual void FundingProjectionPageAC1ForecastDataIsDisplayedCorrectlyWhenForecastBetweenPaymentsMadeAnd23RdOfMonth()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("FundingProjectionPageAC1: Forecast data is displayed correctly when forecast betw" +
                    "een payments made and 23rd of month", null, ((string[])(null)));
#line 10
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "Date",
                        "Funds in",
                        "Cost Of Training",
                        "Completion Payments",
                        "Your Contribution",
                        "Government Contribution",
                        "Future Funds"});
            table1.AddRow(new string[] {
                        "Jul 18",
                        "21000",
                        "2350",
                        "50000",
                        "0",
                        "0",
                        "23000"});
            table1.AddRow(new string[] {
                        "Aug 18",
                        "45200",
                        "850",
                        "45000",
                        "0",
                        "0",
                        "1000"});
            table1.AddRow(new string[] {
                        "Sep 18",
                        "55000",
                        "700",
                        "37880",
                        "0",
                        "0",
                        "12000"});
            table1.AddRow(new string[] {
                        "Oct 18",
                        "42000",
                        "700",
                        "37880",
                        "0",
                        "0",
                        "1000"});
            table1.AddRow(new string[] {
                        "Nov 18",
                        "22000",
                        "1800",
                        "45000",
                        "0",
                        "0",
                        "5000"});
            table1.AddRow(new string[] {
                        "Dec 18",
                        "42000",
                        "1400",
                        "10000",
                        "0",
                        "0",
                        "4000"});
            table1.AddRow(new string[] {
                        "Jan 19",
                        "41000",
                        "2000",
                        "10000",
                        "0",
                        "0",
                        "1000"});
            table1.AddRow(new string[] {
                        "Feb 19",
                        "10000",
                        "1800",
                        "10000",
                        "0",
                        "0",
                        "1000"});
            table1.AddRow(new string[] {
                        "Mar 19",
                        "15000",
                        "1800",
                        "45000",
                        "0",
                        "0",
                        "31000"});
            table1.AddRow(new string[] {
                        "Apr 19",
                        "42500",
                        "2100",
                        "10000",
                        "0",
                        "0",
                        "1000"});
#line 11
  testRunner.Given("I have generated the following projections", ((string)(null)), table1, "Given ");
#line 25
  testRunner.And("I\'m on the Funding projection page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 26
  testRunner.When("the Account projection is displayed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 27
  testRunner.Then("the Account projection has the correct columns without Co-Investment", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 28
  testRunner.And("the first month displayed is the next calendar month", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 29
  testRunner.And("there are months up to \'Apr 19\' displayed in the forecast", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 30
  testRunner.And("the data is displayed correctly in each column", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("FundingProjectionPageAC2: Forecast data is displayed correctly when forecast betw" +
            "een 23rd of month until end of month")]
        public virtual void FundingProjectionPageAC2ForecastDataIsDisplayedCorrectlyWhenForecastBetween23RdOfMonthUntilEndOfMonth()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("FundingProjectionPageAC2: Forecast data is displayed correctly when forecast betw" +
                    "een 23rd of month until end of month", null, ((string[])(null)));
#line 34
  this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
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
                        "Jul 18",
                        "21000",
                        "2350",
                        "50000",
                        "0",
                        "0",
                        "23000"});
            table2.AddRow(new string[] {
                        "Aug 18",
                        "45200",
                        "850",
                        "45000",
                        "0",
                        "0",
                        "1000"});
            table2.AddRow(new string[] {
                        "Sep 18",
                        "55000",
                        "700",
                        "37880",
                        "0",
                        "0",
                        "12000"});
            table2.AddRow(new string[] {
                        "Oct 18",
                        "42000",
                        "700",
                        "37880",
                        "0",
                        "0",
                        "1000"});
            table2.AddRow(new string[] {
                        "Nov 18",
                        "22000",
                        "1800",
                        "45000",
                        "0",
                        "0",
                        "5000"});
            table2.AddRow(new string[] {
                        "Dec 18",
                        "42000",
                        "1400",
                        "10000",
                        "0",
                        "0",
                        "4000"});
            table2.AddRow(new string[] {
                        "Jan 19",
                        "41000",
                        "2000",
                        "10000",
                        "0",
                        "0",
                        "1000"});
            table2.AddRow(new string[] {
                        "Feb 19",
                        "10000",
                        "1800",
                        "10000",
                        "0",
                        "0",
                        "1000"});
            table2.AddRow(new string[] {
                        "Mar 19",
                        "15000",
                        "1800",
                        "45000",
                        "0",
                        "0",
                        "31000"});
            table2.AddRow(new string[] {
                        "Apr 19",
                        "42500",
                        "2100",
                        "10000",
                        "0",
                        "0",
                        "1000"});
#line 35
  testRunner.Given("I have generated the following projections", ((string)(null)), table2, "Given ");
#line 49
  testRunner.And("I\'m on the Funding projection page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 50
  testRunner.When("the Account projection is displayed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 51
  testRunner.Then("the Account projection has the correct columns without Co-Investment", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 52
  testRunner.And("the first month displayed is the next calendar month", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 53
  testRunner.And("there are months up to \'Apr 19\' displayed in the forecast", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 54
  testRunner.And("the data is displayed correctly in each column", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("FundingProjectionPageAC3: Forecast data is displayed correctly when forecast betw" +
            "een 1st of month until next payments made")]
        public virtual void FundingProjectionPageAC3ForecastDataIsDisplayedCorrectlyWhenForecastBetween1StOfMonthUntilNextPaymentsMade()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("FundingProjectionPageAC3: Forecast data is displayed correctly when forecast betw" +
                    "een 1st of month until next payments made", null, ((string[])(null)));
#line 58
  this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "Date",
                        "Funds in",
                        "Cost Of Training",
                        "Completion Payments",
                        "Your Contribution",
                        "Government Contribution",
                        "Future Funds"});
            table3.AddRow(new string[] {
                        "Jul 18",
                        "21000",
                        "2350",
                        "50000",
                        "0",
                        "0",
                        "23000"});
            table3.AddRow(new string[] {
                        "Aug 18",
                        "45200",
                        "850",
                        "45000",
                        "0",
                        "0",
                        "1000"});
            table3.AddRow(new string[] {
                        "Sep 18",
                        "55000",
                        "700",
                        "37880",
                        "0",
                        "0",
                        "12000"});
            table3.AddRow(new string[] {
                        "Oct 18",
                        "42000",
                        "700",
                        "37880",
                        "0",
                        "0",
                        "1000"});
            table3.AddRow(new string[] {
                        "Nov 18",
                        "22000",
                        "1800",
                        "45000",
                        "0",
                        "0",
                        "5000"});
            table3.AddRow(new string[] {
                        "Dec 18",
                        "42000",
                        "1400",
                        "10000",
                        "0",
                        "0",
                        "4000"});
            table3.AddRow(new string[] {
                        "Jan 19",
                        "41000",
                        "2000",
                        "10000",
                        "0",
                        "0",
                        "1000"});
            table3.AddRow(new string[] {
                        "Feb 19",
                        "10000",
                        "1800",
                        "10000",
                        "0",
                        "0",
                        "1000"});
            table3.AddRow(new string[] {
                        "Mar 19",
                        "15000",
                        "1800",
                        "45000",
                        "0",
                        "0",
                        "31000"});
            table3.AddRow(new string[] {
                        "Apr 19",
                        "42500",
                        "2100",
                        "10000",
                        "0",
                        "0",
                        "1000"});
#line 59
  testRunner.Given("I have generated the following projections", ((string)(null)), table3, "Given ");
#line 73
  testRunner.And("I\'m on the Funding projection page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 74
  testRunner.When("the Account projection is displayed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 75
  testRunner.Then("the Account projection has the correct columns without Co-Investment", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 76
  testRunner.And("the first month displayed is the next calendar month", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 77
  testRunner.And("there are months up to \'Apr 19\' displayed in the forecast", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 78
  testRunner.And("the data is displayed correctly in each column", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Hide Co-investment columns")]
        public virtual void HideCo_InvestmentColumns()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Hide Co-investment columns", null, ((string[])(null)));
#line 80
  this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "Date",
                        "Funds in",
                        "Cost Of Training",
                        "Completion Payments",
                        "Your Contribution",
                        "Government Contribution",
                        "Future Funds"});
            table4.AddRow(new string[] {
                        "Apr 18",
                        "14000",
                        "880",
                        "3220",
                        "0",
                        "0",
                        "32000"});
            table4.AddRow(new string[] {
                        "May 18",
                        "15000",
                        "880",
                        "3220",
                        "0",
                        "0",
                        "18000"});
            table4.AddRow(new string[] {
                        "Jun 18",
                        "91000",
                        "1800",
                        "1000",
                        "0",
                        "0",
                        "23000"});
            table4.AddRow(new string[] {
                        "Jul 18",
                        "21000",
                        "2350",
                        "5000",
                        "0",
                        "0",
                        "23000"});
            table4.AddRow(new string[] {
                        "Aug 18",
                        "45200",
                        "850",
                        "4500",
                        "0",
                        "0",
                        "1000"});
            table4.AddRow(new string[] {
                        "Sep 18",
                        "55000",
                        "700",
                        "3788",
                        "0",
                        "0",
                        "12000"});
            table4.AddRow(new string[] {
                        "Oct 18",
                        "42000",
                        "700",
                        "3788",
                        "0",
                        "0",
                        "1000"});
            table4.AddRow(new string[] {
                        "Nov 18",
                        "22000",
                        "1800",
                        "4500",
                        "0",
                        "0",
                        "5000"});
            table4.AddRow(new string[] {
                        "Dec 18",
                        "42000",
                        "1400",
                        "1000",
                        "0",
                        "0",
                        "4000"});
            table4.AddRow(new string[] {
                        "Jan 19",
                        "41000",
                        "2000",
                        "1000",
                        "0",
                        "0",
                        "1000"});
            table4.AddRow(new string[] {
                        "Feb 19",
                        "10000",
                        "1800",
                        "1000",
                        "0",
                        "0",
                        "1000"});
            table4.AddRow(new string[] {
                        "Mar 19",
                        "15000",
                        "1800",
                        "4500",
                        "0",
                        "0",
                        "31000"});
            table4.AddRow(new string[] {
                        "Apr 19",
                        "42500",
                        "2100",
                        "1000",
                        "0",
                        "0",
                        "1000"});
#line 81
  testRunner.Given("I have generated the following projections", ((string)(null)), table4, "Given ");
#line 97
  testRunner.And("I\'m on the Funding projection page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 98
  testRunner.When("the Account projection is displayed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 99
  testRunner.Then("the Account projection has the correct columns without Co-Investment", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 100
  testRunner.And("there are months up to \'Apr 19\' displayed in the forecast", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("FundingProjection completion payments overdue")]
        public virtual void FundingProjectionCompletionPaymentsOverdue()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("FundingProjection completion payments overdue", null, ((string[])(null)));
#line 102
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line 103
 testRunner.Given("I have completion payments of £ 2401 on commitments without stop date", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 104
 testRunner.And("I\'m on the Funding projection page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 105
 testRunner.Then("I see Pending completion payments with the amount of £ 2,401", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
