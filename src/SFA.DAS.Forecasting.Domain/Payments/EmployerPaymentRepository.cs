using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Payments.Services;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Domain.Payments
{
    public interface IEmployerPaymentRepository
    {
        Task StorePayment(Payment payment);

        Task<List<Payment>> GetPayments(long employerAccountId, int month, int year);
    }

    public class EmployerPaymentRepository : IEmployerPaymentRepository
	{
		public IEmployerPaymentDataService EmployerPaymentDataService { get; set; }


		public EmployerPaymentRepository(IEmployerPaymentDataService employerPaymentDataService)
		{
			EmployerPaymentDataService = employerPaymentDataService ?? throw new ArgumentNullException(nameof(employerPaymentDataService));
		}


		public async Task StorePayment(Payment payment)
		{
			await EmployerPaymentDataService.StoreEmployerPayment(payment);
		}

		public async Task<List<Payment>> GetPayments(long employerAccountId, int month, int year)
		{
			return await EmployerPaymentDataService.GetEmployerPayments(employerAccountId, month, year);
		}
	}
}
