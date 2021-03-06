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
namespace SFA.DAS.Forecasting.AcceptanceTests.Payments.Feature
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.4.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Build Projections From Payment Run")]
    public partial class BuildProjectionsFromPaymentRunFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Build Projections From Payment Run.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Build Projections From Payment Run", "\tAs an employer with a pay bill over £3 million each year and therefore must now " +
                    "pay the apprenticeship levy\r\n\tI want my levy credit to be forecast for the next " +
                    "4 years\r\n\tSo that I can effectively forecast my account balance", ProgrammingLanguage.CSharp, ((string[])(null)));
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
#line 11
 testRunner.And("I have no existing levy declarations for the payroll period", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 12
 testRunner.And("no account projections have been generated", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Payment run triggers build of projections")]
        public virtual void PaymentRunTriggersBuildOfProjections()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Payment run triggers build of projections", null, ((string[])(null)));
#line 14
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "Scheme",
                        "Amount",
                        "Created Date"});
            table2.AddRow(new string[] {
                        "ABC-1234",
                        "64569.55",
                        "Today"});
#line 15
 testRunner.Given("the following levy declarations have been recorded", ((string)(null)), table2, "Given ");
#line 18
 testRunner.And("the current balance is 5000", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "Payment Amount",
                        "Apprentice Name",
                        "Course Name",
                        "Course Level",
                        "Provider Name",
                        "Start Date",
                        "Installment Amount",
                        "Completion Amount",
                        "Number Of Installments"});
            table3.AddRow(new string[] {
                        "133.33",
                        "Test Apprentice",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "16/04/2017 00:00",
                        "133.33",
                        "400.00",
                        "12"});
            table3.AddRow(new string[] {
                        "133.33",
                        "Test Apprentice 2",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "16/04/2017 00:00",
                        "133.33",
                        "400.00",
                        "12"});
            table3.AddRow(new string[] {
                        "133.33",
                        "Test Apprentice 3",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "16/04/2017 00:00",
                        "133.33",
                        "400.00",
                        "12"});
            table3.AddRow(new string[] {
                        "133.33",
                        "Test Apprentice 4",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "16/04/2017 00:00",
                        "133.33",
                        "400.00",
                        "12"});
            table3.AddRow(new string[] {
                        "133.33",
                        "Test Apprentice 5",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "16/04/2017 00:00",
                        "133.33",
                        "400.00",
                        "12"});
            table3.AddRow(new string[] {
                        "133.33",
                        "Test Apprentice 6",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "16/04/2017 00:00",
                        "133.33",
                        "400.00",
                        "12"});
            table3.AddRow(new string[] {
                        "133.33",
                        "Test Apprentice 7",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "29/05/2017 00:00",
                        "133.33",
                        "400.00",
                        "12"});
            table3.AddRow(new string[] {
                        "133.33",
                        "Test Apprentice 8",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "29/05/2017 00:00",
                        "133.33",
                        "400.00",
                        "12"});
            table3.AddRow(new string[] {
                        "133.33",
                        "Test Apprentice 9",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "29/05/2017 00:00",
                        "133.33",
                        "400.00",
                        "12"});
#line 19
 testRunner.And("I have made the following payments", ((string)(null)), table3, "And ");
#line 30
 testRunner.When("the SFA Employer HMRC Payment service notifies the Forecasting service of the pay" +
                    "ment", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 31
 testRunner.Then("the account projection should be generated", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
