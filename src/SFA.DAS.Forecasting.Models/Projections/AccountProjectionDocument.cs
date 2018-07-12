using System;
using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Models.Projections
{
    public class AccountProjectionDocument : IDocument
    {
        public string Id { get; set; }
        public long EmployerAccountId { get; set; }
        public System.DateTime ProjectionCreationDate { get; set; }
        public ProjectionGenerationType ProjectionGenerationType { get; set; }
        public List<AccountProjectionMonth> Projections { get; set; } = new List<AccountProjectionMonth>();
    }
}