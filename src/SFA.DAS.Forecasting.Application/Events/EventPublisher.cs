using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Events;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Events
{
    public class EventPublisher: IEventPublisher
    {
        public ILog Logger { get; }
        public IMediator Mediator { get; }

        public EventPublisher(ILog logger, IMediator mediator)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task Publish<T>(T domainEvent) // where T: INotification
        {
            //TODO: Don't want to have to reference mediatr in domain but really really don't want to write custom DomainEvents api so may have to take that hit.
            Logger.Debug($"Publishing domain event: {domainEvent.ToJson()}");
            //await Mediator.Publish(domainEvent);
            return Task.CompletedTask;
        }
    }
}