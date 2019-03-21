namespace SFA.DAS.Forecasting.Payments.Functions
{
    internal class QueueNames
    {
	    public const string PaymentValidator = "forecasting-payment-validate-payment";
	    public const string PaymentValidatorNoCommitment = "forecasting-payment-validate-payment-no-commitment";
		public const string PaymentProcessor = "forecasting-payment-process-payment";
	    public const string PaymentNoCommitmentProcessor = "forecasting-payment-process-payment-no-commitment";
		public const string CommitmentProcessor = "forecasting-payment-process-commitment";
	    public const string AllowProjection = "forecasting-payment-allow-projection";
	    public const string GenerateProjections = "forecasting-projections-generate-projections";
        public const string AllowPaymentAggregation = "forecasting-projections-allow-aggregation";
        public const string AggregatePaymentData = "forecasting-projections-generate-aggregation";
    }
}
