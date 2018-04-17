using System.Threading.Tasks;
using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.Orchestrators.Estimations
{
    public interface IApprenticeshipOrchestrator
    {
        Task<AddApprenticeshipViewModel> GetApprenticeshipAddSetup();

        void StoreApprenticeship(AddApprenticeshipViewModel viewModel, string hashedAccountId, string estimationName);
    }
}