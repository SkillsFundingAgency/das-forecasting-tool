using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<long>> AllowedEmployerAccountIds(PaymentCreatedMessage paymentCreatedMessage)
	    {
		    Logger.Debug($"Now checking if projections can be generated for payment events: {paymentCreatedMessage.EmployerAccountId}, {paymentCreatedMessage.Id}");
		    if (!ApplicationConfiguration.AllowTriggerProjections)
		    {
			    Logger.Warn("Triggering of projections is disabled.");
			    return new List<long>();
		    }

		    var employerAccountIds = new List<long>();

		    if (await IsEmployerAccountIdAllowed(paymentCreatedMessage.EmployerAccountId))
		    {
			    employerAccountIds.Add(paymentCreatedMessage.EmployerAccountId);
		    }
			
		    if (paymentCreatedMessage.EmployerAccountId != paymentCreatedMessage.SendingEmployerAccountId)
		    {
			    if (await IsSendingEmployerAccountIdAllowed(paymentCreatedMessage.SendingEmployerAccountId))
			    {
					employerAccountIds.Add(paymentCreatedMessage.SendingEmployerAccountId);
				}
			}

			return employerAccountIds;
	    }

		private async Task<bool> IsEmployerAccountIdAllowed(long employerAccountId)
		{
			var payments = Repository.Get(employerAccountId);
			var lastReceivedTime = await payments.GetLastTimeReceivedPayment();
			if (lastReceivedTime == null)
				throw new InvalidOperationException($"No last time recorded for employer account: {employerAccountId}");

			return await CheckIfTimeAllowed(lastReceivedTime, employerAccountId);
		}

	    private async Task<bool> IsSendingEmployerAccountIdAllowed(long sendingEmployerAccountId)
	    {
		    var payments = Repository.Get(sendingEmployerAccountId);
		    var lastReceivedTime = await payments.GetLastTimeSentPayment();
		    if (lastReceivedTime == null)
			    throw new InvalidOperationException($"No last time recorded for employer account: {sendingEmployerAccountId}");

		    return await CheckIfTimeAllowed(lastReceivedTime, sendingEmployerAccountId);
		}

		private async Task<bool> CheckIfTimeAllowed(DateTime? lastReceivedTime, long employerAccountId)
	    {
			var allowProjections = lastReceivedTime.Value.AddSeconds(ApplicationConfiguration.SecondsToWaitToAllowProjections) <= DateTime.UtcNow;
		    if (!allowProjections)
		    {
			    Logger.Debug($"Cannot allow projections for employer {employerAccountId}. Not enough time has elapsed since last payment received.");
			    return false;
		    }
		    Logger.Debug($"Enough time has elapsed since last received payment to allow projections to be generated for employer {employerAccountId}.");

		    if (!await AuditService.RecordRunOfProjections(employerAccountId, nameof(ProjectionSource.PaymentPeriodEnd)))
		    {
			    Logger.Debug($"Triggering of payment projections for employer {employerAccountId} has already been started.");
			    return false;
		    }

		    return true;
		}
	}
}