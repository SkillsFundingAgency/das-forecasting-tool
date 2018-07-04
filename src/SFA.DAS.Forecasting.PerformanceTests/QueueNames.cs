namespace SFA.DAS.Forecasting.PerformanceTests
{
    public static class QueueNames
    {
        public const string LevyValidateDeclaration = "forecasting-levy-validate-declaration";
        public static class Payments
        {
            public const string PaymentValidator = "forecasting-payment-validate-payment";
            public const string PaymentProcessor = "forecasting-payment-process-payment";
            public const string CommitmentProcessor = "forecasting-payment-process-commitment";
            public const string AllowProjection = "forecasting-payment-allow-projection";
            public const string GenerateProjections = "forecasting-projections-generate-projections";
        }

    }
}