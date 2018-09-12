using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.Forecasting.StubApi.Functions
{
    public static class AddApprenticeshipsFunction
    {
        [FunctionName("AddLevyFunction")]
        public static async Task<HttpResponseMessage> Run([
            HttpTrigger(AuthorizationLevel.Function, "post",
            Route = "data/employer/{employerAccountId}/apprenticeships/")]HttpRequestMessage req, string employerAccountId, TraceWriter writer)
        {
            writer.Info($"C# HTTP trigger for {nameof(AddApprenticeshipsFunction)} POST.");

            var body = await req.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<IEnumerable<Apprenticeship>>(body);

            if (StubDataStore.Apprenticeships.ContainsKey(employerAccountId))
                StubDataStore.Apprenticeships.Remove(employerAccountId);

            StubDataStore.Apprenticeships.Add(employerAccountId, data);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($"Data added for {employerAccountId}", Encoding.UTF8, "application/json")
            };
        }
    }
}
