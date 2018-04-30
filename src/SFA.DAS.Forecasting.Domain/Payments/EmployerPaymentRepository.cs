using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Payments.Services;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Domain.Payments
{
    public interface IEmployerPaymentRepository
    {
        Task<EmployerPayment> Get(long employerAccountId, string paymentId);
        Task StorePayment(EmployerPayment payment);
    }

    public class EmployerPaymentRepository : IEmployerPaymentRepository
	{
	    private readonly IEmployerPaymentDataSession _dataSession;

	    public EmployerPaymentRepository(IEmployerPaymentDataSession dataSession)
	    {
	        _dataSession = dataSession ?? throw new ArgumentNullException(nameof(dataSession));
	    }

	    public async Task<EmployerPayment> Get(long employerAccountId, string paymentId)
	    {
	        var payment = await _dataSession.Get(employerAccountId,paymentId) ?? new PaymentModel
	        {
	            EmployerAccountId = employerAccountId,
                ExternalPaymentId = paymentId
	        };
            return new EmployerPayment(payment);
	    }

	    public async Task StorePayment(EmployerPayment payment)
	    {
	        _dataSession.Store(payment.Model);
	        await _dataSession.SaveChanges();
	    }
	}
}
