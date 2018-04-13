using System.Threading.Tasks;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.Orchestrators
{
    public interface IApprenticeshipOrchestrator
    {
        Task<ApprenticeshipAddViewModel> GetApprenticeshipAddSetup(string hashedAccountId, string estimationName);

        void StoreApprenticeship(string hashedAccountId, string estimationName, ApprenticeshipAddViewModel viewModel);
    }
}