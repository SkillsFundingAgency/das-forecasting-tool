using System;

namespace SFA.DAS.Forecasting.Domain.Entities
{
    public class Apprenticeship
    {
        public long EmployerAccountId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime StartDate { get; set; } 

        public decimal MonthlyPayment { get; set; }

        public int TotalInstallments { get; set; }

        public decimal CompletionPayment { get; set; }
    }
}