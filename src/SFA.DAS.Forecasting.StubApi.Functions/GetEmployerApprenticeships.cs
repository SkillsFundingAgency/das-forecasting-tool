using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;

namespace SFA.DAS.Forecasting.StubApi.Functions
{
    public static class GetEmployerApprenticeships
    {
        [FunctionName("GetEmployerApprenticeships")]
        public static async Task<HttpResponseMessage> Run([
            HttpTrigger(AuthorizationLevel.Function, "get",
            Route = "employer/{employerAccountId}/apprenticeships/search")]HttpRequestMessage req, string employerAccountId, TraceWriter writer)
        {
            if (ConfigurationManager.AppSettings["StubData"] != null && ConfigurationManager.AppSettings["StubData"]
                    .Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                return CreateFakeCommitments(employerAccountId);
            }

            StubDataStore.Apprenticeships.TryGetValue(employerAccountId, out var data);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };
        }

        private static HttpResponseMessage CreateFakeCommitments(string employerAccountId)
        {
            var fakeCommitment = CreateFakeCommitment(employerAccountId);

            var numberOfCommitments = Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfCommitments"]);


            var data = SendCommitments(fakeCommitment, numberOfCommitments);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };
        }

        private static Faker<Apprenticeship> CreateFakeCommitment(string employerAccountId)
        {
            return new Faker<Apprenticeship>()
                .RuleFor(commitment => commitment.Id, faker => faker.Random.Long(99999,9999999))
                .RuleFor(commitment => commitment.EmployerAccountId, Convert.ToInt64(employerAccountId))
                .RuleFor(commitment => commitment.TransferSenderId, Convert.ToInt64(employerAccountId))
                .RuleFor(commitment => commitment.Cost, faker => faker.Random.Int(300,1000))
                .RuleFor(commitment => commitment.AgreementStatus, AgreementStatus.BothAgreed)
                .RuleFor(commitment => commitment.PaymentStatus, PaymentStatus.Active)
                .RuleFor(commitment => commitment.TrainingType, TrainingType.Standard)
                .RuleFor(commitment => commitment.StartDate, faker=> DateTime.Now.AddMonths(faker.Random.Int(1,12)))
                .RuleFor(commitment => commitment.EndDate, faker=> DateTime.Now.AddMonths(faker.Random.Int(15,30)))
                .RuleFor(commitment => commitment.ApprenticeshipName, faker => faker.Company.CompanyName())
                .RuleFor(commitment => commitment.FirstName, faker => faker.Name.FirstName())
                .RuleFor(commitment => commitment.LastName, faker => faker.Name.LastName())
                .RuleFor(commitment => commitment.ULN, faker => faker.Random.Long(99999,999999).ToString())
                .RuleFor(commitment => commitment.TrainingCode, "107")
                .RuleFor(commitment => commitment.TrainingName, "Embedded electronic systems design and development engineer")
                ;
        }

        private static string SendCommitments(Faker<Apprenticeship> fakeCommitment, int count = 10)
        {
            var commitments = new List<Apprenticeship>();
            for (var i = 0; i < count; i++)
            {
                var commitment = fakeCommitment.Generate();
                commitments.Add(commitment);
            }

            var page = new 
            {
                PageSize = 1000,
                TotalApprenticeships = commitments.Count,
                Apprenticeships = commitments.ToArray()
            };
            return JsonConvert.SerializeObject(page);

        }
    }
}