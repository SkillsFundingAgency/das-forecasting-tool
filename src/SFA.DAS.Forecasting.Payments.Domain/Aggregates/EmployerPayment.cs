using System.Threading.Tasks;
using SFA.DAS.Forecasting.Payments.Domain.Repositories;

namespace SFA.DAS.Forecasting.Payments.Domain.Aggregates
{
    public class EmployerPayment
    {
		private readonly IEmployerPaymentRepository _paymentStorage;

	    public EmployerPayment(IEmployerPaymentRepository paymentStorage)
	    {
		    _paymentStorage = paymentStorage;
	    }
	    public async Task AddDeclaration(string id, string employerAccountId, long ukprn, long apprenticeshipId, decimal amount)
	    {
		    var payment = new Entities.Payment
		    {
				Id = id,
			    EmployerAccountId = employerAccountId,
				Ukprn = ukprn,
				ApprenticeshipId = apprenticeshipId,
			    Amount = amount
		    };

		    await _paymentStorage.StorePayment(payment);
	    }
	}
}