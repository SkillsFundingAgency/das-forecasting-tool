using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Forecasting.StubApi.Functions
{

    public class GetPaymentsFunction
    {
        private readonly IConfiguration _configuration;

        public GetPaymentsFunction(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [FunctionName("GetPaymentsFunction")]
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "payments")]HttpRequestMessage req, 
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function {nameof(GetPaymentsFunction)}.");
            string data;
            if (_configuration["StubData"] != null && _configuration["StubData"].Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                var payments = CreateFakePayments("12345");
                data = SendPayments(payments, 200);
            }
            else
            {
                data = JsonConvert.SerializeObject(StubDataStore.PaymentsData);
            }
            

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };
        }

        private static Faker<Payment> CreateFakePayments(params string[] accountIds)
        {

            var paymentFaker = new Faker<Payment>()
                .RuleFor(payment => payment.Id, faker => Guid.NewGuid().ToString("D"))
                .RuleFor(payment => payment.EmployerAccountId,
                    faker => faker.PickRandom(accountIds))
                .RuleFor(payment => payment.Amount, faker => faker.Finance.Amount(1))
                .RuleFor(payment => payment.ApprenticeshipId, faker => faker.Random.Long(999999, 9999999))
                .RuleFor(payment => payment.CollectionPeriod,
                    faker => new NamedCalendarPeriod
                        { Id = "1819-R01", Year = DateTime.Today.Year, Month = DateTime.Today.Month })
                .RuleFor(payment => payment.DeliveryPeriod,
                    faker => new CalendarPeriod { Year = DateTime.Today.Year, Month = DateTime.Today.Month })
                .RuleFor(payment => payment.FundingSource, faker => FundingSource.Levy)
                .RuleFor(payment => payment.Ukprn, faker => faker.Random.Long(1, 100))
                .RuleFor(payment => payment.EarningDetails, faker => new List<Earning>
                {
                    new Earning
                    {
                        CompletionAmount = 2000,
                        MonthlyInstallment = 133,
                        TotalInstallments = faker.Random.Number(12, 24),
                        ActualEndDate = DateTime.MinValue}
                    
                });

            paymentFaker.FinishWith((faker, message) =>
            {
                message.Uln = message.ApprenticeshipId.Value;
                message.EarningDetails.FirstOrDefault().RequiredPaymentId = Guid.Parse(message.Id);
                message.EarningDetails.FirstOrDefault().PlannedEndDate =
                    DateTime.Today.AddMonths(message.EarningDetails.FirstOrDefault().TotalInstallments);
                message.EarningDetails.FirstOrDefault().StartDate = DateTime.Today;
            });
            return paymentFaker;
        }

        private static string SendPayments(Faker<Payment> paymentFaker, int count = 10)
        {
            var payments = new List<Payment>();
            for (var i = 0; i < count; i++)
            {
                var payment = paymentFaker.Generate();
                payments.Add(payment);
            }


            var page = new PageOfResults<Payment>
            {
                PageNumber = 1,
                TotalNumberOfPages = 1,
                Items = payments.ToArray()
            };
            return JsonConvert.SerializeObject(page);
            
        }
    }
}