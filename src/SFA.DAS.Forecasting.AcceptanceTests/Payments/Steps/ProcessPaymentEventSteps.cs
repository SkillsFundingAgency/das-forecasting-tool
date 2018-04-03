using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using Dapper;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Models.Payments;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using NamedCalendarPeriod = SFA.DAS.Forecasting.Application.Payments.Messages.NamedCalendarPeriod;

namespace SFA.DAS.Forecasting.AcceptanceTests.Payments.Steps
{
    [Binding]
    public class ProcessPaymentEventSteps : StepsBase
    {
        [Scope(Feature = "Process Payment Event")]
        [BeforeFeature(Order = 1)]
        public static void StartLevyFunction()
        {
            StartFunction("SFA.DAS.Forecasting.Payments.Functions");
            StartFunction("SFA.DAS.Forecasting.StubApi.Functions");
        }

        [Given(@"I have no existing payments")]
        [Given(@"I have no existing payments recorded in the forecasting service")]
        public void GivenIHaveNoExistingPayments()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId", Config.EmployerAccountId, DbType.Int64);
            Connection.Execute("Delete from Payment where employerAccountId = @employerAccountId or employerAccountId = '112233'",
                parameters, commandType: CommandType.Text);
        }

        [Given(@"I have no existing commitments")]
        [Given(@"I have no existing commitments recorded in the forecasting service")]
        public void GivenIHaveNoExistingCommitments()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId", Config.EmployerAccountId, DbType.Int64);
            Connection.Execute("Delete from Commitment where employerAccountId = @employerAccountId or employerAccountId = '112233'",
                parameters, commandType: CommandType.Text);
        }

        [Given(@"I have made the following payments")]
        public void GivenIHaveMadeTheFollowingPayments(Table table)
        {
            Payments = table.CreateSet<TestPayment>().ToList();
            Assert.IsTrue(Payments.Any());
        }

        [Given(@"I made some invalid payments")]
        public void GivenIHaveMadeSomeInvalidPayments(Table table)
        {
            Payments = table.CreateSet<TestPayment>().ToList();
        }

        [When(@"the SFA Employer HMRC Payment service notifies the Forecasting service of the payment")]
        public void WhenTheSFAEmployerHMRCPaymentServiceNotifiesTheForecastingServiceOfThePayment()
        {
            for (var idx = 0; idx < Payments.Count; idx++)
            {
                var id = idx + 1;
                var payment = Payments[idx];
                payment.PaymentId = id.ToString();
                payment.ApprenticeshipId = id;
                payment.ProviderId = id;
            }
            Payments.Select((payment, idx) => new PaymentCreatedMessage
            {
                Id = payment.PaymentId,
                EmployerAccountId = Config.EmployerAccountId,
                Amount = payment.PaymentAmount,
                CollectionPeriod = new NamedCalendarPeriod
                {
                    Id = "1718-R01",
                    Month = DateTime.Now.Month,
                    Year = DateTime.Now.Year
                },
                DeliveryPeriod = new Application.Payments.Messages.CalendarPeriod
                {
                    Month = DateTime.Now.Month,
                    Year = DateTime.Now.Year
                },
                EarningDetails = new EarningDetails
                {
                    ActualEndDate = DateTime.MinValue,
                    StartDate = payment.StartDateValue,
                    PlannedEndDate = payment.PlannedEndDate,
                    TotalInstallments = payment.NumberOfInstallments,
                    MonthlyInstallment = payment.InstallmentAmount,
                    CompletionAmount = payment.CompletionAmount,
                    EndpointAssessorId = "654321",
                    CompletionStatus = 1,
                    RequiredPaymentId = Guid.NewGuid()
                },
                ApprenticeshipId = payment.ApprenticeshipId,
                ProviderName = payment.ProviderName,
                Ukprn = payment.ProviderId,
                ApprenticeName = payment.ApprenticeName,
                CourseLevel = payment.CourseLevel,
                CourseName = payment.CourseName,
                Uln = idx,
                CourseStartDate = payment.StartDateValue,
                FundingSource = FundingSource.Levy
            })
            .ToList()
            .ForEach(paymentEvent =>
            {
                var payload = paymentEvent.ToJson();
                var url = Config.PaymentFunctionUrl;
                Console.WriteLine($"Sending payment event to payment function: {url}, Payload: {payload}");
                var response = HttpClient.PostAsync(url, new StringContent(payload, Encoding.UTF8, "application/json")).Result;
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            });
        }

        [Then(@"the Forecasting Payment service should store the payment declarations")]
        public void ThenTheForecastingPaymentServiceShouldStoreThePaymentDeclarations()
        {
            WaitForIt(() =>
            {
                Console.WriteLine($"Looking for Payments. Employer Account Id: {Config.EmployerAccountId}");
                var parameters = new DynamicParameters();
                parameters.Add("@employerAccountId", Config.EmployerAccountId, DbType.Int64);
                var payments = Connection.Query<Payment>("Select * from Payment where employerAccountId = @employerAccountId", parameters, commandType: CommandType.Text).ToList();

                foreach (var payment in Payments)
                {
                    if (payments.Any(p => payment.PaymentId == p.ExternalPaymentId)) continue;
                    Console.WriteLine($"Payment not found. Payment: {payment.ToJson()}");
                    return false;
                }
                return true;
            }, "Failed to find all the payments.");
        }

        [Then(@"the Forecasting Payment service should store the commitment declarations")]
        public void ThenTheForecastingPaymentServiceShouldStoreTheCommitmentDeclarations()
        {
            WaitForIt(() =>
            {
                foreach (var payment in Payments)
                {
                    Console.WriteLine($"Looking for Commitments. Employer Account Id: {Config.EmployerAccountId}, Month: {DateTime.Now.Month}, Year: {DateTime.Now.Year}");
                    var parameters = new DynamicParameters();
                    parameters.Add("@employerAccountId", Config.EmployerAccountId, DbType.Int64);
                    parameters.Add("@providerId", payment.ProviderId, DbType.Int64);
                    parameters.Add("@apprenticeshipId", payment.ApprenticeshipId, DbType.Int64);
                    var count = Connection.ExecuteScalar<int>("Select Count(*) from Commitment where employerAccountId = @employerAccountId and ApprenticeshipId = @apprenticeshipId and ProviderId = @providerId",
                        parameters, commandType: CommandType.Text);
                    return count == 1;
                }
                return false;
            }, "Failed to find all the commitments.");
        }

        [Then(@"the Forecasting Payment service should not store the payments")]
        public void ThenTheForecastingPaymentServiceShouldNotStoreThePaymentDeclarations()
        {
            Thread.Sleep(Config.TimeToWait);
            Console.WriteLine($"Looking for Payments. Employer Account Id: {Config.EmployerAccountId}, Collection Period Year: {DateTime.Now.Year}, Collection Period Month: {DateTime.Now.Month}");
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId", Config.EmployerAccountId, DbType.Int64);
            parameters.Add("@collectionPeriodYear", DateTime.Now.Year, DbType.Int32);
            parameters.Add("@collectionPeriodMonth", DateTime.Now.Month, DbType.Int32);
            var count = Connection.ExecuteScalar<int>("Select Count(*) from Payment where employerAccountId = @employerAccountId and CollectionPeriodYear = @collectionPeriodYear and CollectionPeriodMonth = @collectionPeriodMonth",
                parameters, commandType: CommandType.Text);
            Assert.AreEqual(0, count);
        }

        [Then(@"the Forecasting Payment service should not store commitments")]
        public void ThenTheForecastingPaymentServiceShouldNotStoreTheCommitments()
        {
            Thread.Sleep(Config.TimeToWait);
            Console.WriteLine($"Looking for Commitments. Employer Account Id: {Config.EmployerAccountId}, Collection Period Year: {DateTime.Now.Year}, Collection Period Month: {DateTime.Now.Month}");
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId", Config.EmployerAccountId, DbType.Int64);
            var count = Connection.ExecuteScalar<int>("Select Count(*) from Commitment where employerAccountId = @employerAccountId",
                parameters, commandType: CommandType.Text);
            Assert.AreEqual(0, count);
        }
    }
}
