using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.Orchestrators.Estimations
{
    public interface IAddApprenticeshipOrchestrator
    {
        AddApprenticeshipViewModel GetApprenticeshipAddSetup();

        Task StoreApprenticeship(AddApprenticeshipViewModel vm, string hashedAccountId, string estimationName);

        Task RemoveApprenticeship(string hashedAccountId, string apprenticeshipId);

        Task<RemoveApprenticeshipViewModel> GetVirtualApprenticeshipsForRemoval(string hashedAccountId, string apprenticeshipsId, string estimationName);
        Task<AddApprenticeshipViewModel> ValidateAddApprenticeship(AddApprenticeshipViewModel vm);

        void AdjustTotalCostApprenticeship(AddApprenticeshipViewModel vm);

        Task<decimal?> GetFundingCapForCourse(string courseId);

        Task<object> GetDefaultNumberOfMonths(string courseId);
    }
}