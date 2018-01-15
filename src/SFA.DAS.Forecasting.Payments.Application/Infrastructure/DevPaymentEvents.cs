using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Payments.Domain.Interfaces;
using SFA.DAS.Forecasting.Payments.Messages.Events;

namespace SFA.DAS.Forecasting.Payments.Application.Infrastructure
{
    public class DevPaymentEvents : IPaymentEvents
    {
        public async Task<IEnumerable<PaymentEvent>> ReadAsync()
        {
            var payments = new List<PaymentEvent> {
                new PaymentEvent
                {
                    Id = Guid.NewGuid().ToString("N"),
                    EmployerAccountId = "12345",
                    Ukprn = 1234,
                    ApprenticeshipId = 1,
                    Uln = 345,
                    StandardCode = 1,
                    Amount =  new Random(Guid.NewGuid().GetHashCode()).Next(100,500)
                }
            };

            return await Task.FromResult(payments);
        }
    }
}