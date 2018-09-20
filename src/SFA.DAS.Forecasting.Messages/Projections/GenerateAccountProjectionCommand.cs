using System;

namespace SFA.DAS.Forecasting.Messages.Projections
{
    public class GenerateAccountProjectionCommand
    {
        public long EmployerAccountId { get; set; }
        public ProjectionSource ProjectionSource { get; set; }
        public CalendarPeriod StartPeriod { get; set; }
    }
}