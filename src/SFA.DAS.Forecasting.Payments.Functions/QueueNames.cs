namespace SFA.DAS.Forecasting.Payments.Functions
{
    internal class QueueNames
    {
	    public const string PaymentValidator = "forecasting-payment-validate-payment";
	    public const string PaymentProcessor = "forecasting-payment-process-payment";
        public const string AddEarningDetails = "forecasting-payment-add-earning-details";
        public const string PreLoadPayment = "forecasting-payment-create-preload";
    }
}
