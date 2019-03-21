namespace SFA.DAS.Forecasting.Messages.Projections
{
    public class AggregatePaymentDataCommand
    {
        public long EmployerAccountId { get; set; }
        public int CollectionPeriodYear { get; set; }
        public int CollectionPeriodMonth { get; set; }
    }
}