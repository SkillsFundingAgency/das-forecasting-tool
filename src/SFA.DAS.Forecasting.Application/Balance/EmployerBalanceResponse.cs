using System.Collections.Generic;

using SFA.DAS.EmployerCommitments.Domain.Entities;

namespace SFA.DAS.Forcasting.Application.Balance
{
    public class EmployerBalanceResponse
    {
        public IEnumerable<BalanceItem> Data { get; set; } = new BalanceItem[0];
    }
}
