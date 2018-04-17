using System.Threading.Tasks;
using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.Orchestrators.Estimations
{
    public interface IApprenticeshipOrchestrator
    {
        Task<AddApprenticeshipViewModel> GetApprenticeshipAddSetup(string hashedAccountId, string estimationName);

        Task StoreApprenticeship(AddApprenticeshipViewModel vm, string hashedAccountId, string estimationName);

        Task RemoveApprenticeship(string hashedAccountId, string apprenticeshipId);

        Task<RemoveApprenticeshipViewModel> GetVirtualApprenticeshipsForRemoval(string hashedAccountId, string apprenticeshipsId, string estimationName);
    }
}