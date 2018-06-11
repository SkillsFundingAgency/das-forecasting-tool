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
    [NUnit.Framework.DescriptionAttribute("Pre-Load Payments")]
    public partial class Pre_LoadPaymentsFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "PreLoadPayments.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Pre-Load Payments", "\tAs a product owner \r\n\tI want to pre populate the database for some employers\r\n\tS" +
                    "o that I don\'t have to wait for the process to run.", ProgrammingLanguage.CSharp, ((string[])(null)));
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
 testRunner.Given("my employer account id \"12345\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 8
 testRunner.And("I have no existing payments recorded in the forecasting service", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 9
 testRunner.And("I have no existing payments recorded in the employer accounts service", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 10
 testRunner.And("I have no existing commitments recorded in the forecasting service", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "Id",
                        "Month",
                        "Year"});
            table1.AddRow(new string[] {
                        "1718-R07",
                        "2",
                        "2018"});
#line 11
 testRunner.And("the collection period is", ((string)(null)), table1, "And ");
#line 14
 testRunner.And("the funding source for the payments is \"Levy\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Pre load payments")]
        public virtual void PreLoadPayments()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Pre load payments", ((string[])(null)));
#line 16
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
                        "Number Of Installments",
                        "Delivery Period Month",
                        "Delivery Period Year"});
            table2.AddRow(new string[] {
                        "166.66667",
                        "Test Apprentice 1",
                        "Test Course 1",
                        "1",
                        "Test Provider 1",
                        "01/01/2018",
                        "166.66667",
                        "500.00",
                        "12",
                        "2",
                        "2018"});
            table2.AddRow(new string[] {
                        "83.33333",
                        "Test Apprentice 2",
                        "Test Course 2",
                        "2",
                        "Test Provider 2",
                        "01/01/2018",
                        "83.33333",
                        "250.00",
                        "24",
                        "2",
                        "2018"});
#line 17
 testRunner.Given("payments for the following apprenticeships have been recorded in the Payments ser" +
                    "vice", ((string)(null)), table2, "Given ");
#line 21
 testRunner.And("the payments have also been recorded in the Employer Accounts Service", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 22
 testRunner.When("I trigger the pre-load of the payment events", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 23
 testRunner.Then("the funding projections payments service should record the payments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 24
 testRunner.And("the funding projections commitments service should record the commitments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Pre-load transfer payments")]
        public virtual void Pre_LoadTransferPayments()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Pre-load transfer payments", ((string[])(null)));
#line 26
 this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 27
 testRunner.Given("the funding source for the payments is \"Transfer\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
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
                        "Delivery Period Month",
                        "Delivery Period Year"});
            table3.AddRow(new string[] {
                        "166.66667",
                        "Test Apprentice 1",
                        "Test Course 1",
                        "1",
                        "Test Provider 1",
                        "01/01/2018",
                        "166.66667",
                        "500.00",
                        "12",
                        "2",
                        "2018"});
            table3.AddRow(new string[] {
                        "83.33333",
                        "Test Apprentice 2",
                        "Test Course 2",
                        "2",
                        "Test Provider 2",
                        "01/01/2018",
                        "83.33333",
                        "250.00",
                        "24",
                        "2",
                        "2018"});
#line 28
 testRunner.And("payments for the following apprenticeships have been recorded in the Payments ser" +
                    "vice", ((string)(null)), table3, "And ");
#line 32
 testRunner.And("the payments have also been recorded in the Employer Accounts Service", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 33
 testRunner.When("I trigger the pre-load of the payment events", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 34
 testRunner.Then("the funding projections payments service should record the payments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 35
 testRunner.And("the funding projections commitments service should record the commitments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Pre load anonymised payments")]
        public virtual void PreLoadAnonymisedPayments()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Pre load anonymised payments", ((string[])(null)));
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
                        "Delivery Period Month",
                        "Delivery Period Year"});
            table4.AddRow(new string[] {
                        "166.66667",
                        "Test Apprentice 1",
                        "Test Course 1",
                        "1",
                        "Test Provider 1",
                        "01/01/2018",
                        "166.66667",
                        "500.00",
                        "12",
                        "2",
                        "2018"});
            table4.AddRow(new string[] {
                        "83.33333",
                        "Test Apprentice 2",
                        "Test Course 2",
                        "2",
                        "Test Provider 2",
                        "01/01/2018",
                        "83.33333",
                        "250.00",
                        "24",
                        "2",
                        "2018"});
#line 46
 testRunner.Given("payments for the following apprenticeships have been recorded in the Payments ser" +
                    "vice", ((string)(null)), table4, "Given ");
#line 50
 testRunner.And("the payments have also been recorded in the Employer Accounts Service", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 51
 testRunner.When("I trigger the pre-load of anonymised payment events", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 52
 testRunner.Then("the funding projections payments service should record the anonymised payments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 53
 testRunner.And("the Payment Id should be anonymised", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 54
 testRunner.And("the Apprenticeship Id should be anonymised", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 55
 testRunner.And("the funding projections commitments service should record the anonymised commitme" +
                    "nts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Pre load multiple anonymised payments for a single apprenticeship")]
        public virtual void PreLoadMultipleAnonymisedPaymentsForASingleApprenticeship()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Pre load multiple anonymised payments for a single apprenticeship", ((string[])(null)));
#line 57
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                        "Payment Amount",
                        "Apprentice Name",
                        "Course Name",
                        "Course Level",
                        "Provider Name",
                        "Start Date",
                        "Installment Amount",
                        "Completion Amount",
                        "Number Of Installments",
                        "Delivery Period Month",
                        "Delivery Period Year"});
            table5.AddRow(new string[] {
                        "166.66667",
                        "Test Apprentice 1",
                        "Test Course 1",
                        "1",
                        "Test Provider 1",
                        "01/01/2018",
                        "166.66667",
                        "500.00",
                        "12",
                        "2",
                        "2018"});
            table5.AddRow(new string[] {
                        "83.33333",
                        "Test Apprentice 1",
                        "Test Course 1",
                        "1",
                        "Test Provider 1",
                        "01/01/2018",
                        "83.33333",
                        "250.00",
                        "24",
                        "1",
                        "2018"});
            table5.AddRow(new string[] {
                        "333.333334",
                        "Test Apprentice 1",
                        "Test Course 1",
                        "1",
                        "Test Provider 1",
                        "01/01/2018",
                        "333.333334",
                        "1000.00",
                        "6",
                        "12",
                        "2017"});
#line 58
 testRunner.Given("payments for the following apprenticeship have been recorded in the Payments serv" +
                    "ice", ((string)(null)), table5, "Given ");
#line 63
 testRunner.And("the payments have also been recorded in the Employer Accounts Service", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 64
 testRunner.When("I trigger the pre-load of anonymised payment events", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 65
 testRunner.Then("the funding projections payments service should record the anonymised payments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 66
 testRunner.And("the Payment Id should be anonymised", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 67
 testRunner.And("the Apprenticeship Id should be anonymised", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 68
 testRunner.And("the funding projections commitments service should record the anonymised commitme" +
                    "nt", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
