using System.Threading.Tasks;
using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.Orchestrators.Estimations
{
    public interface IApprenticeshipOrchestrator
    {
        AddApprenticeshipViewModel GetApprenticeshipAddSetup();

        Task StoreApprenticeship(AddApprenticeshipViewModel vm, string hashedAccountId, string estimationName);

        Task RemoveApprenticeship(string hashedAccountId, string apprenticeshipId);

        Task<RemoveApprenticeshipViewModel> GetVirtualApprenticeshipsForRemoval(string hashedAccountId, string apprenticeshipsId, string estimationName);
        Task<AddApprenticeshipViewModel> ValidateAddApprenticeship(AddApprenticeshipViewModel vm);

        AddApprenticeshipViewModel AdjustTotalCostApprenticeship(AddApprenticeshipViewModel vm);
    }
}