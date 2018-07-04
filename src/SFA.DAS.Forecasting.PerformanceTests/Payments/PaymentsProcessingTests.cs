using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bogus;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Models.Payments;
using SFA.DAS.Forecasting.PerformanceTests.Infrastructure;
using NamedCalendarPeriod = SFA.DAS.Forecasting.Application.Payments.Messages.NamedCalendarPeriod;

namespace SFA.DAS.Forecasting.PerformanceTests.Payments
{
    [TestFixture]
    public class PaymentsProcessingTests
    {
        private FunctionFunner functionFunner;
        private CloudStorageAccount account;
        private CloudQueueClient cloudQueueClient;

        [SetUp]
        public void SetUp()
        {
            account = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"]?.ConnectionString);
            cloudQueueClient = account.CreateCloudQueueClient();
            ClearQueues();

            functionFunner = new FunctionFunner();
            functionFunner.StartFunction("SFA.DAS.Forecasting.Payments.Functions");
            functionFunner.StartFunction("SFA.DAS.Forecasting.StubApi.Functions");
        }

        private void ClearQueues()
        {
            cloudQueueClient.ClearQueue(QueueNames.Payments.PaymentValidator);
            cloudQueueClient.ClearQueue(QueueNames.Payments.PaymentProcessor);
            cloudQueueClient.ClearQueue(QueueNames.Payments.CommitmentProcessor);
            cloudQueueClient.ClearQueue(QueueNames.Payments.AllowProjection);
        }

        [TearDown]
        public void CleanUp()
        {
            functionFunner?.StopFunctions();
        }


        [Test]
        public void Test_Payment_Processing()
        {
            var accountIds = new[] { 9912345, 9923451, 9934512, 9945123, 9951234, 9954321 };
            var senderAccountIds = new[] { 99012345, 99023451, 99034512, 99045123, 99051234, 99054321 };
            var apprenticeshipId = 999999;
            var paymentFaker = new Faker<PaymentCreatedMessage>()
                .RuleFor(payment => payment.Id, faker => Guid.NewGuid().ToString("D"))
                .RuleFor(payment => payment.Uln, faker => faker.IndexFaker)
                .RuleFor(payment => payment.ApprenticeshipId, faker => ++apprenticeshipId)
                .RuleFor(payment => payment.EmployerAccountId,
                    faker => faker.PickRandom(accountIds))
                .RuleFor(payment => payment.CourseStartDate,
                    faker => faker.Date.Between(DateTime.Now.AddYears(-2), DateTime.Now.AddMonths(-2)))
                .RuleFor(payment => payment.FundingSource,
                    faker => faker.PickRandom(new[] { FundingSource.Levy, FundingSource.Transfer }))
                .RuleFor(payment => payment.ApprenticeName, faker => faker.Name.FullName())
                .RuleFor(payment => payment.Amount, faker => 250)
                .RuleFor(payment => payment.CollectionPeriod, faker => new NamedCalendarPeriod
                {
                    Id = "1718-R01",
                    Month = DateTime.Now.Month,
                    Year = DateTime.Now.Year
                })
                .RuleFor(payment => payment.CourseLevel, faker => faker.Random.Number(3))
                .RuleFor(payment => payment.CourseName, faker => faker.Name.JobTitle())
                .RuleFor(payment => payment.DeliveryPeriod,
                    faker => new Application.Payments.Messages.CalendarPeriod
                    {
                        Month = DateTime.Now.Month,
                        Year = DateTime.Now.Year
                    })
                .RuleFor(payment => payment.ProviderName, faker => faker.Company.CompanyName())
                .RuleFor(payment => payment.Ukprn, faker => faker.Random.Number(100))
                .FinishWith((faker, payment) =>
                    {
                        payment.SendingEmployerAccountId = payment.FundingSource == FundingSource.Levy
                            ? payment.EmployerAccountId
                            : faker.PickRandom(senderAccountIds);
                        payment.EarningDetails = new EarningDetails
                        {
                            ApprenticeshipId = payment.ApprenticeshipId,
                            CompletionAmount = 2000,
                            ActualEndDate = DateTime.MinValue,
                            MonthlyInstallment = 250,
                            PaymentId = Guid.NewGuid().ToString("D"),
                            StartDate = payment.CourseStartDate.Value,
                            CompletionStatus = 0,
                            RequiredPaymentId = Guid.NewGuid(),
                            PlannedEndDate = payment.CourseStartDate.Value.AddYears(2),
                            TotalInstallments = 24

                        };
                    });
            var payments = new List<PaymentCreatedMessage>();
            var dataContext = new ForecastingDataContext(ConfigurationManager.ConnectionStrings["DatabaseConnectionString"]
                .ConnectionString);
            dataContext.Payments.RemoveRange(dataContext.Payments
                .Where(payment => accountIds.Any(id => id == payment.EmployerAccountId)).ToList());
            dataContext.SaveChanges();
            
            var queue = cloudQueueClient.GetQueueReference(QueueNames.Payments.PaymentValidator);
            Assert.IsTrue(queue.Exists(), $"Queue not found: {QueueNames.Payments.PaymentValidator}");
            //There is an issue with structrmap/nlog where the first set of concurrent calls to the container hang the thread then cause the func to fail.
            for (var i = 0; i < 32; i++)
            {
                var payment = paymentFaker.Generate();
                queue.SendPayment(payment);
                payments.Add(payment);
            }
            Thread.Sleep(TimeSpan.FromSeconds(60));
            ClearQueues();
            Console.WriteLine("Sending message init the payments app. Waiting for 5 seconds.");
            Thread.Sleep(TimeSpan.FromSeconds(5));
            Console.WriteLine("Now generating the test payments.");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (var i = 0; i < 100; i++)
            {
                var payment = paymentFaker.Generate();
                queue.SendPayment(payment);
                payments.Add(payment);
            }

            var timeToWait = Config.Instance.TimeToWait;
            var timeToPause = Config.Instance.TimeToPause;
            var endTime = DateTime.Now.Add(timeToWait);
            while (DateTime.Now < endTime)
            {
                var payment = payments.FirstOrDefault();
                if (payment == null)
                    break;

                if (dataContext.Payments.Any(p => p.ApprenticeshipId == payment.ApprenticeshipId && p.EmployerAccountId == payment.EmployerAccountId))
                {
                    payments.Remove(payment);
                    Console.WriteLine($"Found payment: {payment.ApprenticeshipId}, {payment.ApprenticeName}, {payment.FundingSource:G}");
                    continue;
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Payment not found: {payment.ApprenticeshipId}, {payment.ApprenticeName}, {payment.FundingSource:G}");
                Console.ResetColor();
                Thread.Sleep(timeToPause);
            }
            stopwatch.Stop();
            if (payments.Any())
            {
                payments.ForEach(payment =>
                {
                    if (!dataContext.Payments.Any(p =>
                        p.ApprenticeshipId == payment.ApprenticeshipId &&
                        p.EmployerAccountId == payment.EmployerAccountId))
                        Console.WriteLine(
                            $"No Payment was recorded for test payment: {payment.ApprenticeshipId}, {payment.ApprenticeName}, {payment.FundingSource:G}");
                });
                Assert.Fail("Failed to find all the payments");
            }
            Console.WriteLine($"Found all payments. Took: {stopwatch.ElapsedMilliseconds}ms");
        }

        
    }
}