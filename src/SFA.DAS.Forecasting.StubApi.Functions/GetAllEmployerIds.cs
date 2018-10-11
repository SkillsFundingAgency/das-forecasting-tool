using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace SFA.DAS.Forecasting.StubApi.Functions
{
    public static class GetAllEmployerIds
    {
        [FunctionName("GetAllEmployerIds")]
        public static async Task<IEnumerable<long>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "employer/ids")]HttpRequestMessage req, 
            TraceWriter writer)
        {
            writer.Info("C# HTTP trigger function processed a request.");

            return
                StubDataStore.Apprenticeships
                    .Select(m => long.Parse(m.Key))
                    .Distinct();
        }
    }
}
