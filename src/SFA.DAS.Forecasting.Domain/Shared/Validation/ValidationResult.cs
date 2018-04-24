namespace SFA.DAS.Forecasting.Domain.Shared.Validation
{
    public class ValidationResult
    {
        public bool IsValid { get; private set; }
        public string FailureReason { get; private set; }
        public static ValidationResult Success => new ValidationResult { IsValid = true };
        public static ValidationResult Failed(string failureReason) => new ValidationResult { IsValid = false, FailureReason = failureReason };
    }
}