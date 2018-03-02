using System;
using System.Collections.Generic;

namespace SFA.DAS.Forecasting.ReadModel.Projections
{
    public class AccountProjectionReadModel
    {
        public long Id { get; set; }
        public long EmployerAccountId { get; set; }
        public DateTime ProjectionCreationDate { get; set; }
        public ProjectionGenerationType ProjectionGenerationType { get; set; }
        public short Month { get; set; }
        public int Year { get;set; }
        public decimal FundsIn { get; set; }
        public decimal TotalCostOfTraining { get; set; }
        public decimal CompletionPayments { get; set; }
        public decimal FutureFunds { get; set; }
        public List<long> Commitments { get; set; } = new List<long>();
        public decimal CoInvestmentEmployer { get; set; }
        public decimal CoInvestmentGovernment { get; set; }
    }
}