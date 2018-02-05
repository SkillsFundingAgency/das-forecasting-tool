namespace SFA.DAS.Forecasting.Domain.Payments.Entities
{
	public class EmployerTotalCostOfTraining
	{
		public long EmployerAccountId { get; set; }

		public decimal TotalCostOfTraining { get; set; }

		public int PeriodYear { get; set; }

		public int PeriodMonth { get; set; }
	}
}
