using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Payments;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Payments.Handlers
{
    public class AllowAccountProjectionsHandler
    {
        public IEmployerPaymentsRepository Repository { get; }
        public ILog Logger { get; }
        public IApplicationConfiguration ApplicationConfiguration { get; }

        public AllowAccountProjectionsHandler(
            IEmployerPaymentsRepository repository, 
			ILog logger, 
			IApplicationConfiguration applicationConfiguration)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ApplicationConfiguration = applicationConfiguration ?? throw new ArgumentNullException(nameof(applicationConfiguration));
        }

        public async Task<bool> Allow(PaymentCreatedMessage paymentCreatedMessage)
        {
            Logger.Debug($"Now checking if projections can be generated for payment events: {paymentCreatedMessage.EmployerAccountId}, {paymentCreatedMessage.Id}");
            if (!ApplicationConfiguration.AllowTriggerProjections)
            {
                Logger.Warn("Triggering of projections is disabled.");
                return false;
            }
            var payments = Repository.Get(paymentCreatedMessage.EmployerAccountId);
	        var lastReceivedTime = await payments.GetLastTimeReceivedPayment();
			if (lastReceivedTime == null)
                throw new InvalidOperationException($"No last time recorded for employer account: {paymentCreatedMessage.EmployerAccountId}");

			var allowProjections = lastReceivedTime.Value.AddSeconds(ApplicationConfiguration.SecondsToWaitToAllowProjections) <= DateTime.Now;
            Logger.Info($"Allow projections '{allowProjections}' for employer '{paymentCreatedMessage.EmployerAccountId}' in response to payment event.");
			return allowProjections;
        }
    }
}