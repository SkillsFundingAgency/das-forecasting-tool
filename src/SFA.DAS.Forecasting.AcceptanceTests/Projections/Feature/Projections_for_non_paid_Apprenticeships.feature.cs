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
    [NUnit.Framework.DescriptionAttribute("Projections For Employers With Non Paid Apprenticeships")]
    public partial class ProjectionsForEmployersWithNonPaidApprenticeshipsFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Projections_for_non_paid_Apprenticeships.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Projections For Employers With Non Paid Apprenticeships", "\tAs an Employer\r\n\tI want my transferred funds in to be considered in my forecast\r" +
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
        [NUnit.Framework.DescriptionAttribute("Sending employer account has transfers in with non paid apprenticeships")]
        public virtual void SendingEmployerAccountHasTransfersInWithNonPaidApprenticeships()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Sending employer account has transfers in with non paid apprenticeships", ((string[])(null)));
#line 16
this.ScenarioSetup(scenarioInfo);
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
                        "FundingSource",
                        "HasHadPayment"});
            table3.AddRow(new string[] {
                        "Test Apprentice 1",
                        "Test Course 1",
                        "1",
                        "Test Provider 1",
                        "Yesterday",
                        "600",
                        "2000",
                        "12",
                        "Levy",
                        "0"});
            table3.AddRow(new string[] {
                        "Test Apprentice 2",
                        "Test Course 2",
                        "1",
                        "Test Provider 2",
                        "Next Month",
                        "400",
                        "2000",
                        "18",
                        "Transfer",
                        "0"});
#line 18
 testRunner.And("the following commitments have been recorded", ((string)(null)), table3, "And ");
#line 22
 testRunner.When("the account projection is triggered after a payment run", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 23
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
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "8000"});
            table4.AddRow(new string[] {
                        "1",
                        "600",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "10400"});
            table4.AddRow(new string[] {
                        "2",
                        "600",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "12400"});
            table4.AddRow(new string[] {
                        "3",
                        "600",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "14400"});
            table4.AddRow(new string[] {
                        "4",
                        "600",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "16400"});
            table4.AddRow(new string[] {
                        "5",
                        "600",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "18400"});
            table4.AddRow(new string[] {
                        "6",
                        "600",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "20400"});
            table4.AddRow(new string[] {
                        "7",
                        "600",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "22400"});
            table4.AddRow(new string[] {
                        "8",
                        "600",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "24400"});
            table4.AddRow(new string[] {
                        "9",
                        "600",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "26400"});
            table4.AddRow(new string[] {
                        "10",
                        "600",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "28400"});
            table4.AddRow(new string[] {
                        "11",
                        "600",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "30400"});
            table4.AddRow(new string[] {
                        "12",
                        "600",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "32400"});
            table4.AddRow(new string[] {
                        "13",
                        "0.00",
                        "400",
                        "0.00",
                        "0.00",
                        "2000.00",
                        "0.00",
                        "33000"});
            table4.AddRow(new string[] {
                        "14",
                        "0.00",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "35600"});
            table4.AddRow(new string[] {
                        "15",
                        "0.00",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "38200"});
            table4.AddRow(new string[] {
                        "16",
                        "0.00",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "40800"});
            table4.AddRow(new string[] {
                        "17",
                        "0.00",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "43400"});
            table4.AddRow(new string[] {
                        "18",
                        "0.00",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "46000"});
            table4.AddRow(new string[] {
                        "19",
                        "0.00",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "48600"});
            table4.AddRow(new string[] {
                        "20",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "2000.00",
                        "49600"});
#line 24
 testRunner.And("should have the following projected values", ((string)(null)), table4, "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Receiving employer account has transfers in with non paid apprenticeships")]
        public virtual void ReceivingEmployerAccountHasTransfersInWithNonPaidApprenticeships()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Receiving employer account has transfers in with non paid apprenticeships", ((string[])(null)));
#line 48
 this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 49
 testRunner.Given("I am a receiving employer", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
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
                        "FundingSource",
                        "HasHadPayment"});
            table5.AddRow(new string[] {
                        "Test Apprentice 1",
                        "Test Course 1",
                        "1",
                        "Test Provider 1",
                        "Yesterday",
                        "3500",
                        "2000",
                        "12",
                        "Levy",
                        "0"});
            table5.AddRow(new string[] {
                        "Test Apprentice 2",
                        "Test Course 2",
                        "1",
                        "Test Provider 2",
                        "Next Month",
                        "400",
                        "2000",
                        "18",
                        "Transfer",
                        "0"});
#line 50
 testRunner.And("the following commitments have been recorded", ((string)(null)), table5, "And ");
#line 54
 testRunner.When("the account projection is triggered after a payment run", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 55
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
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "8000"});
            table6.AddRow(new string[] {
                        "1",
                        "3500",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "7500"});
            table6.AddRow(new string[] {
                        "2",
                        "3500",
                        "400",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "7000"});
            table6.AddRow(new string[] {
                        "3",
                        "3500",
                        "400",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "6500"});
            table6.AddRow(new string[] {
                        "4",
                        "3500",
                        "400",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "6000"});
            table6.AddRow(new string[] {
                        "5",
                        "3500",
                        "400",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "5500"});
            table6.AddRow(new string[] {
                        "6",
                        "3500",
                        "400",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "5000"});
            table6.AddRow(new string[] {
                        "7",
                        "3500",
                        "400",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "4500"});
            table6.AddRow(new string[] {
                        "8",
                        "3500",
                        "400",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "4000"});
            table6.AddRow(new string[] {
                        "9",
                        "3500",
                        "400",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "3500"});
            table6.AddRow(new string[] {
                        "10",
                        "3500",
                        "400",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "3400"});
            table6.AddRow(new string[] {
                        "11",
                        "3500",
                        "400",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "3400"});
            table6.AddRow(new string[] {
                        "12",
                        "3500",
                        "400",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "3400"});
            table6.AddRow(new string[] {
                        "13",
                        "0.00",
                        "400",
                        "400",
                        "0.00",
                        "2000.00",
                        "0.00",
                        "4400"});
            table6.AddRow(new string[] {
                        "14",
                        "0.00",
                        "400",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "7400"});
            table6.AddRow(new string[] {
                        "15",
                        "0.00",
                        "400",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "10400"});
            table6.AddRow(new string[] {
                        "16",
                        "0.00",
                        "400",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "13400"});
            table6.AddRow(new string[] {
                        "17",
                        "0.00",
                        "400",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "16400"});
            table6.AddRow(new string[] {
                        "18",
                        "0.00",
                        "400",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "19400"});
            table6.AddRow(new string[] {
                        "19",
                        "0.00",
                        "400",
                        "400",
                        "0.00",
                        "0.00",
                        "0.00",
                        "22400"});
            table6.AddRow(new string[] {
                        "20",
                        "0.00",
                        "0.00",
                        "0.00",
                        "2000.00",
                        "0.00",
                        "2000.00",
                        "25400"});
#line 56
 testRunner.And("should have the following projected values", ((string)(null)), table6, "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
