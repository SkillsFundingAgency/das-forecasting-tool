using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Domain.Payments.Entities;
using SFA.DAS.Forecasting.Domain.Payments.Services;

namespace SFA.DAS.Forecasting.Domain.Payments.Repositories
{
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
