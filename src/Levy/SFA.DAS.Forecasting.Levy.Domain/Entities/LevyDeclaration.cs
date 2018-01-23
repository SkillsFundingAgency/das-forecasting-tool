using System;

namespace SFA.DAS.Forecasting.Levy.Domain.Entities
{
    public class LevyDeclaration
    {
        public long EmployerAccountId { get; set; }
        public string Scheme { get; set; }
        public string Period { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}