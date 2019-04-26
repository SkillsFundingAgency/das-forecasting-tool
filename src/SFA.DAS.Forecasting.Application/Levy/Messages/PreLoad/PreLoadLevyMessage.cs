namespace SFA.DAS.Forecasting.Application.Levy.Messages.PreLoad
{
    public class PreLoadLevyMessage
    {
        public long? SubstitutionId { get; set; }
        public long EmployerAccountId { get; set; }
        public string PeriodYear { get; set; }
        public short PeriodMonth { get; set; }
    }
}