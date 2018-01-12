using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using StructureMap;

namespace SFA.DAS.Messaging.POC.Infrastructure.Registries
{
    public class MessagingRegistry: Registry
    {
        public MessagingRegistry()
        {
            ForSingletonOf<CloudStorageAccount>().Use(CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]));
            ForSingletonOf<CloudQueueClient>().Use(c => c.GetInstance<CloudStorageAccount>().CreateCloudQueueClient());
            ForSingletonOf<CloudTableClient>().Use(c => c.GetInstance<CloudStorageAccount>().CreateCloudTableClient());
            ForSingletonOf<ISubscriptionService>().Use<SubscriptionService>();
            For<IMessageSender>().Use<MessageSender>();
            For<IMessagePublisher>().Use<MessagePublisher>();
        }
    }
}