using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Levy.Domain.Services
{
    public interface ILevyPeriodService
    {
        Task<string> GetCurrentPeriod();
    }
}