using System;
using System.Threading.Tasks;

namespace SFA.DAS.Messaging.POC
{
    public interface IMessagePublisher
    {
        Task Publish<T>(T @event);
    }

    public class MessagePublisher : IMessagePublisher
    {
        public IMessageSender MessageSender { get; }
        public ISubscriptionService SubscriptionService { get; }

        public MessagePublisher(IMessageSender messageSender, ISubscriptionService subscriptionService)
        {
            MessageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            SubscriptionService = subscriptionService ?? throw new ArgumentNullException(nameof(subscriptionService));
        }

        public async Task Publish<T>(T @event)
        {
            var subscriptions = await SubscriptionService.GetSubscriptions<T>();
            foreach (var subscription in subscriptions)
            {
                try
                {
                    await MessageSender.Send(subscription.QueueName, @event);
                }
                catch
                {
                    //TODO: log exception here
                }
            }
        }
    }
}