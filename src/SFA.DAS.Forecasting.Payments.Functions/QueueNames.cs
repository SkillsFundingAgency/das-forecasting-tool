namespace SFA.DAS.Forecasting.Payments.Functions
{
    internal class QueueNames
    {
	    public const string PaymentValidator = "forecasting-payment-validate-payment";
	    public const string PaymentProcessor = "forecasting-payment-process-payment";
	    public const string CommitmentProcessor = "forecasting-payment-process-commitment";
        public const string AddEarningDetails = "forecasting-payment-add-earning-details";
        public const string PreLoadPayment = "forecasting-payment-create-preload";
        public const string PreLoadEarningDetailsPayment = "forecasting-payment-load-earning-details";
	    public const string AllowProjection = "forecasting-payment-allow-projection";
	    public const string GeneratePaymentProjection = "forecasting-projections-generate-payment-projections";
    }
}
