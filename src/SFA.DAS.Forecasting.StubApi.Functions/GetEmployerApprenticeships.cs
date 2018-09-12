using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.StubApi.Functions
{
    public static class GetEmployerApprenticeships
    {
        [FunctionName("GetEmployerApprenticeships")]
        public static async Task<HttpResponseMessage> Run([
            HttpTrigger(AuthorizationLevel.Function, "get",
            Route = "employer/{employerAccountId}/apprenticeships/")]HttpRequestMessage req, string employerAccountId, TraceWriter writer)
        {

            IEnumerable<Commitments.Api.Types.Apprenticeship.Apprenticeship> data;
            StubDataStore.Apprenticeships.TryGetValue(employerAccountId, out data);

            var d = JsonConvert.SerializeObject(data);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(d, Encoding.UTF8, "application/json")
            };
        }
    }
}