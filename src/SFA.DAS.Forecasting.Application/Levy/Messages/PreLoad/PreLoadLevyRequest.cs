namespace SFA.DAS.Forecasting.Application.Levy.Messages.PreLoad
{
    public class PreLoadLevyRequest
    {
        public long[] EmployerAccountIds { get; set; }
        public string PeriodYear { get; set; }
        public short PeriodMonth { get; set; }
        public long? SubstitutionId { get; set; }
    }
}