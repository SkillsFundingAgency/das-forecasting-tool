namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public static class QueueNames
    {
        // Levy
        public const string ValidateLevyDeclaration = "forecasting-levy-validate-declaration";
		public const string ValidateLevyDeclarationNoProjection = "forecasting-levy-validate-declaration-no-projection";
		public const string LevyPreLoadRequest = "forecasting-levy-preload-request";
	    public const string LevyPreLoadRequestNoProjection = "forecasting-levy-preload-request-no-projection";

		// Payment
		public const string PreLoadPayment = "forecasting-payment-create-preload";
	    public const string PreLoadPaymentNoCommitment = "forecasting-payment-create-preload-no-commitment";
		public const string PreLoadEarningDetailsPayment = "forecasting-payment-load-earning-details";
	    public const string PreLoadEarningDetailsPaymentNoCommitment = "forecasting-payment-load-earning-details-no-commitment";
		public const string CreatePaymentMessage = "forecasting-payment-create-payment-message";
	    public const string CreatePaymentMessageNoCommitment = "forecasting-payment-create-payment-message-no-commitment";
		public const string RemovePreLoadData = "forecasting-payment-remove-preload-data";
	    public const string RemovePreLoadDataNoCommitment = "forecasting-payment-remove-preload-data-no-commitment";
		public const string PaymentValidator = "forecasting-payment-validate-payment";
        public const string PaymentValidatorNoCommitment = "forecasting-payment-validate-payment-no-commitment";

        //
        public const string GenerateProjections = "forecasting-projections-generate-projections";
    }
}