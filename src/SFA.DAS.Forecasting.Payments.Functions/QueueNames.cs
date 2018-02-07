namespace SFA.DAS.Forecasting.Payments.Functions
{
    internal class QueueNames
    {
	    public const string PaymentValidator = "forecasting-payment-validate-payment";
	    public const string PaymentProcessor = "forecasting-payment-process-payment";
	    public const string PaymentAggregationAllower = "forecasting-payment-allow-aggregate-payments";
		public const string PaymentAggregation = "forecasting-payment-get-aggregations";
	    public const string PaymentAggregationProcessor = "forecasting-payment-process-aggregation";
	}
}
