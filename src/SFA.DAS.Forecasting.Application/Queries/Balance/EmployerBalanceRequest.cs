using MediatR;

namespace SFA.DAS.Forecasting.Application.Queries.Balance
{
    public class EmployerBalanceRequest : IRequest<EmployerBalanceResponse>
    {
        public long EmployerAccountId { get; set; }
    }
}