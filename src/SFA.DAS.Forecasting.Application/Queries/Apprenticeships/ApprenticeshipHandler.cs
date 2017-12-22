using System.Threading;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Forecasting.Domain.Interfaces;

namespace SFA.DAS.Forecasting.Application.Queries.Apprenticeships
{
    public class ApprenticeshipHandler : IRequestHandler<ApprenticeshipsRequest, ApprenticeshipsResponse>
    {
        private readonly IApprenticeshipRepository _apprenticeshipRepository;

        public ApprenticeshipHandler(IApprenticeshipRepository apprenticeshipRepository)
        {
            _apprenticeshipRepository = apprenticeshipRepository;
        }

        public async Task<ApprenticeshipsResponse> Handle(ApprenticeshipsRequest request, CancellationToken cancellationToken)
        {
            var apprenticeships = await _apprenticeshipRepository.GetApprenticeships(request.EmployerAccountId);
            return new ApprenticeshipsResponse { Data = apprenticeships };
        }
    }
}