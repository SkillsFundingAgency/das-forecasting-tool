using MediatR;

namespace SFA.DAS.Forecasting.Application.Queries.Apprenticeships
{
    public class ApprenticeshipsRequest : IRequest<ApprenticeshipsResponse>
    {
        public long EmployerAccountId { get; set; }
    }
}
