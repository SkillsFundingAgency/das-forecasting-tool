using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Forecasting.Web.Authentication;
using SFA.DAS.Forecasting.Web.Orchestrators;
using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.Controllers
{
    //[ValidateMembership]
    //[AuthorizeForecasting]
    [RoutePrefix("accounts/{hashedAccountId}/forecasting")]
    public class AppenticeshipsController : Controller
    {

        private readonly IApprenticeshipOrchestrator _orchestrator;
        private readonly IMembershipService _membershipService;

              public AppenticeshipsController(IApprenticeshipOrchestrator orchestrator, IMembershipService membershipService)
        {
            _orchestrator = orchestrator;
            _membershipService = membershipService;
        }

        [HttpGet]
        [Route("estimations/{estimationName}/apprenticeship/add", Name = "AddApprenticeships")]
        public async Task<ActionResult> AddApprenticeships(string hashedAccountId, string estimationName)
        {
            var vm = await _orchestrator.GetApprenticeshipAddSetup(hashedAccountId, estimationName);

            return View(vm);
        }

        [HttpPost]
        public ActionResult Store(ApprenticeshipAddViewModel vm)
        {
            var estimationCostsUrl = "";

            _orchestrator.StoreApprenticeship(vm);

            return RedirectToAction(estimationCostsUrl);

        }
    }
}