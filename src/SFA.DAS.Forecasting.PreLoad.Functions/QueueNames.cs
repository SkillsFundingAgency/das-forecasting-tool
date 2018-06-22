namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public static class QueueNames
    {
        // Levy
        public const string ValidateLevyDeclaration = "forecasting-levy-validate-declaration";
        public const string LevyPreLoadRequest = "forecasting-levy-preload-request";

        // Payment
        public const string PreLoadPayment = "forecasting-payment-create-preload";
        public const string PreLoadEarningDetailsPayment = "forecasting-payment-load-earning-details";
        public const string CreatePaymentMessage = "forecasting-payment-create-payment-message";
        public const string RemovePreLoadData = "forecasting-payment-remove-preload-data";
        public const string PaymentValidator = "forecasting-payment-validate-payment";
    }
}