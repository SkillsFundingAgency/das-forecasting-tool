namespace SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad
{
    public class AllEmployersPreLoadPaymentRequest
    {
        public int PeriodYear { get; set; }

        public int PeriodMonth { get; set; }

        public string PeriodId { get; set; }
    }
}