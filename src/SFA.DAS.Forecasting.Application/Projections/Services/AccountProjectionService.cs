using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Projections;
using SFA.DAS.Forecasting.Messages.Projections;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Application.Projections.Services;

public interface IAccountProjectionService
{
    Task<ProjectionSource> GetOriginalProjectionSource(long employerAccountId,
        ProjectionSource currentProjectionSource);
}

public class AccountProjectionService : IAccountProjectionService
{
    private readonly IAccountProjectionRepository _accountProjectionRepository;
        
    public AccountProjectionService(IAccountProjectionRepository accountProjectionRepository)
    {
        _accountProjectionRepository = accountProjectionRepository;
    }

    public async Task<ProjectionSource> GetOriginalProjectionSource(long employerAccountId, ProjectionSource currentProjectionSource)
    {
            
        var messageProjectionSource = currentProjectionSource;
        if (messageProjectionSource == ProjectionSource.Commitment)
        {
            var previousProjection = await _accountProjectionRepository.Get(employerAccountId);
            var projectionGenerationType = previousProjection?.FirstOrDefault()?.ProjectionGenerationType;
            if (projectionGenerationType != null)
            {
                messageProjectionSource = projectionGenerationType == ProjectionGenerationType.LevyDeclaration
                    ? ProjectionSource.LevyDeclaration : ProjectionSource.PaymentPeriodEnd;
            }
        }
            
        return messageProjectionSource;
    }
}