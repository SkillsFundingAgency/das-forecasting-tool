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
namespace SFA.DAS.Forecasting.AcceptanceTests.Payments.Feature
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.3.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Process Payment Event")]
    public partial class ProcessPaymentEventFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Process Payment Event.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Process Payment Event", "\tAs an employer\r\n\tI want my payments to be forecast for the next 4 years\r\n\tSo tha" +
                    "t I can effectively forecast my account balance", ProgrammingLanguage.CSharp, ((string[])(null)));
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
 testRunner.Given("I have no existing payments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 8
 testRunner.And("I have no existing commitments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("AC1: Store payment event data")]
        public virtual void AC1StorePaymentEventData()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("AC1: Store payment event data", ((string[])(null)));
#line 10
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "Payment Amount",
                        "Apprentice Name",
                        "Course Name",
                        "Course Level",
                        "Provider Name",
                        "Start Date",
                        "Installment Amount",
                        "Completion Amount",
                        "Number Of Installments"});
            table1.AddRow(new string[] {
                        "133.33",
                        "Test Apprentice",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "16/04/2017 00:00",
                        "133.33",
                        "400.00",
                        "12"});
            table1.AddRow(new string[] {
                        "133.33",
                        "Test Apprentice 2",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "16/04/2017 00:00",
                        "133.33",
                        "400.00",
                        "12"});
#line 11
 testRunner.Given("I have made the following payments", ((string)(null)), table1, "Given ");
#line 15
 testRunner.When("the SFA Employer HMRC Payment service notifies the Forecasting service of the pay" +
                    "ment", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 16
 testRunner.Then("the Forecasting Payment service should store the payment declarations", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 17
 testRunner.And("the Forecasting Payment service should store the commitment declarations", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("AC2: Do not store invalid data")]
        public virtual void AC2DoNotStoreInvalidData()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("AC2: Do not store invalid data", ((string[])(null)));
#line 19
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "Payment Amount",
                        "Apprentice Name",
                        "Course Name",
                        "Course Level",
                        "Provider Name",
                        "Start Date",
                        "Installment Amount",
                        "Completion Amount",
                        "Number Of Installments"});
            table2.AddRow(new string[] {
                        "0",
                        "Test Apprentice",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "16/04/2017 00:00",
                        "133.33",
                        "400.00",
                        "12"});
            table2.AddRow(new string[] {
                        "133.33",
                        "",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "16/04/2017 00:00",
                        "133.33",
                        "400.00",
                        "12"});
            table2.AddRow(new string[] {
                        "133.33",
                        "Test Apprentice 3",
                        "",
                        "1",
                        "Test Provider",
                        "16/04/2017 00:00",
                        "133.33",
                        "400.00",
                        "12"});
            table2.AddRow(new string[] {
                        "133.33",
                        "Test Apprentice 4",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "01/01/0001 00:00",
                        "133.33",
                        "400.00",
                        "12"});
            table2.AddRow(new string[] {
                        "133.33",
                        "Test Apprentice 5",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "16/04/2017 00:00",
                        "0",
                        "400.00",
                        "12"});
            table2.AddRow(new string[] {
                        "133.33",
                        "Test Apprentice 6",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "16/04/2017 00:00",
                        "133.33",
                        "0",
                        "12"});
            table2.AddRow(new string[] {
                        "133.33",
                        "Test Apprentice 7",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "29/05/2017 00:00",
                        "133.33",
                        "400.00",
                        "0"});
#line 20
 testRunner.Given("I made some invalid payments", ((string)(null)), table2, "Given ");
#line 29
 testRunner.When("the SFA Employer HMRC Payment service notifies the Forecasting service of the pay" +
                    "ment", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 30
 testRunner.Then("the Forecasting Payment service should not store the payments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 31
 testRunner.And("the Forecasting Payment service should not store commitments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Ensure sending employer transfer payments are processed")]
        public virtual void EnsureSendingEmployerTransferPaymentsAreProcessed()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Ensure sending employer transfer payments are processed", ((string[])(null)));
#line 33
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
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
                        "Number Of Installments",
                        "Sending Employer Account Id"});
            table3.AddRow(new string[] {
                        "133.33",
                        "Test Apprentice",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "16/04/2017 00:00",
                        "133.33",
                        "400.00",
                        "12",
                        "100021"});
            table3.AddRow(new string[] {
                        "133.33",
                        "Test Apprentice 2",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "16/04/2017 00:00",
                        "133.33",
                        "400.00",
                        "12",
                        "100022"});
            table3.AddRow(new string[] {
                        "133.33",
                        "Test Apprentice 3",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "16/04/2017 00:00",
                        "133.33",
                        "400.00",
                        "12",
                        "100021"});
            table3.AddRow(new string[] {
                        "133.33",
                        "Test Apprentice 4",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "16/04/2017 00:00",
                        "133.33",
                        "400.00",
                        "12",
                        ""});
            table3.AddRow(new string[] {
                        "133.33",
                        "Test Apprentice 5",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "16/04/2017 00:00",
                        "133.33",
                        "400.00",
                        "12",
                        "12345"});
#line 34
 testRunner.Given("I have made the following payments", ((string)(null)), table3, "Given ");
#line 41
 testRunner.When("the SFA Employer HMRC Payment service notifies the Forecasting service of the pay" +
                    "ment", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 42
 testRunner.Then("the Forecasting Payment service should store the payment declarations", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 43
 testRunner.And("the Forecasting Payment service should store the commitment declarations", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Ensure receiving employer transfer payments are processed (CI-762)")]
        public virtual void EnsureReceivingEmployerTransferPaymentsAreProcessedCI_762()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Ensure receiving employer transfer payments are processed (CI-762)", ((string[])(null)));
#line 45
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "Payment Amount",
                        "Apprentice Name",
                        "Course Name",
                        "Course Level",
                        "Provider Name",
                        "Start Date",
                        "Installment Amount",
                        "Completion Amount",
                        "Number Of Installments",
                        "Sending Employer Account Id",
                        "FundingSource"});
            table4.AddRow(new string[] {
                        "133.33",
                        "Test Apprentice",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "16/04/2015 00:00",
                        "133.33",
                        "400.00",
                        "12",
                        "1",
                        "Transfer"});
            table4.AddRow(new string[] {
                        "133.33",
                        "Test Apprentice 2",
                        "Test Course",
                        "1",
                        "Test Provider",
                        "16/04/2015 00:00",
                        "133.33",
                        "400.00",
                        "12",
                        "1",
                        "Transfer"});
#line 46
 testRunner.And("I have made the following payments", ((string)(null)), table4, "And ");
#line 51
 testRunner.When("the SFA Employer HMRC Payment service notifies the Forecasting service of the pay" +
                    "ment", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 52
 testRunner.Then("the Forecasting Payment service should store the payment declarations receiving e" +
                    "mployer 12345 from sending employer 1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 53
 testRunner.And("the Forecasting Payment service should store the commitment declarations for rece" +
                    "iving employer 12345 from sending employer 1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
