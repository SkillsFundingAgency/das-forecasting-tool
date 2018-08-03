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
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Models.Payments;
using CalendarPeriod = SFA.DAS.Forecasting.Application.Payments.Messages.CalendarPeriod;
using FundingSource = SFA.DAS.Provider.Events.Api.Types.FundingSource;
using NamedCalendarPeriod = SFA.DAS.Forecasting.Application.Payments.Messages.NamedCalendarPeriod;

namespace SFA.DAS.Forecasting.PerformanceTests.Levy
{
    [TestFixture]
    public class PaymentProcessingTests
    {
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void Test_Payments()
        {
            var accountIds = new[] { 9912345, 9923451, 9934512, 9945123, 9951234, 9954321 };
            var paymentFaker = new Faker<PaymentCreatedMessage>()
                .RuleFor(payment => payment.Id, faker => Guid.NewGuid().ToString("D"))
                .RuleFor(payment => payment.EmployerAccountId,
                    faker => faker.PickRandom(accountIds))
                .RuleFor(payment => payment.Amount, faker => faker.Finance.Amount(1))
                .RuleFor(payment => payment.ApprenticeName, faker => faker.Person.FullName)
                .RuleFor(payment => payment.CourseName, faker => "Test Course")
                .RuleFor(payment => payment.ProviderName, faker => faker.Company.CompanyName())
                .RuleFor(payment => payment.ApprenticeshipId, faker => faker.Random.Long(999999,9999999))
                .RuleFor(payment => payment.CollectionPeriod,
                    faker => new NamedCalendarPeriod
                        {Id = "1819-R01", Year = DateTime.Today.Year, Month = DateTime.Today.Month})
                .RuleFor(payment => payment.CourseLevel, faker => faker.Random.Number(1, 10))
                .RuleFor(payment => payment.CourseStartDate,
                    faker => faker.Date.Between(DateTime.Today.AddYears(-1), DateTime.Today.AddYears(2)))
                .RuleFor(payment => payment.DeliveryPeriod,
                    faker => new CalendarPeriod {Year = DateTime.Today.Year, Month = DateTime.Today.Month})
                .RuleFor(payment => payment.FundingSource, faker => FundingSource.Levy)
                .RuleFor(payment => payment.Ukprn, faker => faker.Random.Long(1,100))
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
            var account = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"]?.ConnectionString);
            var queue = account.CreateCloudQueueClient().GetQueueReference(QueueNames.PaymentValidator);
            Assert.IsTrue(queue.Exists(), $"Queue not found: {QueueNames.PaymentValidator}");
            var payments = new List<PaymentCreatedMessage>();
            var dataContext = new ForecastingDataContext(ConfigurationManager.ConnectionStrings["DatabaseConnectionString"]
                .ConnectionString);
            //dataContext.Database.ExecuteSqlCommand(
            //    $"delete from AccountProjectionCommitment where EmployerAccountId = 9912345 or EmployerAccountId =  9923451 or EmployerAccountId =  9934512 or EmployerAccountId =  9945123 or EmployerAccountId = 9951234 or EmployerAccountId = 9954321");
            dataContext.Database.ExecuteSqlCommand(
                $"delete from AccountProjection where EmployerAccountId = 9912345 or EmployerAccountId =  9923451 or EmployerAccountId =  9934512 or EmployerAccountId =  9945123 or EmployerAccountId = 9951234 or EmployerAccountId = 9954321");            
            dataContext.Database.ExecuteSqlCommand(
                $"delete from Commitment where EmployerAccountId = 9912345 or EmployerAccountId =  9923451 or EmployerAccountId =  9934512 or EmployerAccountId =  9945123 or EmployerAccountId = 9951234 or EmployerAccountId = 9954321");            
            dataContext.Database.ExecuteSqlCommand(
                $"delete from Payment where EmployerAccountId = 9912345 or EmployerAccountId =  9923451 or EmployerAccountId =  9934512 or EmployerAccountId =  9945123 or EmployerAccountId = 9951234 or EmployerAccountId = 9954321");            

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (var i = 0; i < 100; i++)
            {
                var payment = paymentFaker.Generate();
                var payload = JsonConvert.SerializeObject(payment);
                queue.AddMessage(new CloudQueueMessage(payload));
                payments.Add(payment);
            }
        }
    }

    [TestFixture]
    public class LevyProcessingTests
    {
        public void SetUp()
        {

        }


        [Test]
        public void Test_Levy_Declarations()
        {
            var accountIds = new[] { 9912345, 9923451, 9934512, 9945123, 9951234, 9954321 };
            var levyFaker = new Faker<LevySchemeDeclarationUpdatedMessage>()
                .RuleFor(levy => levy.Id, faker => faker.IndexFaker)
                .RuleFor(levy => levy.AccountId,
                    faker => faker.PickRandom(accountIds))
                .RuleFor(levy => levy.CreatedAt, faker => DateTime.Now)
                .RuleFor(levy => levy.CreatedDate, faker => DateTime.Now)
                .RuleFor(levy => levy.EmpRef, faker => $"{faker.Random.AlphaNumeric(4)}/{faker.Random.Number(1, 999)}")
                .RuleFor(levy => levy.EndOfYearAdjustment, faker => false)
                .RuleFor(levy => levy.PayrollMonth, faker => (short)1)
                .RuleFor(levy => levy.PayrollYear, faker => "18-19")
                .RuleFor(levy => levy.LevyDeclaredInMonth, faker => faker.Random.Decimal(50, 1000))
                .RuleFor(levy => levy.SubmissionDate, faker => faker.Date.Recent());

            var account = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"]?.ConnectionString);
            var queue = account.CreateCloudQueueClient().GetQueueReference(QueueNames.LevyValidateDeclaration);
            Assert.IsTrue(queue.Exists(), $"Queue not found: {QueueNames.LevyValidateDeclaration}");
            var levyDeclarations = new List<LevySchemeDeclarationUpdatedMessage>();
            var dataContext = new ForecastingDataContext(ConfigurationManager.ConnectionStrings["DatabaseConnectionString"]
                .ConnectionString);
            dataContext.LevyDeclarations.RemoveRange(dataContext.LevyDeclarations
                .Where(levy => accountIds.Any(id => id == levy.EmployerAccountId)).ToList());
            dataContext.SaveChanges();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (var i = 0; i < 100; i++)
            {
                var levyDeclaration = levyFaker.Generate();
                var payload = JsonConvert.SerializeObject(levyDeclaration);
                queue.AddMessage(new CloudQueueMessage(payload));
                levyDeclarations.Add(levyDeclaration);
            }

            var timeToWait = TimeSpan.Parse(ConfigurationManager.AppSettings["TimeToWait"]);
            var timeToPause = TimeSpan.Parse(ConfigurationManager.AppSettings["TimeToPause"]);
            var endTime = DateTime.Now.Add(timeToWait);
            while (DateTime.Now < endTime)
            {
                var levyDeclaration = levyDeclarations.FirstOrDefault();
                if (levyDeclaration == null)
                    break;

                var amount = GetTruncatedAmount(levyDeclaration.LevyDeclaredInMonth);// decimal.Round(levyDeclaration.LevyDeclaredInMonth, 2, MidpointRounding.AwayFromZero);
                if (dataContext.LevyDeclarations.Any(levy => levy.EmployerAccountId == levyDeclaration.AccountId &&
                                                             levy.Scheme == levyDeclaration.EmpRef &&
                                                             levy.PayrollMonth == levyDeclaration.PayrollMonth &&
                                                             levy.PayrollYear == levyDeclaration.PayrollYear &&
                                                             levy.LevyAmountDeclared == amount))
                {
                    levyDeclarations.Remove(levyDeclaration);
                    Console.WriteLine($"Found levy declaration: {levyDeclaration.AccountId}, {levyDeclaration.EmpRef}, {levyDeclaration.LevyDeclaredInMonth}");
                    continue;
                }
                Console.WriteLine($"Levy declaration not found: {levyDeclaration.AccountId}, {levyDeclaration.EmpRef}, {amount}");
                Thread.Sleep(timeToPause);
            }
            stopwatch.Stop();
            if (levyDeclarations.Any())
            {
                levyDeclarations.ForEach(levy => Console.WriteLine($"Failed to find levy declaration: {levy.AccountId}, {levy.EmpRef}, {levy.LevyDeclaredInMonth}"));
                Assert.Fail("Failed to find all the levy declarations");
            }
            Console.WriteLine($"Found all levy declarations. Took: {stopwatch.ElapsedMilliseconds}ms");
        }


        private decimal GetTruncatedAmount(decimal value)
        {
            return decimal.Round(value - (value % .01M), 2);
        }
    }
}