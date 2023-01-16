using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Forecasting.StubApi.Functions
{
    public class AddApprenticeshipsFunction
    {
        [FunctionName("AddApprenticeshipsFunction")]
        public async Task<HttpResponseMessage> Run([
            HttpTrigger(AuthorizationLevel.Function, "post",
            Route = "data/employer/{employerAccountId}/apprenticeships")]HttpRequestMessage req, string employerAccountId, ILogger logger)
        {
            logger.LogInformation($"C# HTTP trigger for {nameof(AddApprenticeshipsFunction)} POST.");

            var body = await req.Content.ReadAsStringAsync();

            if (StubDataStore.Apprenticeships.ContainsKey(employerAccountId))
                StubDataStore.Apprenticeships.Remove(employerAccountId);

            StubDataStore.Apprenticeships.Add(employerAccountId, body);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($"Data added for {employerAccountId}", Encoding.UTF8, "application/json")
            };
        }
    }
}
