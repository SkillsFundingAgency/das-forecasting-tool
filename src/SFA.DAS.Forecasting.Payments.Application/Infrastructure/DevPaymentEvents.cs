using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Payments.Domain.Interfaces;
using SFA.DAS.Forecasting.Payments.Messages.Events;

namespace SFA.DAS.Forecasting.Payments.Application.Infrastructure
{
    public class DevPaymentEvents : IPaymentEvents
    {
        public async Task<IEnumerable<PaymentMessage>> ReadAsync()
        {
            var payments = new List<PaymentMessage> {
                new PaymentMessage { Id = Guid.NewGuid().ToString("N"), EmployerAccountId = "12345"},
                new PaymentMessage
                {
                    Id = Guid.NewGuid().ToString("N"),
                    EmployerAccountId = "6789",
                    Ukprn = 1234,
                    ApprenticeshipId = 1,
                    Uln = 345,
                    StandardCode = 1,
                    Amount = 186
                }
            };

            return await Task.FromResult(payments);
        }
    }
}