using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    public class LevyDeclarationUpdatedMessage
    {
        public long AccountId { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public string PayrollYear { get; set; }
        public short? PayrollMonth { get; set; }
        public decimal LevyDeclaredInMonth { get; set; }
    }
}