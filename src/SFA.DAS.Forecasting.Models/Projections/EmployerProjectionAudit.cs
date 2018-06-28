using System;

namespace SFA.DAS.Forecasting.Models.Projections
{
    public class EmployerProjectionAudit: IDocument
    {
        public string Id { get; set; }
        public long EmployerAccountId { get; set; }
        public DateTimeOffset LastGenerated { get; set; }
    }
}