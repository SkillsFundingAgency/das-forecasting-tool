namespace SFA.DAS.Forecasting.Models.Payments
{
	public class EmployerTotalCostOfTraining
	{
		public long EmployerAccountId { get; set; }

		public decimal TotalCostOfTraining { get; set; }

		public int PeriodYear { get; set; }

		public int PeriodMonth { get; set; }
	}
}
