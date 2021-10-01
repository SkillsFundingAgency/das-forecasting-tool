using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Models.Payments;
using CalendarPeriod = SFA.DAS.EmployerFinance.Types.Models.CalendarPeriod;

namespace SFA.DAS.Forecasting.Domain.Payments.Services
{
    public interface IEmployerPaymentDataSession
    {
        Task<PaymentModel> Get(long employerAccountId, string paymentId);
        void Store(PaymentModel payment);
        Task<bool?> HasReceivedRecentPayment(long employerAccountId);
	    Task<bool?> HasReceivedRecentPaymentForSendingEmployer(long sendingEmployerAccountId);
        Task<Dictionary<CalendarPeriod, decimal>> GetPaymentTotals(long employerAccountId);
		Task SaveChanges();
    }
}