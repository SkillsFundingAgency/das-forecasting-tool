using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Infrastructure.Persistence;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Projections.Services;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Payments;
using SFA.DAS.Forecasting.Models.Projections;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Payments.Handlers
{
    public class AllowAccountProjectionsHandler
    {
        public IEmployerPaymentsRepository Repository { get; }
        public IAppInsightsTelemetry Logger { get; }
        public IApplicationConfiguration ApplicationConfiguration { get; }
        public IEmployerProjectionAuditService AuditService { get; }

        public AllowAccountProjectionsHandler(
            IEmployerPaymentsRepository repository, 
			IAppInsightsTelemetry logger, 
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
            Logger.Debug("PaymentEventAllowProjectionFunction", $"Now checking if projections can be generated for payment events: {paymentCreatedMessage.EmployerAccountId}, {paymentCreatedMessage.Id}", "Allow");
            if (!ApplicationConfiguration.AllowTriggerProjections)
            {
	            Logger.Warning("PaymentEventAllowProjectionFunction", "Triggering of projections is disabled.", "Allow");

                return false;
            }

            if (!await AuditService.RecordRunOfProjections(paymentCreatedMessage.EmployerAccountId))
            {
	            Logger.Debug("PaymentEventAllowProjectionFunction", $"Triggering of projections for employer {paymentCreatedMessage.EmployerAccountId} has already been started.", "Allow");

                return false;
            }
            
            var payments = Repository.Get(paymentCreatedMessage.EmployerAccountId);
	        var lastReceivedTime = await payments.GetLastTimeReceivedPayment();
	        if (lastReceivedTime == null)
	        {
		        Logger.Error("PaymentEventAllowProjectionFunction", new InvalidOperationException($"No last time recorded for employer account: {paymentCreatedMessage.EmployerAccountId}"), $"No last time recorded for employer account: {paymentCreatedMessage.EmployerAccountId}", "Allow");

				throw new InvalidOperationException(
			        $"No last time recorded for employer account: {paymentCreatedMessage.EmployerAccountId}");
	        }
	        var allowProjections = lastReceivedTime.Value.AddSeconds(ApplicationConfiguration.SecondsToWaitToAllowProjections) <= DateTime.UtcNow;
	        Logger.Info("PaymentEventAllowProjectionFunction", $"Allow projections '{allowProjections}' for employer '{paymentCreatedMessage.EmployerAccountId}' in response to payment event.", "Allow");

			return allowProjections;
        }
    }
}