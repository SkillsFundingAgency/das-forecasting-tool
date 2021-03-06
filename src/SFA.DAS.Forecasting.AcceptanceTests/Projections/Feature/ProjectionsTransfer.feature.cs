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
namespace SFA.DAS.Forecasting.AcceptanceTests.Projections.Feature
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.4.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Projections For Employers With Transfer")]
    public partial class ProjectionsForEmployersWithTransferFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "ProjectionsTransfer.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Projections For Employers With Transfer", "\tAs an Employer\r\n\tI want my transferred funds in to be considered in my forecast\r" +
                    "\n\tSo that the balance of my account can be accurately calculated", ProgrammingLanguage.CSharp, ((string[])(null)));
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
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "Scheme",
                        "Amount",
                        "Created Date"});
            table2.AddRow(new string[] {
                        "ABC-1234",
                        "0",
                        "Today"});
#line 11
 testRunner.And("the following levy declarations have been recorded", ((string)(null)), table2, "And ");
#line 14
 testRunner.And("the current balance is 500", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Sending employer with transfer out after levy run")]
        public virtual void SendingEmployerWithTransferOutAfterLevyRun()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Sending employer with transfer out after levy run", null, ((string[])(null)));
#line 16
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line 17
 testRunner.Given("I am a sending employer", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
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
                        "Test Apprentice 2",
                        "Test Course 2",
                        "1",
                        "Test Provider 2",
                        "last month",
                        "100",
                        "2000",
                        "6",
                        "Transfer"});
#line 18
 testRunner.And("the following commitments have been recorded", ((string)(null)), table3, "And ");
#line 21
 testRunner.When("the account projection is triggered after a levy run", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 22
 testRunner.Then("the account projection should be generated", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "MonthsFromNow",
                        "TotalCostOfTraining",
                        "TransferOutTotalCostOfTraining",
                        "TransferInTotalCostOfTraining",
                        "TransferInCompletionPayments",
                        "CompletionPayments",
                        "TransferOutCompletionPayments",
                        "FutureFunds"});
            table4.AddRow(new string[] {
                        "0",
                        "0.00",
                        "100",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "500"});
            table4.AddRow(new string[] {
                        "1",
                        "0.00",
                        "100",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "400"});
            table4.AddRow(new string[] {
                        "2",
                        "0.00",
                        "100",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "300"});
            table4.AddRow(new string[] {
                        "3",
                        "0.00",
                        "100",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "200"});
            table4.AddRow(new string[] {
                        "4",
                        "0.00",
                        "100",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "100"});
            table4.AddRow(new string[] {
                        "5",
                        "0.00",
                        "100",
                        "0.00",
                        "0.00",
                        "0.00",
                        "00.0",
                        "0"});
            table4.AddRow(new string[] {
                        "6",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "2000.00",
                        "0"});
            table4.AddRow(new string[] {
                        "7",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0"});
#line 23
 testRunner.And("should have the following projected values", ((string)(null)), table4, "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Sending employer with transfer out after payment run")]
        public virtual void SendingEmployerWithTransferOutAfterPaymentRun()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Sending employer with transfer out after payment run", null, ((string[])(null)));
#line 34
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line 35
 testRunner.Given("I am a sending employer", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                        "Apprentice Name",
                        "Course Name",
                        "Course Level",
                        "Provider Name",
                        "Start Date",
                        "Installment Amount",
                        "Completion Amount",
                        "Number Of Installments",
                        "FundingSource"});
            table5.AddRow(new string[] {
                        "Test Apprentice 2",
                        "Test Course 2",
                        "1",
                        "Test Provider 2",
                        "last month",
                        "100.00",
                        "2000",
                        "6",
                        "Transfer"});
#line 36
 testRunner.And("the following commitments have been recorded", ((string)(null)), table5, "And ");
#line 39
 testRunner.When("the account projection is triggered after a payment run", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 40
 testRunner.Then("the account projection should be generated", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                        "MonthsFromNow",
                        "TotalCostOfTraining",
                        "TransferOutTotalCostOfTraining",
                        "TransferInTotalCostOfTraining",
                        "TransferInCompletionPayments",
                        "CompletionPayments",
                        "TransferOutCompletionPayments",
                        "FutureFunds"});
            table6.AddRow(new string[] {
                        "0",
                        "0.00",
                        "100",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "500"});
            table6.AddRow(new string[] {
                        "1",
                        "0.00",
                        "100",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "400"});
            table6.AddRow(new string[] {
                        "2",
                        "0.00",
                        "100",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "300"});
            table6.AddRow(new string[] {
                        "3",
                        "0.00",
                        "100",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "200"});
            table6.AddRow(new string[] {
                        "4",
                        "0.00",
                        "100",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "100"});
            table6.AddRow(new string[] {
                        "5",
                        "0.00",
                        "100",
                        "0.00",
                        "0.00",
                        "0.00",
                        "00.0",
                        "0"});
            table6.AddRow(new string[] {
                        "6",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "2000.00",
                        "0"});
            table6.AddRow(new string[] {
                        "7",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0"});
#line 41
 testRunner.And("should have the following projected values", ((string)(null)), table6, "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Receiving employer account has transfers in after levy run")]
        public virtual void ReceivingEmployerAccountHasTransfersInAfterLevyRun()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Receiving employer account has transfers in after levy run", null, ((string[])(null)));
#line 52
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line 53
 testRunner.Given("I am a receiving employer", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table7 = new TechTalk.SpecFlow.Table(new string[] {
                        "Apprentice Name",
                        "Course Name",
                        "Course Level",
                        "Provider Name",
                        "Start Date",
                        "Installment Amount",
                        "Completion Amount",
                        "Number Of Installments",
                        "FundingSource"});
            table7.AddRow(new string[] {
                        "Test Apprentice 2",
                        "Test Course 2",
                        "1",
                        "Test Provider 2",
                        "last month",
                        "100",
                        "2000",
                        "6",
                        "Transfer"});
#line 54
 testRunner.And("the following commitments have been recorded", ((string)(null)), table7, "And ");
#line 57
 testRunner.When("the account projection is triggered after a levy run", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 58
 testRunner.Then("the account projection should be generated", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            TechTalk.SpecFlow.Table table8 = new TechTalk.SpecFlow.Table(new string[] {
                        "MonthsFromNow",
                        "TotalCostOfTraining",
                        "TransferOutTotalCostOfTraining",
                        "TransferInTotalCostOfTraining",
                        "TransferInCompletionPayments",
                        "CompletionPayments",
                        "TransferOutCompletionPayments",
                        "FutureFunds"});
            table8.AddRow(new string[] {
                        "0",
                        "0.00",
                        "100",
                        "100",
                        "0.00",
                        "0.00",
                        "0.00",
                        "500"});
            table8.AddRow(new string[] {
                        "1",
                        "0.00",
                        "100",
                        "100",
                        "0.00",
                        "0.00",
                        "0.00",
                        "500"});
            table8.AddRow(new string[] {
                        "2",
                        "0.00",
                        "100",
                        "100",
                        "0.00",
                        "0.00",
                        "0.00",
                        "500"});
            table8.AddRow(new string[] {
                        "3",
                        "0.00",
                        "100",
                        "100",
                        "0.00",
                        "0.00",
                        "0.00",
                        "500"});
            table8.AddRow(new string[] {
                        "4",
                        "0.00",
                        "100",
                        "100",
                        "0.00",
                        "0.00",
                        "0.00",
                        "500"});
            table8.AddRow(new string[] {
                        "5",
                        "0.00",
                        "100",
                        "100",
                        "0.00",
                        "0.00",
                        "0.00",
                        "500"});
            table8.AddRow(new string[] {
                        "6",
                        "0.00",
                        "0.00",
                        "0.00",
                        "2000.00",
                        "0.00",
                        "2000.00",
                        "500"});
            table8.AddRow(new string[] {
                        "7",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "500"});
#line 59
 testRunner.And("should have the following projected values", ((string)(null)), table8, "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Receiving employer account has transfers in after payment run")]
        public virtual void ReceivingEmployerAccountHasTransfersInAfterPaymentRun()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Receiving employer account has transfers in after payment run", null, ((string[])(null)));
#line 70
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line 71
 testRunner.Given("I am a receiving employer", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table9 = new TechTalk.SpecFlow.Table(new string[] {
                        "Apprentice Name",
                        "Course Name",
                        "Course Level",
                        "Provider Name",
                        "Start Date",
                        "Installment Amount",
                        "Completion Amount",
                        "Number Of Installments",
                        "FundingSource"});
            table9.AddRow(new string[] {
                        "Test Apprentice 2",
                        "Test Course 2",
                        "1",
                        "Test Provider 2",
                        "last month",
                        "100.00",
                        "2000",
                        "6",
                        "Transfer"});
#line 72
 testRunner.And("the following commitments have been recorded", ((string)(null)), table9, "And ");
#line 75
 testRunner.When("the account projection is triggered after a payment run", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 76
 testRunner.Then("the account projection should be generated", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            TechTalk.SpecFlow.Table table10 = new TechTalk.SpecFlow.Table(new string[] {
                        "MonthsFromNow",
                        "TotalCostOfTraining",
                        "TransferOutTotalCostOfTraining",
                        "TransferInTotalCostOfTraining",
                        "TransferInCompletionPayments",
                        "CompletionPayments",
                        "TransferOutCompletionPayments",
                        "FutureFunds"});
            table10.AddRow(new string[] {
                        "0",
                        "0.00",
                        "100",
                        "100",
                        "0.00",
                        "0.00",
                        "0.00",
                        "500"});
            table10.AddRow(new string[] {
                        "1",
                        "0.00",
                        "100",
                        "100",
                        "0.00",
                        "0.00",
                        "0.00",
                        "500"});
            table10.AddRow(new string[] {
                        "2",
                        "0.00",
                        "100",
                        "100",
                        "0.00",
                        "0.00",
                        "0.00",
                        "500"});
            table10.AddRow(new string[] {
                        "3",
                        "0.00",
                        "100",
                        "100",
                        "0.00",
                        "0.00",
                        "0.00",
                        "500"});
            table10.AddRow(new string[] {
                        "4",
                        "0.00",
                        "100",
                        "100",
                        "0.00",
                        "0.00",
                        "0.00",
                        "500"});
            table10.AddRow(new string[] {
                        "5",
                        "0.00",
                        "100",
                        "100",
                        "0.00",
                        "0.00",
                        "0.00",
                        "500"});
            table10.AddRow(new string[] {
                        "6",
                        "0.00",
                        "0.00",
                        "0.00",
                        "2000.00",
                        "0.00",
                        "2000.00",
                        "500"});
            table10.AddRow(new string[] {
                        "7",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "500"});
#line 77
 testRunner.And("should have the following projected values", ((string)(null)), table10, "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
