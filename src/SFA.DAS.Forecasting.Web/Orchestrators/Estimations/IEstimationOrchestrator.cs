using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.Orchestrators.Estimations
{
    public interface IEstimationOrchestrator
    {
        Task<EstimationPageViewModel> CostEstimation(string hashedAccountId, string estimateName, bool? apprenticeshipRemoved);
        Task<bool> HasValidApprenticeships(string hashedAccountId);
        Task<AddEditApprenticeshipsViewModel> EditApprenticeshipModel(string hashedAccountId, string apprenticeshipsId, string estimationName);
        Task UpdateApprenticeshipModel(AddEditApprenticeshipsViewModel editApprenticeships);
    }
}