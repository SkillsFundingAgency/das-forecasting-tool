using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Projections.Services;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Payments;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Application.Payments.Handlers
{
    public interface IAllowAccountProjectionsHandler
    {
        Task<IEnumerable<long>> AllowedEmployerAccountIds(PaymentCreatedMessage paymentCreatedMessage);

    }
    public class AllowAccountProjectionsHandler : IAllowAccountProjectionsHandler
    {
        private readonly IEmployerPaymentsRepository _paymentsRepository;
        private readonly IEmployerCommitmentsRepository _commitmentsRepository;
        private readonly ILogger<AllowAccountProjectionsHandler> _logger;
        private readonly ForecastingJobsConfiguration _applicationConfiguration;
        private readonly IEmployerProjectionAuditService _auditService;

        public AllowAccountProjectionsHandler(
            IEmployerPaymentsRepository repository,
            ILogger<AllowAccountProjectionsHandler> logger,
            ForecastingJobsConfiguration applicationConfiguration,
            IEmployerProjectionAuditService auditService,
            IEmployerCommitmentsRepository commitmentsRepository)
        {
            _paymentsRepository = repository;
            _logger = logger;
            _applicationConfiguration = applicationConfiguration;
            _auditService = auditService;
            _commitmentsRepository = commitmentsRepository;
        }

        public async Task<IEnumerable<long>> AllowedEmployerAccountIds(PaymentCreatedMessage paymentCreatedMessage)
        {
            _logger.LogInformation($"Now checking if projections can be generated for payment events: {paymentCreatedMessage.EmployerAccountId}, {paymentCreatedMessage.Id}");
            if (!_applicationConfiguration.AllowTriggerProjections)
            {
                _logger.LogWarning("Triggering of projections is disabled.");
                return new List<long>();
            }

            var employerAccountIds = new List<long>();

            if (await IsEmployerAccountIdAllowed(paymentCreatedMessage.EmployerAccountId))
            {
                _logger.LogInformation($"Enough time has elapsed since last received payment and commitment to allow projections to be generated for employer {paymentCreatedMessage.EmployerAccountId}.");
                if (!await _auditService.RecordRunOfProjections(paymentCreatedMessage.EmployerAccountId, nameof(ProjectionSource.PaymentPeriodEnd)))
                {
                    _logger.LogInformation($"Triggering of payment projections for employer {paymentCreatedMessage.EmployerAccountId} has already been started.");
                }
                else
                {
                    employerAccountIds.Add(paymentCreatedMessage.EmployerAccountId);
                }
            }
            else
            {
                _logger.LogInformation($"Cannot allow projections for employer {paymentCreatedMessage.EmployerAccountId}. Not enough time has elapsed since last payment or commitment received.");
            }

            if (paymentCreatedMessage.SendingEmployerAccountId != 0 && paymentCreatedMessage.EmployerAccountId != paymentCreatedMessage.SendingEmployerAccountId)
            {
                if (await IsSendingEmployerAccountIdAllowed(paymentCreatedMessage.SendingEmployerAccountId))
                {
                    employerAccountIds.Add(paymentCreatedMessage.SendingEmployerAccountId);
                    if (!await _auditService.RecordRunOfProjections(paymentCreatedMessage.SendingEmployerAccountId, nameof(ProjectionSource.PaymentPeriodEnd)))
                    {
                        _logger.LogInformation($"Triggering of payment projections for employer {paymentCreatedMessage.SendingEmployerAccountId} has already been started.");
                    }
                    else
                    {
                        employerAccountIds.Add(paymentCreatedMessage.SendingEmployerAccountId);
                    }
                }
                else
                {
                    _logger.LogInformation($"Cannot allow projections for employer {paymentCreatedMessage.SendingEmployerAccountId}. Not enough time has elapsed since last payment or commitment received.");
                }
            }
            

            return employerAccountIds;
        }

        private async Task<bool> IsEmployerAccountIdAllowed(long employerAccountId)
        {
            var payments = _paymentsRepository.Get(employerAccountId);
            var hasReceivedRecentPayment = await payments.HasReceivedRecentPayment();
            var lastCommitmentReceivedTime = await _commitmentsRepository.GetLastTimeRecieved(employerAccountId);

            if (!hasReceivedRecentPayment)
            {
                _logger.LogInformation($"No last payment received recorded for employer account: {employerAccountId}  5 minutes has elapsed since last received payment");
            }

            if (lastCommitmentReceivedTime == null)
            {
                throw new InvalidOperationException($"No last commitment time recorded for employer account: {employerAccountId}");
            }

            return AllowProjections(hasReceivedRecentPayment, lastCommitmentReceivedTime);
        }

        private async Task<bool> IsSendingEmployerAccountIdAllowed(long sendingEmployerAccountId)
        {
            var payments = _paymentsRepository.Get(sendingEmployerAccountId);
            var hasReceivedRecentPaymentForSendingEmployer = await payments.HasReceivedRecentPaymentForSendingEmployer();
            if (!hasReceivedRecentPaymentForSendingEmployer)
            {
                _logger.LogInformation($"No last payment received recorded for sending employer account: {sendingEmployerAccountId}  5 minutes has elapsed since last received payment");
            }                

            var lastCommitmentReceivedTime = await _commitmentsRepository.GetLastTimeRecieved(sendingEmployerAccountId);
            if (lastCommitmentReceivedTime == null)
                throw new InvalidOperationException($"No last commitment time recorded for employer account: {sendingEmployerAccountId}");

            return AllowProjections(hasReceivedRecentPaymentForSendingEmployer, lastCommitmentReceivedTime);
        }
        private bool AllowProjections(bool hasReceivedRecentPayment, DateTime? lastCommitmentReceivedTime)
        {
            var allowPaymentProjections = !hasReceivedRecentPayment;
            
            var allowCommitmentProjections =
                lastCommitmentReceivedTime.Value.AddSeconds(_applicationConfiguration.SecondsToWaitToAllowProjections) <=
                DateTime.UtcNow;

            return allowPaymentProjections && allowCommitmentProjections;
        }
    }

}