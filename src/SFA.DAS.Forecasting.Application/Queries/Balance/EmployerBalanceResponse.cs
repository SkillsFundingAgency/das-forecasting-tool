using System.Collections.Generic;

using SFA.DAS.Forecasting.Domain.Entities;

namespace SFA.DAS.Forecasting.Application.Queries.Balance
{
    public class EmployerBalanceResponse
    {
        public IEnumerable<BalanceItem> Data { get; set; } = new BalanceItem[0];
    }
}
