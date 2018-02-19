namespace SFA.DAS.Forecasting.Application.Payments.Messages
{
    public class PreLoadPaymentMessage
    {
        public long EmployerAccountId { get; set; }

        public string HashedEmployerAccountId { get; set; }

        public int PeriodYear { get; set; }

        public int PeriodMonth { get; set; }

        public string PeriodId { get; set; }
    }

    public class PreLoadMessage
    {
        public long EmployerAccountId { get; set; }
        public string PeriodId { get; set; }
    }
}