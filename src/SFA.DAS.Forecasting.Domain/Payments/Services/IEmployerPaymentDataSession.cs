using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Domain.Payments.Services
{
    public interface IEmployerPaymentDataSession
    {
        Task<PaymentModel> Get(long employerAccountId, string paymentId);
        void Store(PaymentModel payment);
        Task<DateTime?> GetLastReceivedTime(long employerAccountId);
	    Task<DateTime?> GetLastSentTime(long sendingEmployerAccountId);

		Task SaveChanges();
    }
}