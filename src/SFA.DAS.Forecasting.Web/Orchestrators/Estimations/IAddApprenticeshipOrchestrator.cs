using System.Threading.Tasks;
using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.Orchestrators.Estimations
{
    public interface IAddApprenticeshipOrchestrator
    {
        Task<AddApprenticeshipViewModel> GetApprenticeshipAddSetup(string hashedAccountId, string estimationName);

        void StoreApprenticeship(AddApprenticeshipViewModel viewModel);
    }
}