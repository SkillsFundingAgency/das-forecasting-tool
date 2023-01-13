using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Provider.Events.Api.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Forecasting.Application.Shared.Services
{
    public interface IPaymentApiDataService
    {
        Task<List<EarningDetails>> PaymentForPeriod(string periodId, long employerAccountId);
    }
    public class PaymentApiDataService : IPaymentApiDataService
    {
        private readonly IPaymentsEventsApiClient _paymentsEventsApiClient;
        private readonly ILogger<PaymentApiDataService> _logger;

        public PaymentApiDataService(IPaymentsEventsApiClient paymentsEventsApiClient, ILogger<PaymentApiDataService> logger)
        {
            _paymentsEventsApiClient = paymentsEventsApiClient;
            _logger = logger;
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
                    _logger.LogInformation($"No payments returned for page {i}.");
                    break;
                }

                _logger.LogDebug($"Got {page.Items.Length} payments for page {i}.");
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
                _logger.LogDebug($"Got {paymentEarningDetails.Count} payments for page {i}.");
                result.AddRange(paymentEarningDetails);
                if (page.PageNumber == page.TotalNumberOfPages)
                    break;
            }
            return result;
        }
    }
}