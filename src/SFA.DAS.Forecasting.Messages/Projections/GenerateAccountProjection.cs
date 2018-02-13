using System;

namespace SFA.DAS.Forecasting.Messages.Projections
{
    public abstract class GenerateAccountProjection
    {
        public long EmployerAccountId { get; set; }
        public string PayrollYear { get; set; }
        public short PayrollMonth { get; set; }
    }
}