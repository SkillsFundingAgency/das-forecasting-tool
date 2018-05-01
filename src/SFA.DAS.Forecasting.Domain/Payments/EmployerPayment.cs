using System;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Domain.Payments
{
    public class EmployerPayment
    {
        internal PaymentModel Model { get; private set; }
        public long Id => Model.Id;
        public string ExternalPaymentId => Model.ExternalPaymentId;
        public long EmployerAccountId => Model.EmployerAccountId;
        public long? SendingEmployerAccountId => Model.SendingEmployerAccountId;
        public long ProviderId => Model.ProviderId;
        public long ApprenticeshipId => Model.ApprenticeshipId;
        public decimal Amount => Model.Amount;
        public DateTime ReceivedTime => Model.ReceivedTime;
        public long LearnerId => Model.LearnerId;
        public CalendarPeriod CollectionPeriod => Model.CollectionPeriod;
        public CalendarPeriod DeliveryPeriod => Model.DeliveryPeriod;
        public FundingSource FundingSource => Model.FundingSource;

        public EmployerPayment(PaymentModel model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public void RegisterPayment(PaymentModel payment)
        {
            if (payment.ExternalPaymentId != ExternalPaymentId)
                throw new InvalidOperationException($"Invalid payment id.  Does not match original payment id. Id: {payment.ExternalPaymentId}, Current id: {ExternalPaymentId}");
            Model.Amount = payment.Amount;
            Model.CollectionPeriod = payment.CollectionPeriod;
            Model.DeliveryPeriod = payment.DeliveryPeriod;
            Model.FundingSource = payment.FundingSource;
            Model.LearnerId = payment.LearnerId;
            Model.ReceivedTime = payment.ReceivedTime;
            Model.ApprenticeshipId = payment.ApprenticeshipId;
            Model.ProviderId = payment.ProviderId;
            Model.SendingEmployerAccountId = payment.SendingEmployerAccountId;
        }
	}
}