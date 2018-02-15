namespace SFA.DAS.Forecasting.Messages.Projections
{
	public class GenerateLevyAccountProjection : GenerateAccountProjection
	{
		public string PayrollYear { get; set; }

		public short PayrollMonth { get; set; }
	}
}