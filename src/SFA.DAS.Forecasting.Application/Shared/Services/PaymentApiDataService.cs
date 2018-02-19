using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Provider.Events.Api.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Application.Shared.Services
{
    public class PaymentApiDataService
    {
        private readonly IPaymentsEventsApiClient _paymentsEventsApiClient;

        public PaymentApiDataService(IPaymentsEventsApiClient paymentsEventsApiClient)
        {
            _paymentsEventsApiClient = paymentsEventsApiClient;
        }

        public async Task<IEnumerable<EarningDetails>> PaymentForPeriod(string periodId, string employerAccountId)
        {
            List<EarningDetails> result = new List<EarningDetails>();
            var maxPages = 10000;
            for (int i = 1; i < maxPages; i++)
            {
                var page = await _paymentsEventsApiClient.GetPayments(periodId, employerAccountId, page: i);
                var paymentEarningDetails = page.Items
                    .Select(m => m.EarningDetails);

                foreach (var item in paymentEarningDetails)
                {
                    var earningDetails = item.Select(m => 
                        new EarningDetails
                        {
                            ActualEndDate = m.ActualEndDate,
                            PlannedEndDate = m.PlannedEndDate,
                            EndpointAssessorId = m.EndpointAssessorId,
                            CompletionAmount = m.CompletionAmount,
                            CompletionStatus = m.CompletionStatus,
                            MonthlyInstallment = m.MonthlyInstallment,
                            RequiredPaymentId = m.RequiredPaymentId,
                            StartDate = m.StartDate,
                            TotalInstallments = m.TotalInstallments

                        });

                    result.AddRange(earningDetails);
                }
                if (page.PageNumber == page.PageNumber)
                    break;
            }
            return result;
        }
    }
}