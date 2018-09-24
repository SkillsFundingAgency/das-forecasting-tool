using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.Orchestrators.Estimations
{
    public interface IAddApprenticeshipOrchestrator
    {
        AddEditApprenticeshipsViewModel GetApprenticeshipAddSetup(bool standardsOnly);

        Task StoreApprenticeship(AddEditApprenticeshipsViewModel vm, string hashedAccountId, string estimationName);

        Task RemoveApprenticeship(string hashedAccountId, string apprenticeshipId);

        Task<RemoveApprenticeshipViewModel> GetVirtualApprenticeshipsForRemoval(string hashedAccountId, string apprenticeshipsId, string estimationName);
        Task<AddEditApprenticeshipsViewModel> UpdateAddApprenticeship(AddEditApprenticeshipsViewModel vm);

        Task<CourseViewModel> GetCourse(string courseId);

        List<ApprenticeshipCourse> GetStandardCourses();
    }
}