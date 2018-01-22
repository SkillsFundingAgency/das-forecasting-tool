using SFA.DAS.Forecasting.Levy.Domain.Entities;

namespace SFA.DAS.Forecasting.Levy.Domain.Repositories
{
    public interface IEmployerLevyRepository
    {
        void StoreLevyDeclaration(LevyDeclaration levyDeclaration);
    }
}