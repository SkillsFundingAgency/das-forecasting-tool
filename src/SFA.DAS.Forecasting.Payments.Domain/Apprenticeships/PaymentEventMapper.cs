using System;
using SFA.DAS.Forecasting.Payments.Domain.Entities;
using SFA.DAS.Forecasting.Payments.Messages.Events;

namespace SFA.DAS.Forecasting.Payments.Domain.Apprenticeships
{
    public interface IPaymentEventMapper
    {
        PaymentApprenticeship Map(PaymentEvent payment);
    }

    public class PaymentEventMapper : IPaymentEventMapper
    {
        public PaymentApprenticeship Map(PaymentEvent payment)
        {
            return new PaymentApprenticeship
            {
                EmployerAccountId = long.Parse(payment.EmployerAccountId),
                Name = "Gustav",
                Uln = payment.Uln,
                DateOfBirth = new DateTime(1998, 12, 08),
                TrainingName = "Code manager",
                TrainingLevel = 2,
                TrainingProviderName = "Provider name",
                StartDate = payment.EarningDetails.StartDate,
                MonthlyPayment = payment.EarningDetails.MonthlyInstallment,
                Instalments = payment.EarningDetails.TotalInstallments,
                CompletionPayment = payment.EarningDetails.CompletionAmount
            };
        }
    }
}