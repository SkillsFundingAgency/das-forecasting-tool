using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Functions.Framework.Infrastructure
{
    public interface IFunctionInitialisationService
    {
        Task Initialise<T>() where T : IFunction;
    }

    public class FunctionInitialisationService : IFunctionInitialisationService
    {
        private readonly ILog _logger;
        private readonly IApplicationConfiguration _configuration;

        public FunctionInitialisationService(ILog logger, IApplicationConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task Initialise<T>() where T: IFunction
        {
            var functions = typeof(T).Assembly
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
                    _logger.Info($"No QueueTrigger found on function: {function.FullName}.");
                    continue;
                }

                _logger.Debug($"Now creating queue: {triggerAttribute.QueueName}");
                var client = CloudStorageAccount.Parse(_configuration.StorageConnectionString).CreateCloudQueueClient();
                var queue = client.GetQueueReference(triggerAttribute.QueueName);
                await queue.CreateIfNotExistsAsync();
            }
        }
    }
}
