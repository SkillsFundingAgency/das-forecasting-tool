namespace SFA.DAS.Forecasting.Payments.Application.Messages
{
	/// <summary>
	/// TODO: Temp event definition. this will be replaced by the actual Collection period published by the employer services.
	/// </summary>
	public class CollectionPeriod
    {
        public string Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}