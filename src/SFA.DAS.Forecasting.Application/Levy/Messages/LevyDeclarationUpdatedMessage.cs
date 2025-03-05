using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages;

public class LevyDeclarationUpdatedMessage
{
    public long AccountId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string PayrollYear { get; set; }
    public short? PayrollMonth { get; set; }
    public decimal LevyDeclaredInMonth { get; set; }
}