using System.Threading.Tasks;
using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.Orchestrators
{
    public interface IEstimationOrchestrator
    {
        Task<EstimationPageViewModel> CostEstimation(string hashedAccountId, string estimateName);
    }
}