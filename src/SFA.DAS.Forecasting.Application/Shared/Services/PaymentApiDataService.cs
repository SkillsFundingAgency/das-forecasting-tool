using System;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Provider.Events.Api.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Shared.Services
{
    public class PaymentApiDataService
    {
        private readonly IPaymentsEventsApiClient _paymentsEventsApiClient;
        private readonly IAppInsightsTelemetry _logger;

        public PaymentApiDataService(IPaymentsEventsApiClient paymentsEventsApiClient, IAppInsightsTelemetry logger)
        {
            _paymentsEventsApiClient = paymentsEventsApiClient;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<EarningDetails>> PaymentForPeriod(string periodId, long employerAccountId)
        {
            var result = new List<EarningDetails>();
            var maxPages = 10000;
            for (int i = 1; i < maxPages; i++)
            {
                var page = await _paymentsEventsApiClient.GetPayments(periodId, employerAccountId.ToString(), i);
                if (!page.Items.Any())
                {
	                _logger.Info("GetEarningDetailsFunction", $"No payments returned for page {i}.", "PaymentForPeriod");

                    break;
                }

	            _logger.Debug("GetEarningDetailsFunction", $"Got {page.Items.Length} payments for page {i}.", "PaymentForPeriod");

                var paymentEarningDetails = page.Items
                    .Where(p => p.EarningDetails.Any())
                    .SelectMany(p => p.EarningDetails, (p, e) =>
                        new EarningDetails
                        {
                            ActualEndDate = e.ActualEndDate,
                            PlannedEndDate = e.PlannedEndDate,
                            EndpointAssessorId = e.EndpointAssessorId,
                            CompletionAmount = e.CompletionAmount,
                            CompletionStatus = e.CompletionStatus,
                            MonthlyInstallment = e.MonthlyInstallment,
                            RequiredPaymentId = e.RequiredPaymentId,
                            ApprenticeshipId = p.ApprenticeshipId ?? 0,
                            PaymentId = p.Id,
                            StartDate = e.StartDate,
                            TotalInstallments = e.TotalInstallments
                        })
                    .ToList();
	            _logger.Debug("GetEarningDetailsFunction", $"Got {paymentEarningDetails.Count} payments for page {i}.", "PaymentForPeriod");

                result.AddRange(paymentEarningDetails);
                if (page.PageNumber == page.TotalNumberOfPages)
                    break;
            }
            return result;
        }
    }
}