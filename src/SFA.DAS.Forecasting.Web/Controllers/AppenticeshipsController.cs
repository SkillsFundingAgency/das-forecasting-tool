using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Web.Authentication;
using SFA.DAS.Forecasting.Web.Orchestrators;
using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.Controllers
{
    //[ValidateMembership]
    //[AuthorizeForecasting]
   //[Route("accounts/{hashedAccountId}/forecasting")]
    public class AppenticeshipsController: Controller
    {

        private readonly IApprenticeshipOrchestrator _orchestrator;
        private readonly IMembershipService _membershipService;

        public AppenticeshipsController(IApprenticeshipOrchestrator orchestrator, IMembershipService membershipService)
        {
            _orchestrator = orchestrator;
            _membershipService = membershipService;
        }

        [HttpGet]
        [Route("accounts/{hashedAccountId}/forecasting/estimations/{estimationName}/apprenticeship/add", Name = "AddApprenticeships")]
        public async Task<ActionResult> AddApprenticeships(string hashedAccountId, string estimationName)
        {
            var vm = await _orchestrator.GetApprenticeshipAddSetup(hashedAccountId, estimationName);

            return View(vm);
        }


        [HttpGet]
        [Route("accounts/{hashedAccountId}/forecasting/estimations/{estimationName}/apprenticeship/remove/{rowId}", Name = "RemoveApprenticeships")]
        public async Task<ActionResult> RemoveApprenticeships(string hashedAccountId, string estimationName, string rowId)
        {
               var vm = new ApprenticeshipRemoveViewModel
            {
                Name = "Add Apprenticeships",
                RowId = rowId,
                EstimationName = estimationName
              };
            return View(vm);
        }
    }
}