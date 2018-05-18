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
    [NUnit.Framework.DescriptionAttribute("Calculate actual transfer completion payments (CI-620)")]
    public partial class CalculateActualTransferCompletionPaymentsCI_620Feature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "CalculateActualTransferCompletionPayments.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Calculate actual transfer completion payments (CI-620)", "\tAs an employer\r\n\tI want to see my actual training completion costs for transfers" +
                    " that I\'m funding\r\n\tSo that they can be taken into account while forecasting aga" +
                    "inst my transfer allowance", ProgrammingLanguage.CSharp, ((string[])(null)));
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
                        "8000",
                        "Today"});
#line 11
 testRunner.And("the following levy declarations have been recorded", ((string)(null)), table2, "And ");
#line 14
 testRunner.And("the current balance is 5000", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("AC1: Multiple transfer commitments")]
        public virtual void AC1MultipleTransferCommitments()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("AC1: Multiple transfer commitments", ((string[])(null)));
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
                        "EmployerAccountId",
                        "SendingEmployerAccountId",
                        "FundingSource"});
            table3.AddRow(new string[] {
                        "Test Apprentice",
                        "Test Course",
                        "1",
                        "Test Provider 1",
                        "Yesterday",
                        "2000",
                        "1200",
                        "6",
                        "999",
                        "12345",
                        "Transfer"});
            table3.AddRow(new string[] {
                        "Test Apprentice 1",
                        "Test Course",
                        "1",
                        "Test Provider 2",
                        "Yesterday",
                        "2000",
                        "1200",
                        "6",
                        "999",
                        "12345",
                        "Transfer"});
            table3.AddRow(new string[] {
                        "Test Apprentice 2",
                        "Test Course 2",
                        "1",
                        "Test Provider 3",
                        "Yesterday",
                        "2000",
                        "1200",
                        "6",
                        "999",
                        "12345",
                        "Transfer"});
            table3.AddRow(new string[] {
                        "Test Apprentice 3",
                        "Test Course",
                        "1",
                        "Test Provider 4",
                        "Yesterday",
                        "2000",
                        "1200",
                        "6",
                        "999",
                        "12345",
                        "Transfer"});
            table3.AddRow(new string[] {
                        "Test Apprentice 4",
                        "Test Course 2",
                        "1",
                        "Test Provider 5",
                        "Next Year",
                        "2000",
                        "1200",
                        "6",
                        "999",
                        "12345",
                        "Transfer"});
            table3.AddRow(new string[] {
                        "Test Apprentice 5",
                        "Test Course 5",
                        "1",
                        "Test Provider 6",
                        "Next Year",
                        "3000",
                        "1600",
                        "7",
                        "12345",
                        "999",
                        "Transfer"});
            table3.AddRow(new string[] {
                        "Test Apprentice 7",
                        "Test Course 6",
                        "1",
                        "Test Provider 7",
                        "Yesterday",
                        "3000",
                        "1600",
                        "7",
                        "12345",
                        "12345",
                        "Levy"});
#line 17
 testRunner.Given("the following commitments have been recorded", ((string)(null)), table3, "Given ");
#line 27
 testRunner.When("the account projection is triggered after a payment run", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 28
 testRunner.Then("there should be 7 commitments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 29
 testRunner.And("the account projection should be generated", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "MonthsFromNow",
                        "EmployerAccountId",
                        "ProjectionCreationDate",
                        "ProjectionGenerationType",
                        "FundsIn",
                        "TotalCostOfTraining",
                        "TransferOutTotalCostOfTraining",
                        "TransferInTotalCostOfTraining",
                        "TransferInCompletionPayments",
                        "CompletionPayments",
                        "TransferOutCompletionPayments",
                        "FutureFunds",
                        "CoInvestmentEmployer",
                        "CoInvestmentGovernment"});
            table4.AddRow(new string[] {
                        "0",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "8000.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "1",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "3000.00",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "3000.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "2",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "3000.00",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "2000.00",
                        "200.00",
                        "1800.00"});
            table4.AddRow(new string[] {
                        "3",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "3000.00",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "500.00",
                        "4500.00"});
            table4.AddRow(new string[] {
                        "4",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "3000.00",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "500.00",
                        "4500.00"});
            table4.AddRow(new string[] {
                        "5",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "3000.00",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "500.00",
                        "4500.00"});
            table4.AddRow(new string[] {
                        "6",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "3000.00",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "500.00",
                        "4500.00"});
            table4.AddRow(new string[] {
                        "7",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "3000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "4800.00",
                        "200.00",
                        "180.00",
                        "1620.00"});
            table4.AddRow(new string[] {
                        "8",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "1600.00",
                        "0.00",
                        "6600.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "9",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "14600.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "10",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "9000.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "11",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "12000.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "12",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "15000.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "13",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "5000.00",
                        "3000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "16000.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "14",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "5000.00",
                        "3000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "17000.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "15",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "5000.00",
                        "3000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "18000.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "16",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "5000.00",
                        "3000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "19000.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "17",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "5000.00",
                        "3000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "20000.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "18",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "5000.00",
                        "3000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "21000.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "19",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "3000.00",
                        "3000.00",
                        "0.00",
                        "0.00",
                        "1200.00",
                        "22800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "20",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "1600.00",
                        "0.00",
                        "1600.00",
                        "25800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "21",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "28800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "22",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "31800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "23",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "34800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "24",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "37800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "25",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "40800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "26",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "43800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "27",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "46800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "28",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "49800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "29",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "52800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "30",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "55800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "31",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "58800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "32",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "61800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "33",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "64800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "34",
                        "12345",
                        "2018-05-16 08:08:14.447",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "67800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "35",
                        "12345",
                        "2018-05-16 08:08:14.450",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "70800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "36",
                        "12345",
                        "2018-05-16 08:08:14.450",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "73800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "37",
                        "12345",
                        "2018-05-16 08:08:14.450",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "76800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "38",
                        "12345",
                        "2018-05-16 08:08:14.450",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "79800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "39",
                        "12345",
                        "2018-05-16 08:08:14.450",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "82800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "40",
                        "12345",
                        "2018-05-16 08:08:14.450",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "85800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "41",
                        "12345",
                        "2018-05-16 08:08:14.450",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "88800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "42",
                        "12345",
                        "2018-05-16 08:08:14.450",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "91800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "43",
                        "12345",
                        "2018-05-16 08:08:14.450",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "94800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "44",
                        "12345",
                        "2018-05-16 08:08:14.450",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "97800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "45",
                        "12345",
                        "2018-05-16 08:08:14.450",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "100800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "46",
                        "12345",
                        "2018-05-16 08:08:14.450",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "103800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "47",
                        "12345",
                        "2018-05-16 08:08:14.450",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "106800.00",
                        "0.00",
                        "0.00"});
            table4.AddRow(new string[] {
                        "48",
                        "12345",
                        "2018-05-16 08:08:14.450",
                        "1",
                        "8000.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00",
                        "109800.00",
                        "0.00",
                        "0.00"});
#line 30
 testRunner.And("should have following projections from completion", ((string)(null)), table4, "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("AC3: Multiple transfer commitments and some with end dates after end of forecast " +
            "period")]
        public virtual void AC3MultipleTransferCommitmentsAndSomeWithEndDatesAfterEndOfForecastPeriod()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("AC3: Multiple transfer commitments and some with end dates after end of forecast " +
                    "period", ((string[])(null)));
#line 82
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
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
                        "EmployerAccountId",
                        "SendingEmployerAccountId",
                        "FundingSource"});
            table5.AddRow(new string[] {
                        "Test Apprentice",
                        "Test Course",
                        "1",
                        "Test Provider 1",
                        "Today",
                        "2000",
                        "1200",
                        "48",
                        "999",
                        "12345",
                        "Transfer"});
            table5.AddRow(new string[] {
                        "Test Apprentice 4",
                        "Test Course 2",
                        "1",
                        "Test Provider 5",
                        "Today",
                        "2000",
                        "1200",
                        "48",
                        "999",
                        "12345",
                        "Transfer"});
#line 83
 testRunner.Given("the following commitments have been recorded", ((string)(null)), table5, "Given ");
#line 88
 testRunner.When("the account projection is triggered after a payment run", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 89
 testRunner.Then("the account projection should be generated", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 90
 testRunner.And("should have no payments with TransferOutCompletionPayments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
