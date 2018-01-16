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
            var apprenticeshipId = new Random(Guid.NewGuid().GetHashCode()).Next(1, 5);
            var payments = new List<PaymentEvent> {
                new PaymentEvent
                {
                    Id = Guid.NewGuid().ToString("N"),
                    EmployerAccountId = "12345",
                    Ukprn = 1234,
                    ApprenticeshipId = apprenticeshipId,
                    Uln = apprenticeshipId*10,
                    StandardCode = 1,
                    Amount =  80,
                    EarningDetails = new EarningDetails
                    {
                        RequiredPaymentId = Guid.NewGuid(),
                        ActualEndDate = new DateTime(0001,1,1,0,0,0),
                        PlannedEndDate = DateTime.Today.AddMonths(10),
                        CompletionAmount = 240,
                        MonthlyInstallment = 80,
                        StartDate = DateTime.Today.AddMonths(-2),
                        TotalInstallments = 12
                    },
                    CollectionPeriod = new CollectionPeriod
                    {
                        Id = "17/18-R01",
                        Month = DateTime.Today.Month,
                        Year = DateTime.Today.Year
                    }
                }
            };

            return await Task.FromResult(payments);
        }
    }
}