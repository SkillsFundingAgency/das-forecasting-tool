using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;
using GetApprenticeshipsResponse = SFA.DAS.CommitmentsV2.Api.Types.Responses.GetApprenticeshipsResponse;

namespace SFA.DAS.Forecasting.StubApi.Functions
{
    public class GetEmployerApprenticeships
    {
        private readonly IConfiguration _configuration;

        public GetEmployerApprenticeships(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [FunctionName("GetEmployerApprenticeships")]
        public async Task<HttpResponseMessage> Run([
            HttpTrigger(AuthorizationLevel.Function, "get",
            Route = "employer/{employerAccountId}/apprenticeships/search")]HttpRequestMessage req, string employerAccountId, TraceWriter writer)
        {
            if (_configuration["StubData"] != null && _configuration["StubData"]
                    .Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                return CreateFakeCommitments(employerAccountId,_configuration["NumberOfCommitments"]);
            }

            StubDataStore.Apprenticeships.TryGetValue(employerAccountId, out var data);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(data, System.Text.Encoding.UTF8, "application/json")
            };
        }

        private static HttpResponseMessage CreateFakeCommitments(string employerAccountId, string numberOfCommitmentsConfigValue)
        {
            var fakeCommitment = CreateFakeCommitment(employerAccountId);

            var numberOfCommitments = Convert.ToInt32(numberOfCommitmentsConfigValue);


            var data = SendCommitments(fakeCommitment, numberOfCommitments);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(data, System.Text.Encoding.UTF8, "application/json")
            };
        }

        private static Faker<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse> CreateFakeCommitment(string employerAccountId)
        {
            return new Faker<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>()
                .RuleFor(commitment => commitment.Id, faker => faker.Random.Long(99999, 9999999))
                .RuleFor(commitment => commitment.AccountLegalEntityId, Convert.ToInt64(employerAccountId))
                .RuleFor(commitment => commitment.TransferSenderId, Convert.ToInt64(employerAccountId))
                .RuleFor(commitment => commitment.Cost, faker => faker.Random.Int(300, 1000))
                .RuleFor(commitment => commitment.PaymentStatus, CommitmentsV2.Types.PaymentStatus.Active)
                .RuleFor(commitment => commitment.StartDate, faker => DateTime.Now.AddMonths(faker.Random.Int(1, 12)))
                .RuleFor(commitment => commitment.EndDate, faker => DateTime.Now.AddMonths(faker.Random.Int(15, 30)))
                .RuleFor(commitment => commitment.FirstName, faker => faker.Name.FirstName())
                .RuleFor(commitment => commitment.LastName, faker => faker.Name.LastName())
                .RuleFor(commitment => commitment.Uln, faker => faker.Random.Long(99999, 999999).ToString())
                .RuleFor(commitment => commitment.CourseCode, "107")
                .RuleFor(commitment => commitment.CourseName, "Embedded electronic systems design and development engineer")
                .RuleFor(commitment => commitment.ProviderId, 5)
                .RuleFor(commitment => commitment.ProviderName, "providername");
        }

        private static string SendCommitments(Faker<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse> fakeCommitment, int count = 10)
        {
            var commitments = new List<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>();
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