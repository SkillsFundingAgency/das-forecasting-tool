using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Bogus;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Messages.Projections;
using SFA.DAS.Provider.Events.Api.Types;
using CalendarPeriod = SFA.DAS.Forecasting.Application.Payments.Messages.CalendarPeriod;
using NamedCalendarPeriod = SFA.DAS.Forecasting.Application.Payments.Messages.NamedCalendarPeriod;

namespace SFA.DAS.Forecasting.PerformanceTests.Payment
{
    [TestFixture]
    public class PaymentProcessingTests : BaseProcessingTests
    {
        [SetUp]
        public void SetUp()
        {

        }

        protected Faker<PaymentCreatedMessage> CreateFakePayments(params long[] accountIds)
        {
            var paymentFaker = new Faker<PaymentCreatedMessage>()
                .RuleFor(payment => payment.Id, faker => Guid.NewGuid().ToString("D"))
                .RuleFor(payment => payment.EmployerAccountId,
                    faker => faker.PickRandom(accountIds))
                .RuleFor(payment => payment.Amount, faker => faker.Finance.Amount(1))
                .RuleFor(payment => payment.ApprenticeName, faker => faker.Person.FullName)
                .RuleFor(payment => payment.CourseName, faker => "Test Course")
                .RuleFor(payment => payment.ProviderName, faker => faker.Company.CompanyName())
                .RuleFor(payment => payment.ApprenticeshipId, faker => faker.Random.Long(999999, 9999999))
                .RuleFor(payment => payment.CollectionPeriod,
                    faker => new NamedCalendarPeriod
                    { Id = "1819-R01", Year = DateTime.Today.Year, Month = DateTime.Today.Month })
                .RuleFor(payment => payment.CourseLevel, faker => faker.Random.Number(1, 10))
                .RuleFor(payment => payment.CourseStartDate,
                    faker => faker.Date.Between(DateTime.Today.AddYears(-1), DateTime.Today.AddYears(2)))
                .RuleFor(payment => payment.DeliveryPeriod,
                    faker => new CalendarPeriod { Year = DateTime.Today.Year, Month = DateTime.Today.Month })
                .RuleFor(payment => payment.FundingSource, faker => FundingSource.Levy)
                .RuleFor(payment => payment.Ukprn, faker => faker.Random.Long(1, 100))
                .RuleFor(payment => payment.EarningDetails, faker => new EarningDetails
                {
                    CompletionAmount = 2000,
                    MonthlyInstallment = 133,
                    TotalInstallments = faker.Random.Number(12, 24),
                    ActualEndDate = DateTime.MinValue
                });

            paymentFaker.FinishWith((faker, message) =>
            {
                message.Uln = message.ApprenticeshipId;
                message.EarningDetails.PaymentId = message.Id;
                message.EarningDetails.PlannedEndDate =
                    message.CourseStartDate.Value.AddMonths(message.EarningDetails.TotalInstallments);
                message.EarningDetails.StartDate = message.CourseStartDate.Value;
                message.SendingEmployerAccountId = message.EmployerAccountId;
            });
            return paymentFaker;
        }

        private List<PaymentCreatedMessage> SendPayments(Faker<PaymentCreatedMessage> paymentFaker, int count = 10)
        {
            var account = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"]?.ConnectionString);
            var queue = account.CreateCloudQueueClient().GetQueueReference(QueueNames.PaymentValidator);
            Assert.IsTrue(queue.Exists(), $"Queue not found: {QueueNames.PaymentValidator}");
            var payments = new List<PaymentCreatedMessage>();
            for (var i = 0; i < count; i++)
            {
                var payment = paymentFaker.Generate();
                var payload = JsonConvert.SerializeObject(payment);
                queue.AddMessage(new CloudQueueMessage(payload));
                payments.Add(payment);
            }
            return payments;
        }

        private void FindPayments(ForecastingDataContext dataContext, List<PaymentCreatedMessage> payments)
        {
            var timeToWait = TimeSpan.Parse(ConfigurationManager.AppSettings["TimeToWait"]);
            var timeToPause = TimeSpan.Parse(ConfigurationManager.AppSettings["TimeToPause"]);
            var endTime = DateTime.Now.Add(timeToWait);
            while (DateTime.Now < endTime)
            {
                var payment = payments.FirstOrDefault();
                if (payment == null)
                    break;

                if (dataContext.Payments.Any(p => p.EmployerAccountId == payment.EmployerAccountId &&
                                                  p.ApprenticeshipId == payment.ApprenticeshipId &&
                                                  p.LearnerId == payment.Uln))
                {
                    payments.Remove(payment);
                    Console.WriteLine($"Found payment: {payment.Id}, {payment.EmployerAccountId}, {payment.ApprenticeName}");
                    continue;
                }
                Console.WriteLine($"Payment not found: {payment.Id}, {payment.EmployerAccountId}, {payment.ApprenticeName}");
                Thread.Sleep(timeToPause);
            }
        }

        [Test]
        public void Test_Payments_Random()
        {
            AccountIds = new long[] { 9912345 };
            var dataContext = new ForecastingDataContext(ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString);
            //dataContext.Database.ExecuteSqlCommand(
            //    $"delete from AccountProjectionCommitment where EmployerAccountId = 9912345 or EmployerAccountId =  9923451 or EmployerAccountId =  9934512 or EmployerAccountId =  9945123 or EmployerAccountId = 9951234 or EmployerAccountId = 9954321");
            dataContext.Database.ExecuteSqlCommand(
                $"delete from AccountProjection where EmployerAccountId = 9912345 or EmployerAccountId =  9923451 or EmployerAccountId =  9934512 or EmployerAccountId =  9945123 or EmployerAccountId = 9951234 or EmployerAccountId = 9954321");
            dataContext.Database.ExecuteSqlCommand(
                $"delete from Commitment where EmployerAccountId = 9912345 or EmployerAccountId =  9923451 or EmployerAccountId =  9934512 or EmployerAccountId =  9945123 or EmployerAccountId = 9951234 or EmployerAccountId = 9954321");
            dataContext.Database.ExecuteSqlCommand(
                $"delete from Payment where EmployerAccountId = 9912345 or EmployerAccountId =  9923451 or EmployerAccountId =  9934512 or EmployerAccountId =  9945123 or EmployerAccountId = 9951234 or EmployerAccountId = 9954321");

            var paymentFaker = CreateFakePayments(AccountIds);
            RemoveEmployerProjectionAuditDocuments(ProjectionSource.PaymentPeriodEnd, AccountIds);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var payments = SendPayments(paymentFaker);
            FindPayments(dataContext, payments);
            stopwatch.Stop();
            if (!payments.Any())
            {
                Console.WriteLine($"Found all payments. Took: {stopwatch.ElapsedMilliseconds}ms");
            }
            payments.ForEach(payment => Console.WriteLine($"Failed to find payment: {payment.Id}, {payment.EmployerAccountId}, {payment.ApprenticeName}"));
            Assert.Fail("Failed to find all the payments");
        }
    }
}