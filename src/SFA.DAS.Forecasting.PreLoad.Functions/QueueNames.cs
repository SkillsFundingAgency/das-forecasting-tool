namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public static class QueueNames
    {
        // Levy
        public const string ValidateLevyDeclaration = "forecasting-levy-validate-declaration";

        // Payment
        public const string PreLoadPayment = "forecasting-payment-create-preload";
        public const string PreLoadEarningDetailsPayment = "forecasting-payment-load-earning-details";
        public const string AddEarningDetails = "forecasting-payment-add-earning-details";
        public const string PaymentValidator = "forecasting-payment-validate-payment";
    }
}