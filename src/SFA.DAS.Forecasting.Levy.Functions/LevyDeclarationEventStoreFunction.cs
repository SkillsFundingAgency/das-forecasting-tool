using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Levy.Handlers;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class LevyDeclarationEventStoreFunction : IFunction
    {
        [FunctionName("LevyDeclarationEventStoreFunction")]
        public static async Task Run(
            [QueueTrigger(QueueNames.StoreLevyDeclaration)]LevySchemeDeclarationUpdatedMessage levySchemeUpdatedMessage,
            TraceWriter writer)
        {
            await FunctionRunner.Run<LevyDeclarationEventStoreFunction>(writer,
                async (container, logger) =>
                {
                    logger.Debug("Getting levy declaration handler from container.");
                    var handler = container.GetInstance<StoreLevyDeclarationHandler>();
                    if (handler==null)
                        throw new InvalidOperationException($"Faild to get levy handler from container.");
                    await handler.Handle(levySchemeUpdatedMessage);
                    var config = container.GetInstance<IApplicationConfiguration>();
                    var storageAccount = CloudStorageAccount.Parse(config.StorageConnectionString);
                    var queueClient = storageAccount.CreateCloudQueueClient();
                    var queue = queueClient.GetQueueReference(QueueNames.AllowAggregation);
                    await queue.CreateIfNotExistsAsync();
                    queue.AddMessage(new CloudQueueMessage(levySchemeUpdatedMessage.ToJson()),null,TimeSpan.FromSeconds(config.SecondsToWaitToAllowProjections));
                    logger.Info($"Finished handling levy declaration for EmployerAccountId: {levySchemeUpdatedMessage.AccountId}, PayrollYear: {levySchemeUpdatedMessage.PayrollYear}, month: {levySchemeUpdatedMessage.PayrollMonth}, scheme: {levySchemeUpdatedMessage.EmpRef}");
                });
        }
    }
}
