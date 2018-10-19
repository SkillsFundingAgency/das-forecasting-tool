using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.AcceptanceTests.Handlers
{
   public class AccountProjectionCreatedEventHandler :IHandleMessages<AccountProjectionCreatedEvent>
    {
        public static ConcurrentBag<AccountProjectionCreatedEvent> ReceivedEvents { get; } = new ConcurrentBag<AccountProjectionCreatedEvent>();
        public async Task Handle(AccountProjectionCreatedEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine(message.ToJson());
            ReceivedEvents.Add(message);
        }
    }
}
