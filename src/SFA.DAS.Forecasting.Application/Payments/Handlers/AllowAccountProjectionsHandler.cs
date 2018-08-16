using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Infrastructure.Persistence;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Projections.Services;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Payments;
using SFA.DAS.Forecasting.Models.Projections;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Application.Payments.Handlers
{
    public class AllowAccountProjectionsHandler
    {
        public IEmployerPaymentsRepository Repository { get; }
        public ILog Logger { get; }
        public IApplicationConfiguration ApplicationConfiguration { get; }
        public IEmployerProjectionAuditService AuditService { get; }

        public AllowAccountProjectionsHandler(
            IEmployerPaymentsRepository repository, 
			ILog logger, 
			IApplicationConfiguration applicationConfiguration,
            IEmployerProjectionAuditService auditService)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ApplicationConfiguration = applicationConfiguration ?? throw new ArgumentNullException(nameof(applicationConfiguration));
            AuditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
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


            var allowProjections = lastReceivedTime.Value.AddSeconds(ApplicationConfiguration.SecondsToWaitToAllowProjections) <= DateTime.UtcNow;
            if (!allowProjections)
            {
                Logger.Debug($"Cannot allow projections for employer {paymentCreatedMessage.EmployerAccountId}. Not enough time has elapsed since last payment received.");
                return false;
            }
            Logger.Debug($"Enough time has elapsed since last received payment to allow projections to be generated for employer {paymentCreatedMessage.EmployerAccountId}.");

            if (!await AuditService.RecordRunOfProjections(paymentCreatedMessage.EmployerAccountId, nameof(ProjectionSource.PaymentPeriodEnd)))
            {
                Logger.Debug($"Triggering of payment projections for employer {paymentCreatedMessage.EmployerAccountId} has already been started.");
                return false;
            }

            return true;
        }
    }
}