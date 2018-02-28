using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    public class InitialiseFunction: IFunction
    {
        [FunctionName("InitialiseFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req,
            ExecutionContext executionContext,
            TraceWriter log)
        {
            await FunctionRunner.Run<InitialiseFunction>(log, executionContext, async (container,logger) =>
            {
                log.Info("Initialising the Levy functions.");

                var functions = Assembly.GetExecutingAssembly()
                    .GetExportedTypes()
                    .Where(type => !type.IsAbstract && type.IsClass && typeof(IFunction).IsAssignableFrom(type))
                    .ToList();

                foreach (var function in functions)
                {
                    var triggerAttribute = function.GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .SelectMany(method => method.GetParameters())
                        .SelectMany(paramInfo => paramInfo.GetCustomAttributes(typeof(QueueTriggerAttribute)))
                        .Cast<QueueTriggerAttribute>()
                        .FirstOrDefault();

                    if (triggerAttribute == null)
                    {
                        log.Verbose("No QueueTrigger found.");
                        continue;
                    }

                    logger.Debug($"Now creating queue: {triggerAttribute.QueueName}");
                    var configuration = container.GetInstance<IApplicationConfiguration>();
                    var client = CloudStorageAccount.Parse(configuration.StorageConnectionString).CreateCloudQueueClient();
                    var queue = client.GetQueueReference(triggerAttribute.QueueName);
                    await queue.CreateIfNotExistsAsync();
                }
            });

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
