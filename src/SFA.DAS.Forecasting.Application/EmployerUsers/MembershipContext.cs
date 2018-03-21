using System;

namespace SFA.DAS.Forecasting.Application.EmployerUsers
{
    public class MembershipContext
    {
        public string HashedAccountId { get; set; }
        public string UserEmail { get; set; }
        public string UserRef { get; set; }
    }
}