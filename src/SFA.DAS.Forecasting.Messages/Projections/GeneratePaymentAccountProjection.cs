namespace SFA.DAS.Forecasting.Messages.Projections
{
    public class GeneratePaymentAccountProjection : GenerateAccountProjection
	{
	    public int Month { get; set; }

		public int Year { get; set; }
    }
}