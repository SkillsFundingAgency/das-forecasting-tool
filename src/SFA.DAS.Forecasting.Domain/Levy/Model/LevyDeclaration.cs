using System;

namespace SFA.DAS.Forecasting.Domain.Levy.Model
{
    public class LevyDeclaration
    {
        public long EmployerAccountId { get; set; }
        public string Scheme { get; set; }
        /// <summary>
        /// in the form of yy-yy e.g. 18-19
        /// </summary>
        public string PayrollYear { get; set; }
        public short PayrollMonth { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}