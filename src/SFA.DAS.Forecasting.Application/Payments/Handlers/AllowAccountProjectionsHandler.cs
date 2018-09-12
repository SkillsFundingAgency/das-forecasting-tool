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
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Payments;
using SFA.DAS.Forecasting.Models.Projections;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Application.Payments.Handlers
{
    public class AllowAccountProjectionsHandler
    {
        private readonly IEmployerPaymentsRepository _paymentsRepository;
        private readonly EmployerCommitmentsRepository _commitmentsRepository;
        private readonly ILog _logger;
        private readonly IApplicationConfiguration _applicationConfiguration;
        private readonly IEmployerProjectionAuditService _auditService;

        public AllowAccountProjectionsHandler(
            IEmployerPaymentsRepository repository, 
			ILog logger, 
			IApplicationConfiguration applicationConfiguration,
            IEmployerProjectionAuditService auditService, 
            EmployerCommitmentsRepository commitmentsRepository)
        {
            _paymentsRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _applicationConfiguration = applicationConfiguration ?? throw new ArgumentNullException(nameof(applicationConfiguration));
            _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
            _commitmentsRepository = commitmentsRepository ?? throw new ArgumentNullException(nameof(commitmentsRepository));
        }

        public async Task<bool> Allow(PaymentCreatedMessage paymentCreatedMessage)
        {
            _logger.Debug($"Now checking if projections can be generated for payment events: {paymentCreatedMessage.EmployerAccountId}, {paymentCreatedMessage.Id}");
            if (!_applicationConfiguration.AllowTriggerProjections)
            {
                _logger.Warn("Triggering of projections is disabled.");
                return false;
            }

            var payments = _paymentsRepository.Get(paymentCreatedMessage.EmployerAccountId);
	        var lastPaymentReceivedTime = await payments.GetLastTimeReceivedPayment();
			if (lastPaymentReceivedTime == null)
                throw new InvalidOperationException($"No last payment time recorded for employer account: {paymentCreatedMessage.EmployerAccountId}");

            var lastCommitmentReceivedTime = await _commitmentsRepository.GetLastTimeRecieved(paymentCreatedMessage.EmployerAccountId);
            if (lastCommitmentReceivedTime == null)
                throw new InvalidOperationException($"No last commitment time recorded for employer account: {paymentCreatedMessage.EmployerAccountId}");

            var allowProjections = AllowProjections(lastPaymentReceivedTime, lastCommitmentReceivedTime);
            if (!allowProjections)
            {
                _logger.Debug($"Cannot allow projections for employer {paymentCreatedMessage.EmployerAccountId}. Not enough time has elapsed since last payment or commitment received.");
                return false;
            }
            _logger.Debug($"Enough time has elapsed since last received payment and commitment to allow projections to be generated for employer {paymentCreatedMessage.EmployerAccountId}.");

            if (!await _auditService.RecordRunOfProjections(paymentCreatedMessage.EmployerAccountId, nameof(ProjectionSource.PaymentPeriodEnd)))
            {
                _logger.Debug($"Triggering of payment projections for employer {paymentCreatedMessage.EmployerAccountId} has already been started.");
                return false;
            }

            return true;
        }

        private bool AllowProjections(DateTime? lastPaymentReceivedTime, DateTime? lastCommitmentReceivedTime)
        {
            var allowPaymentProjections =
                lastPaymentReceivedTime.Value.AddSeconds(_applicationConfiguration.SecondsToWaitToAllowProjections) <=
                DateTime.UtcNow;
           var  allowCommitmentProjections = 
               lastCommitmentReceivedTime.Value.AddSeconds(_applicationConfiguration.SecondsToWaitToAllowProjections) <=
                                             DateTime.UtcNow;

            return allowPaymentProjections && allowCommitmentProjections;
        }
    }
}