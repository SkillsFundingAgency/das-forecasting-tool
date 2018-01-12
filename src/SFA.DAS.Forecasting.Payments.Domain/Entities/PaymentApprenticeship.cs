using System;

namespace SFA.DAS.Forecasting.Payments.Domain.Entities
{
    public class PaymentApprenticeship
    {
        public long EmployerAccountId { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string TrainingName { get; set; }
        public int TrainingLevel { get; set; }
        public string TrainingProviderName { get; set; }
        public DateTime StartDate { get; set; }
        public decimal MonthlyPayment { get; set; }
        public int Instalments { get; set; }
        public decimal CompletionPayment { get; set; }
    }
}
