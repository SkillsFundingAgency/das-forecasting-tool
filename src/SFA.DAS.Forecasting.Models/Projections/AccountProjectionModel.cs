using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Models.Projections
{
    public class AccountProjectionModel: IDocument
    {
        public string Id { get; set; }
        public long EmployerAccountId { get; set; }
        public System.DateTime ProjectionCreationDate { get; set; }
        public ProjectionGenerationType ProjectionGenerationType { get; set; }
        public List<AccountProjectionMonth> Projections { get; set; }
        public List<long> Commitments { get; set; }

        public AccountProjectionModel()
        {
            Projections = new List<AccountProjectionMonth>(49);
            Commitments = new List<long>();
        }
    }
}