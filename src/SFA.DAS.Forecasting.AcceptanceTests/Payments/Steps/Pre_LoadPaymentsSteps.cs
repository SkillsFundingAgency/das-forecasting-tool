// using System;
// using System.Collections.Generic;
// using System.Configuration;
// using System.Data;
// using System.Linq;
// using System.Net.Http;
// using Dapper;
// using Microsoft.Data.SqlClient;
// using Newtonsoft.Json;
// using NUnit.Framework;
// using SFA.DAS.Forecasting.Domain.Extensions;
// using SFA.DAS.Provider.Events.Api.Types;
// using TechTalk.SpecFlow;
// using TechTalk.SpecFlow.Assist;
// using CalendarPeriod = SFA.DAS.Forecasting.Models.Payments.CalendarPeriod;
//
// namespace SFA.DAS.Forecasting.AcceptanceTests.Payments.Steps
// {
//     [Binding]
//     public class Pre_LoadPaymentsSteps : StepsBase
//     {
//         //protected CalendarPeriod DeliveryPeriod
//         //{
//         //    get => Get<CalendarPeriod>("delivery_period");
//         //    set => Set(value, "delivery_period");
//         //}
//
//         protected NamedCalendarPeriod CollectionPeriod
//         {
//             get => Get<NamedCalendarPeriod>("collection_period");
//             set => Set(value, "collection_period");
//         }
//
//         protected  FundingSource FundingSource
//         {
//             get => Get<FundingSource>();
//             set => Set(value);
//         }
//
//         [BeforeFeature(Order = 1)]
//         [Scope(Feature = "Pre-Load Payments")]
//         public static void StartPreLoadLevyEvent()
//         {
//             if (!Config.IsDevEnvironment)
//             {
//                 Assert.Fail("Can only run this feature in the dev environment.");
//             }
//             StartFunction("SFA.DAS.Forecasting.Payments.Functions");
//             StartFunction("SFA.DAS.Forecasting.StubApi.Functions");
//             StartFunction("SFA.DAS.Forecasting.PreLoad.Functions");
//         }
//
//         [Given(@"my employer account id ""(.*)""")]
//         public void GivenMyEmployerAccountId(int accountId)
//         {
//
//         }
//
//         //[Given(@"the delivery period is")]
//         //public void GivenTheDeliveryPeriodIs(Table table)
//         //{
//         //    DeliveryPeriod = table.CreateInstance<CalendarPeriod>();
//         //}
//
//         [Given(@"the collection period is")]
//         public void GivenTheCollectionPeriodIs(Table table)
//         {
//             CollectionPeriod = table.CreateInstance<NamedCalendarPeriod>();
//         }
//
//         [Given(@"the funding source for the payments is ""(.*)""")]
//         public void GivenTheFundingSourceForThePaymentsIs(FundingSource fundingSource)
//         {
//             FundingSource = fundingSource;
//         }
//
//         [Given(@"I have no existing payments recorded in the employer accounts service")]
//         public void GivenIHaveNoExistingPaymentsRecordedInTheEmployerAccountsService()
//         {
//             ExecuteSql(() =>
//             {
//                 using (var connection = new SqlConnection(ConfigurationManager
//                     .ConnectionStrings["EmployerDatabaseConnectionString"]
//                     .ConnectionString))
//                 {
//                     var parameters = new DynamicParameters();
//                     parameters.Add("@accountId", Config.EmployerAccountId);
//                     connection.Execute("delete from [employer_financial].[AccountTransfers] where ReceiverAccountId = @accountId; delete from [employer_financial].[LevyDeclaration] where AccountId = @accountId; delete from [employer_financial].[Payment] where AccountId = @accountId;", parameters, commandType: CommandType.Text);
//                 }
//             });
//         }
//
//         [Given(@"payments for the following apprenticeships have been recorded in the Payments service")]
//         public void GivenPaymentsForTheFollowingApprenticeshipsHaveBeenRecordedInThePaymentsService(Table table)
//         {
//             var apprenticeshipId = 123456;
//             var providerId = 789012;
//             var learnerId = 345678;
//
//             Payments = table.CreateSet<TestPayment>().ToList();
//             for (var i = 0; i < Payments.Count; i++)
//             {
//                 var p = Payments[i];
//                 p.DeliveryPeriod = new CalendarPeriod(int.Parse(table.Rows[i]["Delivery Period Year"]),int.Parse(table.Rows[i]["Delivery Period Month"]));
//             }
//             Payments.ForEach(p =>
//             {
//                 Assert.AreNotEqual(p.DeliveryPeriod.Month, 0, "Delivery period month not set");
//                 Assert.AreNotEqual(p.DeliveryPeriod.Year, 0, "Delivery period year not set");
//                 p.PaymentId = Guid.NewGuid().ToString("D");
//                 p.ApprenticeshipId = apprenticeshipId++;
//                 p.ProviderId = providerId++;
//                 p.LearnerId = learnerId++;
//             });
//             Assert.IsTrue(Payments.Any());
//
//             var page = new PageOfResults<Provider.Events.Api.Types.Payment>
//             {
//                 PageNumber = 1,
//                 TotalNumberOfPages = 1,
//                 Items = Payments.Select(testPayment => new Payment
//                 {
//                     ApprenticeshipId = testPayment.ApprenticeshipId,
//                     CollectionPeriod = new Provider.Events.Api.Types.NamedCalendarPeriod { Id = CollectionPeriod.Id, Month = CollectionPeriod.Month, Year = CollectionPeriod.Year },
//                     Id = testPayment.PaymentId,
//                     Ukprn = testPayment.ProviderId,
//                     Uln = testPayment.LearnerId,
//                     DeliveryPeriod = new Provider.Events.Api.Types.CalendarPeriod { Month = testPayment.DeliveryPeriod.Month, Year = testPayment.DeliveryPeriod.Year },
//                     Amount = testPayment.PaymentAmount,
//                     ApprenticeshipVersion = "123456-001",
//                     ContractType = ContractType.ContractWithEmployer,
//                     EmployerAccountId = Config.EmployerAccountId.ToString(),
//                     EmployerAccountVersion = "11111111",
//                     EvidenceSubmittedOn = DateTime.Today,
//                     ProgrammeType = 25,
//                     StandardCode = 123,
//                     PathwayCode = 1,
//                     FrameworkCode = 1,
//                     TransactionType = TransactionType.Learning,
//                     FundingSource = FundingSource,
//                     EarningDetails = new List<Earning>
//                     {
//                         new Earning
//                         {
//                             RequiredPaymentId = Guid.NewGuid(),
//                             ActualEndDate = new DateTime(0001,1,1),
//                             CompletionAmount = testPayment.CompletionAmount,
//                             CompletionStatus = 1,
//                             MonthlyInstallment = testPayment.InstallmentAmount,
//                             PlannedEndDate = testPayment.PlannedEndDate,
//                             StartDate = testPayment.StartDateValue,
//                             TotalInstallments = testPayment.NumberOfInstallments
//                         }
//                     }
//                 }).ToArray()
//             };
//             var json = JsonConvert.SerializeObject(page);
//             Console.WriteLine($"Posting page of payments to StubApi. Url: {Config.ApiInsertPaymentUrl}, Payload: {json}");
//             HttpClient.PostAsync(Config.ApiInsertPaymentUrl, new StringContent(json)).Wait();
//         }
//
//         [Given(@"payments for the following apprenticeship have been recorded in the Payments service")]
//         public void GivenPaymentsForTheFollowingApprenticeshipPaymentsHaveBeenRecordedInThePaymentsService(Table table)
//         {
//             var apprenticeshipId = 123456;
//             var providerId = 789012;
//             var learnerId = 345678;
//
//             Payments = table.CreateSet<TestPayment>().ToList();
//             Assert.IsTrue(Payments.Any());
//             for (var i = 0; i < Payments.Count; i++)
//             {
//                 var p = Payments[i];
//                 p.DeliveryPeriod = new CalendarPeriod(int.Parse(table.Rows[i]["Delivery Period Year"]),int.Parse(table.Rows[i]["Delivery Period Month"]));
//                 Assert.AreNotEqual(p.DeliveryPeriod.Month, 0, "Delivery period month not set");
//                 Assert.AreNotEqual(p.DeliveryPeriod.Year, 0, "Delivery period year not set");
//                 p.PaymentId = Guid.NewGuid().ToString("D");
//                 p.ApprenticeshipId = apprenticeshipId;
//                 p.ProviderId = providerId;
//                 p.LearnerId = learnerId;
//             }
//
//             var page = new PageOfResults<Provider.Events.Api.Types.Payment>
//             {
//                 PageNumber = 1,
//                 TotalNumberOfPages = 1,
//                 Items = Payments.Select(testPayment => new Payment
//                 {
//                     ApprenticeshipId = testPayment.ApprenticeshipId,
//                     CollectionPeriod = new Provider.Events.Api.Types.NamedCalendarPeriod { Id = CollectionPeriod.Id, Month = CollectionPeriod.Month, Year = CollectionPeriod.Year },
//                     Id = testPayment.PaymentId,
//                     Ukprn = testPayment.ProviderId,
//                     Uln = testPayment.LearnerId,
//                     DeliveryPeriod = new Provider.Events.Api.Types.CalendarPeriod { Month = testPayment.DeliveryPeriod.Month, Year = testPayment.DeliveryPeriod.Year },
//                     Amount = testPayment.PaymentAmount,
//                     ApprenticeshipVersion = "123456-001",
//                     ContractType = ContractType.ContractWithEmployer,
//                     EmployerAccountId = Config.EmployerAccountId.ToString(),
//                     EmployerAccountVersion = "11111111",
//                     EvidenceSubmittedOn = DateTime.Today,
//                     ProgrammeType = 25,
//                     StandardCode = 123,
//                     PathwayCode = 1,
//                     FrameworkCode = 1,
//                     TransactionType = TransactionType.Learning,
//                     FundingSource = FundingSource,
//                     EarningDetails = new List<Earning>
//                     {
//                         new Earning
//                         {
//                             RequiredPaymentId = Guid.NewGuid(),
//                             ActualEndDate = new DateTime(0001,1,1),
//                             CompletionAmount = testPayment.CompletionAmount,
//                             CompletionStatus = 1,
//                             MonthlyInstallment = testPayment.InstallmentAmount,
//                             PlannedEndDate = testPayment.PlannedEndDate,
//                             StartDate = testPayment.StartDateValue,
//                             TotalInstallments = testPayment.NumberOfInstallments
//                         }
//                     }
//                 }).ToArray()
//             };
//             var json = JsonConvert.SerializeObject(page);
//             Console.WriteLine($"Posting page of payments to StubApi. Url: {Config.ApiInsertPaymentUrl}, Payload: {json}");
//             HttpClient.PostAsync(Config.ApiInsertPaymentUrl, new StringContent(json)).Wait();
//         }
//
//
//         protected DataTable ToTransferDataTable(IList<TestPayment> payments)
//         {
//             var table = new DataTable();
//
//             table.Columns.AddRange(new[]
//             {
//                 new DataColumn("SenderAccountId", typeof(long)),
//                 new DataColumn("SenderAccountName", typeof(string)),
//                 new DataColumn("ReceiverAccountId", typeof(long)),
//                 new DataColumn("ReceiverAccountName", typeof(string)),
//                 new DataColumn("ApprenticeshipId", typeof(long)),
//                 new DataColumn("CourseName", typeof(string)),
//                 new DataColumn("CourseLevel", typeof(int)),
//                 new DataColumn("Amount", typeof(decimal)),
//                 new DataColumn("PeriodEnd", typeof(string)),
//                 new DataColumn("Type", typeof(string)),
//                 new DataColumn("RequiredPaymentId", typeof(Guid)),
//             });
//
//             foreach (var payment in payments)
//             {
//                 table.Rows.Add(
//                     "54321",
//                     "Test Sender Account",
//                     Config.EmployerAccountId,
//                     "Test Receiver Account",
//                     payment.ApprenticeshipId,
//                     payment.CourseName,
//                     payment.CourseLevel,
//                     payment.InstallmentAmount,
//                     CollectionPeriod.Id,
//                     "0",
//                     Guid.NewGuid());
//             }
//
//             table.AcceptChanges();
//
//             return table;
//         }
//
//
//         public DataTable ToPaymentsDataTable(IList<TestPayment> payments)
//         {
//             var paymentsDataTable = new DataTable();
//
//             paymentsDataTable.Columns.Add("PaymentId", typeof(Guid));
//             paymentsDataTable.Columns.Add("Ukprn", typeof(long));
//             paymentsDataTable.Columns.Add("ProviderName", typeof(string));
//             paymentsDataTable.Columns.Add("Uln", typeof(long));
//             paymentsDataTable.Columns.Add("AccountId", typeof(long));
//             paymentsDataTable.Columns.Add("ApprenticeshipId", typeof(long));
//             paymentsDataTable.Columns.Add("DeliveryPeriodMonth", typeof(int));
//             paymentsDataTable.Columns.Add("DeliveryPeriodYear", typeof(int));
//             paymentsDataTable.Columns.Add("CollectionPeriodId", typeof(string));
//             paymentsDataTable.Columns.Add("CollectionPeriodMonth", typeof(int));
//             paymentsDataTable.Columns.Add("CollectionPeriodYear", typeof(int));
//             paymentsDataTable.Columns.Add("EvidenceSubmittedOn", typeof(DateTime));
//             paymentsDataTable.Columns.Add("EmployerAccountVersion", typeof(string));
//             paymentsDataTable.Columns.Add("ApprenticeshipVersion", typeof(string));
//             paymentsDataTable.Columns.Add("FundingSource", typeof(string));
//             paymentsDataTable.Columns.Add("TransactionType", typeof(string));
//             paymentsDataTable.Columns.Add("Amount", typeof(decimal));
//             paymentsDataTable.Columns.Add("PeriodEnd", typeof(string));
//             paymentsDataTable.Columns.Add("StandardCode", typeof(long));
//             paymentsDataTable.Columns.Add("FrameworkCode", typeof(int));
//             paymentsDataTable.Columns.Add("ProgrammeType", typeof(int));
//             paymentsDataTable.Columns.Add("PathwayCode", typeof(int));
//             paymentsDataTable.Columns.Add("PathwayName", typeof(string));
//             paymentsDataTable.Columns.Add("ApprenticeshipCourseName", typeof(string));
//             paymentsDataTable.Columns.Add("ApprenticeName", typeof(string));
//             paymentsDataTable.Columns.Add("ApprenticeNINumber", typeof(string));
//             paymentsDataTable.Columns.Add("ApprenticeshipCourseLevel", typeof(int));
//             paymentsDataTable.Columns.Add("ApprenticeshipCourseStartDate", typeof(DateTime));
//             paymentsDataTable.Columns.Add("IsHistoricProviderName", typeof(byte));
//             foreach (var payment in payments)
//             {
//                 paymentsDataTable.Rows.Add(
//                     payment.PaymentId,
//                     payment.ProviderId,
//                     payment.ProviderName,
//                     payment.LearnerId,
//                     Config.EmployerAccountId,
//                     payment.ApprenticeshipId,
//                     payment.DeliveryPeriod.Month,
//                     payment.DeliveryPeriod.Year,
//                     CollectionPeriod.Id,
//                     CollectionPeriod.Month,
//                     CollectionPeriod.Year,
//                     DateTime.Today,
//                     string.Empty,
//                     string.Empty,
//                     FundingSource.ToString("D"),
//                     string.Empty,
//                     payment.PaymentAmount,
//                     CollectionPeriod.Id,
//                     123,
//                     1,
//                     25,
//                     1,
//                     string.Empty,
//                     payment.CourseName,
//                     payment.ApprenticeName,
//                     string.Empty,
//                     payment.CourseLevel,
//                     payment.StartDateValue,
//                     false);
//             }
//
//             return paymentsDataTable;
//         }
//
//
//         [Given(@"the payments have also been recorded in the Employer Accounts Service")]
//         public void GivenThePaymentsHaveAlsoBeenRecordedInTheEmployerAccountsService()
//         {
//             ExecuteSql(() =>
//             {
//                 using (var connection = new SqlConnection(ConfigurationManager
//                     .ConnectionStrings["EmployerDatabaseConnectionString"]
//                     .ConnectionString))
//                 {
//                     var parameters = new DynamicParameters();
//                     parameters.Add("@payments", ToPaymentsDataTable(Payments).AsTableValuedParameter("[employer_financial].[PaymentsTable]"));
//                     connection.Execute("[employer_financial].[CreatePayments]", parameters, commandType: CommandType.StoredProcedure);
//                     if (FundingSource == FundingSource.LevyTransfer)
//                     {
//                         parameters = new DynamicParameters();
//                         parameters.Add("@transfers", ToTransferDataTable(Payments).AsTableValuedParameter("[employer_financial].[AccountTransferTable]"));
//                         connection.Execute("[employer_financial].[CreateAccountTransfers]", parameters, commandType: CommandType.StoredProcedure);
//                     }
//                 }
//             });
//         }
//
//         [When(@"I trigger the pre-load of the payment events")]
//         public void WhenITriggerThePre_LoadOfThePaymentEvents()
//         {
//             var request = new
//             {
//                 EmployerAccountIds = new[] { Config.HashedEmployerAccountId },
//                 PeriodYear = CollectionPeriod.Year.ToString(),
//                 PeriodMonth = CollectionPeriod.Month.ToString(),
//                 PeriodId = CollectionPeriod.Id
//             };
//             var json = JsonConvert.SerializeObject(request);
//             Send(Config.PaymentPreLoadHttpFunction, json);
//         }
//
//         [When(@"I trigger the pre-load of the payment events for all employers")]
//         public void WhenITriggerThePre_LoadOfThePaymentEventsForAllEmployers()
//         {
//             var request = new
//             {
//                 PeriodYear = CollectionPeriod.Year.ToString(),
//                 PeriodMonth = CollectionPeriod.Month.ToString(),
//                 PeriodId = CollectionPeriod.Id
//             };
//             var json = JsonConvert.SerializeObject(request);
//             Send(Config.AllEmployersPaymentPreLoadHttpFunction, json);
//         }
//
//         [When(@"I trigger the pre-load of anonymised payment events")]
//         public void WhenITriggerThePre_LoadOfAnonymisedPaymentEvents()
//         {
//             var request = new
//             {
//                 EmployerAccountIds = new[] { Config.HashedEmployerAccountId },
//                 PeriodYear = CollectionPeriod.Year.ToString(),
//                 PeriodMonth = CollectionPeriod.Month.ToString(),
//                 PeriodId = CollectionPeriod.Id,
//                 SubstitutionId = "112233"
//             };
//             var json = JsonConvert.SerializeObject(request);
//             Send(Config.PaymentPreLoadHttpFunction, json);
//         }
//
//         [Then(@"the Payment Id should be anonymised")]
//         public void ThenThePaymentIdShouldBeAnonymised()
//         {
//             Payments.ForEach(payment =>
//             {
//                 Assert.IsFalse(RecordedPayments.Any(recordedPayment =>
//                     Math.Round(payment.PaymentAmount, 2) == Math.Round(recordedPayment.Amount, 2) &&
//                     payment.PaymentId == recordedPayment.ExternalPaymentId),$"Payment Id for payment {payment.ToJson()} was not anonymised.");
//             });
//         }
//
//         [Then(@"the Apprenticeship Id should be anonymised")]
//         public void ThenThePaymentApprenticeshipIdShouldBeAnonymised()
//         {
//             Payments.ForEach(payment =>
//             {
//                 Assert.IsFalse(RecordedPayments.Any(recordedPayment =>
//                     Math.Round(payment.PaymentAmount, 2) == Math.Round(recordedPayment.Amount, 2) &&
//                     payment.ApprenticeshipId == recordedPayment.ApprenticeshipId), 
//                     $"Apprenticeship Id for payment {payment.ToJson()} was not anonymised.");
//             });
//         }
//
//         [Then(@"the funding projections payments service should record the payments")]
//         public void ThenTheFundingProjectionsPaymentsServiceShouldRecordThePayments()
//         {
//             WaitForIt(() =>
//             {
//                 var payments = DataContext.Payments
//                     .Where(payment => payment.EmployerAccountId == Config.EmployerAccountId)
//                     .ToList();
//                 Console.WriteLine($"Got {payments.Count} payments from projections payments table.");
//                 if (payments.Count != Payments.Count)
//                     return false;
//                 RecordedPayments = payments;
//                 return true;
//             }, "Failed to find the payments");
//         }
//
//         [Then(@"the funding projections payments service should record the anonymised payments")]
//         public void ThenTheFundingProjectionsPaymentsServiceShouldRecordTheAnonymisedPayments()
//         {
//             WaitForIt(() =>
//             {
//                 var payments =  DataContext.Payments
//                     .Where(payment => payment.EmployerAccountId == 112233)
//                     .ToList();
//                 Console.WriteLine($"Got {payments.Count} payments from projections payments table.");
//                 if (payments.Count != Payments.Count)
//                     return false;
//                 RecordedPayments = payments;
//                 return true;
//             }, "Failed to find the payments");
//         }
//
//         [Then(@"the funding projections commitments service should record the commitments")]
//         public void ThenTheFundingProjectionsCommitmentsServiceShouldRecordTheCommitments()
//         {
//             WaitForIt(() =>
//             {
//                 var commitments = DataContext.Commitments
//                     .Where(c => c.EmployerAccountId == Config.EmployerAccountId)
//                     .ToList();
//                 Console.WriteLine($"Got {commitments.Count} commitments from projections commitments table.");
//                 if (commitments.Count != Payments.Count)
//                     return false;
//                 RecordedCommitments = commitments;
//                 return true;
//             }, "Failed to find the commitments");
//         }
//
//
//         [Then(@"the funding projections commitments service should record the anonymised commitment")]
//         [Then(@"the funding projections commitments service should record the anonymised commitments")]
//         public void ThenTheFundingProjectionsCommitmentsServiceShouldRecordTheAnonymisedCommitments()
//         {
//             WaitForIt(() =>
//             {
//                 var commitments = DataContext.Commitments
//                     .Where(c => c.EmployerAccountId == 112233)
//                     .ToList();
//                 Console.WriteLine($"Got {commitments.Count} commitments from projections commitments table.");
//                 if (commitments.Count != Payments.Count)
//                     return false;
//                 RecordedCommitments = commitments;
//                 return true;
//             }, "Failed to find the commitments");
//         }
//     }
// }
