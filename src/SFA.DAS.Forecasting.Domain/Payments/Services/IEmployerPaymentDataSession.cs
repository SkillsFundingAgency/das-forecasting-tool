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
        Task<DateTime?> GetLastReceivedTime(long employerAccountId);
	    Task<DateTime?> GetLastSentTime(long sendingEmployerAccountId);
        Task<Dictionary<CalendarPeriod, decimal>> GetPaymentTotals(long employerAccountId);
		Task SaveChanges();
        Task CalculatePaymentTotals(long employerAccountId, int collectionPeriodYear, int collectionPeriodMonth);
    }
}