using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Levy.Domain
{
    public interface ILevyWorker
    {
        Task Run();
    }
}
