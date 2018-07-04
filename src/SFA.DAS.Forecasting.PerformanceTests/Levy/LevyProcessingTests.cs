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
using SFA.DAS.Forecasting.Data;

namespace SFA.DAS.Forecasting.PerformanceTests.Levy
{
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

                var amount = levyDeclaration.LevyDeclaredInMonth.GetTruncatedAmount();// decimal.Round(levyDeclaration.LevyDeclaredInMonth, 2, MidpointRounding.AwayFromZero);
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
    }
}