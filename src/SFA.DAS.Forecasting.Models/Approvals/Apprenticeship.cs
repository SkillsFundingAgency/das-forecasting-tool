using System;

namespace SFA.DAS.Forecasting.Models.Approvals
{
    public class Apprenticeship
    {
        public long Id { get; set; }
        public long EmployerAccountId { get; set; }
        public long? TransferSenderId { get; set; }
        public string Uln { get; set; }
        public long ProviderId { get; set; }
        public string ProviderName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? Cost { get; set; }
    }
}
