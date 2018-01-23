using SFA.DAS.Forecasting.Levy.Domain.Entities;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Levy.Domain.Repositories
{
    public interface IEmployerLevyRepository
    {
        Task StoreLevyDeclaration(LevyDeclaration levyDeclaration);
    }
}