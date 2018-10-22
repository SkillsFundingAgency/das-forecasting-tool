namespace SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad
{
    public class PreLoadPaymentRequestMessage
    {
        public string PayrollPeriod { get; set; }   
        public long AccountId { get; set; }
        public bool PaymentsProcessed { get; set; }
    }
}
