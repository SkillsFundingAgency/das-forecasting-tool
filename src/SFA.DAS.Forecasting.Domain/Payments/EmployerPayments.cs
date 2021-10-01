using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Payments.Services;

namespace SFA.DAS.Forecasting.Domain.Payments
{
    public class EmployerPayments
    {
        private readonly long _employerAccountId;
        private readonly IEmployerPaymentDataSession _dataSession;

        public EmployerPayments(long employerAccountId, IEmployerPaymentDataSession dataSession)
        {
            if (employerAccountId <= 0) throw new ArgumentOutOfRangeException(nameof(employerAccountId));
            _employerAccountId = employerAccountId;
            _dataSession = dataSession ?? throw new ArgumentNullException(nameof(dataSession));
        }

		public async Task<bool?> HasReceivedRecentPayment()
		{
			return await _dataSession.HasReceivedRecentPayment(_employerAccountId);
		}

	    public async Task<bool?> HasReceivedRecentPaymentForSendingEmployer()
	    {
		    return await _dataSession.HasReceivedRecentPaymentForSendingEmployer(_employerAccountId);
	    }
	}
}