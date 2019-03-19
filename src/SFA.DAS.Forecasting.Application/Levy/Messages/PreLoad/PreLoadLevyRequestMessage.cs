namespace SFA.DAS.Forecasting.Application.Levy.Messages.PreLoad
{
    public class PreLoadLevyRequestMessage
    {
        public long AccountId { get; set; }
        public short PeriodMonth { get; set; }
        public string PeriodYear { get; set; }
        public bool LevyImported { get; set; }  
    }
}