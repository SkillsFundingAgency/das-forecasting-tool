using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Projections.Services;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Payments;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Application.Payments.Handlers
{
    public class AllowAccountProjectionsHandler
    {
        private readonly IEmployerPaymentsRepository _paymentsRepository;
        private readonly IEmployerCommitmentsRepository _commitmentsRepository;
        private readonly ILog _logger;
        private readonly IApplicationConfiguration _applicationConfiguration;
        private readonly IEmployerProjectionAuditService _auditService;

        public AllowAccountProjectionsHandler(
            IEmployerPaymentsRepository repository,
            ILog logger,
            IApplicationConfiguration applicationConfiguration,
            IEmployerProjectionAuditService auditService,
            IEmployerCommitmentsRepository commitmentsRepository)
        {
            _paymentsRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _applicationConfiguration = applicationConfiguration ?? throw new ArgumentNullException(nameof(applicationConfiguration));
            _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
            _commitmentsRepository = commitmentsRepository ?? throw new ArgumentNullException(nameof(commitmentsRepository));
        }

        public async Task<IEnumerable<long>> AllowedEmployerAccountIds(PaymentCreatedMessage paymentCreatedMessage)
        {
            _logger.Info($"Now checking if projections can be generated for payment events: {paymentCreatedMessage.EmployerAccountId}, {paymentCreatedMessage.Id}");
            if (!_applicationConfiguration.AllowTriggerProjections)
            {
                _logger.Warn("Triggering of projections is disabled.");
                return new List<long>();
            }

            var employerAccountIds = new List<long>();

            if (await IsEmployerAccountIdAllowed(paymentCreatedMessage.EmployerAccountId))
            {
                _logger.Info($"Enough time has elapsed since last received payment and commitment to allow projections to be generated for employer {paymentCreatedMessage.EmployerAccountId}.");
                if (!await _auditService.RecordRunOfProjections(paymentCreatedMessage.EmployerAccountId, nameof(ProjectionSource.PaymentPeriodEnd)))
                {
                    _logger.Info($"Triggering of payment projections for employer {paymentCreatedMessage.EmployerAccountId} has already been started.");
                }
                else
                {
                    employerAccountIds.Add(paymentCreatedMessage.EmployerAccountId);
                }
            }
            else
            {
                _logger.Info($"Cannot allow projections for employer {paymentCreatedMessage.EmployerAccountId}. Not enough time has elapsed since last payment or commitment received.");
            }

            if (paymentCreatedMessage.SendingEmployerAccountId != 0 && paymentCreatedMessage.EmployerAccountId != paymentCreatedMessage.SendingEmployerAccountId)
            {
                if (await IsSendingEmployerAccountIdAllowed(paymentCreatedMessage.SendingEmployerAccountId))
                {
                    employerAccountIds.Add(paymentCreatedMessage.SendingEmployerAccountId);
                    if (!await _auditService.RecordRunOfProjections(paymentCreatedMessage.SendingEmployerAccountId, nameof(ProjectionSource.PaymentPeriodEnd)))
                    {
                        _logger.Info($"Triggering of payment projections for employer {paymentCreatedMessage.SendingEmployerAccountId} has already been started.");
                    }
                    else
                    {
                        employerAccountIds.Add(paymentCreatedMessage.SendingEmployerAccountId);
                    }
                }
                else
                {
                    _logger.Info($"Cannot allow projections for employer {paymentCreatedMessage.SendingEmployerAccountId}. Not enough time has elapsed since last payment or commitment received.");
                }
            }
            

            return employerAccountIds;
        }

        private async Task<bool> IsEmployerAccountIdAllowed(long employerAccountId)
        {
            var payments = _paymentsRepository.Get(employerAccountId);
            var lastPaymentReceivedTime = await payments.GetLastTimeReceivedPayment();
            if (lastPaymentReceivedTime == null)
            {
                _logger.Info($"No last payment received time recorded for employer account: {employerAccountId}  5 minutes has elapsed since last received payment");                
            }

            var lastCommitmentReceivedTime = await _commitmentsRepository.GetLastTimeRecieved(employerAccountId);
            if (lastCommitmentReceivedTime == null)
                throw new InvalidOperationException($"No last commitment time recorded for employer account: {employerAccountId}");

            return AllowProjections(lastPaymentReceivedTime, lastCommitmentReceivedTime);
        }

        private async Task<bool> IsSendingEmployerAccountIdAllowed(long sendingEmployerAccountId)
        {
            var payments = _paymentsRepository.Get(sendingEmployerAccountId);
            var lastReceivedTime = await payments.GetLastTimeSentPayment();
            if (lastReceivedTime == null)
                throw new InvalidOperationException($"No last time recorded for employer account: {sendingEmployerAccountId}");

            var lastCommitmentReceivedTime = await _commitmentsRepository.GetLastTimeRecieved(sendingEmployerAccountId);
            if (lastCommitmentReceivedTime == null)
                throw new InvalidOperationException($"No last commitment time recorded for employer account: {sendingEmployerAccountId}");

            return AllowProjections(lastReceivedTime, lastCommitmentReceivedTime);
        }
        private bool AllowProjections(DateTime? lastPaymentReceivedTime, DateTime? lastCommitmentReceivedTime)
        {
            var allowPaymentProjections = lastPaymentReceivedTime == null;
            
            var allowCommitmentProjections =
                lastCommitmentReceivedTime.Value.AddSeconds(_applicationConfiguration.SecondsToWaitToAllowProjections) <=
                DateTime.UtcNow;

            return allowPaymentProjections && allowCommitmentProjections;
        }
    }

}