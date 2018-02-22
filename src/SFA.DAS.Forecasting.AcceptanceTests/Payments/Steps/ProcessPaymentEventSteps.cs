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
using CollectionPeriod = SFA.DAS.Forecasting.Application.Payments.Messages.CollectionPeriod;

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
        public void GivenIHaveNoExistingPayments()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId", Config.EmployerAccountId, DbType.Int64);
            Connection.Execute("Delete from Payment where employerAccountId = @employerAccountId",
                parameters, commandType: CommandType.Text);
        }

        [Given(@"I have made the following payments")]
        public void GivenIHaveMadeTheFollowingPayments(Table table)
        {
            Payments = table.CreateSet<Payment>().ToList();
            Assert.IsTrue(Payments.Any());
        }

        [Given(@"I made some invalid payments")]
        public void GivenIHaveMadeSomeInvalidPayments()
        {
            Payments = new List<Payment>
            {
                new Payment
                {
                    ApprenticeshipId = -1,
                    ProviderId = 3
                },
                new Payment
                {
                    ApprenticeshipId = 3,
                    ProviderId = -2
                },
            };
        }

        [Given(@"payment events have been created")]
        public void GivenPaymentEventsHaveBeenCreated()
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"events with invalid data have been created")]
        public void GivenEventsWithInvalidDataHaveBeenCreated()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"the SFA Employer HMRC Payment service notifies the Forecasting service of the payment")]
        public void WhenTheSFAEmployerHMRCPaymentServiceNotifiesTheForecastingServiceOfThePayment()
        {
            Payments.Select(payment => new PaymentCreatedMessage
            {
                Id = "123456",
                EmployerAccountId = Config.EmployerAccountId,
                Amount = 1234,
                CollectionPeriod = new CollectionPeriod
                {
                    Id = "12345",
                    Month = DateTime.Now.Month,
                    Year = DateTime.Now.Year
                },
                EarningDetails = new EarningDetails
                {
                    ActualEndDate = DateTime.Now.AddDays(3),
                    StartDate = DateTime.Now.AddDays(-10),
                    PlannedEndDate = DateTime.Now.AddDays(2),
                    TotalInstallments = 10,
                    MonthlyInstallment = 123654,
                    CompletionAmount = 10000,
                    EndpointAssessorId = "654321",
                    CompletionStatus = 1,
                    RequiredPaymentId = Guid.NewGuid()
                },
                ApprenticeshipId = payment.ApprenticeshipId,
                ProviderName = "ProviderName",
                Ukprn = payment.ProviderId,
                ApprenticeName = "ApprenticeName",
                CourseLevel = 1,
                CourseName = "CourseName",
                Uln = 12341412,
                CourseStartDate = DateTime.Now.AddDays(-10),
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
                foreach (var payment in Payments)
                {
                    Console.WriteLine($"Looking for Payments. Employer Account Id: {Config.EmployerAccountId}, Month: {DateTime.Now.Month}, Year: {DateTime.Now.Year}");
                    var parameters = new DynamicParameters();
                    parameters.Add("@employerAccountId", Config.EmployerAccountId, DbType.Int64);
                    parameters.Add("@collectionPeriodYear", DateTime.Now.Year, DbType.Int32);
                    parameters.Add("@collectionPeriodMonth", DateTime.Now.Month, DbType.Int32);
                    parameters.Add("@providerId", payment.ProviderId, DbType.Int64);
                    var count = Connection.ExecuteScalar<int>("Select Count(*) from Payment where employerAccountId = @employerAccountId and CollectionPeriodYear = @collectionPeriodYear and CollectionPeriodMonth = @collectionPeriodMonth and ProviderId = @providerId",
                        parameters, commandType: CommandType.Text);
                    return count == 1;
                }
                return false;
            }, "Failed to find all the payments.");
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

        [Then(@"there are (.*) payment events stored")]
        public void ThenThereArePaymentEventsStored(int p0)
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the event with invalid data is not stored")]
        public void ThenTheEventWithInvalidDataIsNotStored()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
