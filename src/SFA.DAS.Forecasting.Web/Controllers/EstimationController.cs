using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Forecasting.Web.Attributes;
using SFA.DAS.Forecasting.Web.Authentication;
using SFA.DAS.Forecasting.Web.Orchestrators;

namespace SFA.DAS.Forecasting.Web.Controllers
{
    [ValidateMembership]
    [AuthorizeForecasting]
    [RoutePrefixAttribute("accounts/{hashedaccountId}/forecasting")]
    public class EstimationController : Controller
    {
        private readonly IEstimationOrchestrator _orchestrator;
        private readonly IMembershipService _membershipService;

        public EstimationController(IEstimationOrchestrator orchestrator, IMembershipService membershipService)
        {
            _orchestrator = orchestrator;
            _membershipService = membershipService;
        }

        [HttpGet]
        [Route("estimations/start-transfer", Name = "EstimationStart")]
        public ActionResult StartEstimation(string hashedAccountId)
        {
            ViewBag.HashedAccountId = hashedAccountId;
            return View();
        }

        [HttpGet]
        [Route("estimations/start-redirect", Name = "EstimationStartRedirect")]
        public ActionResult RedirectEstimationStart(string hashedAccountId)
        {
            var accountEstimation = _orchestrator.GetEstimation(hashedAccountId);

            if (accountEstimation != null)
            {
                return RedirectToAction(nameof(CostEstimation), new { hashedaccountId = hashedAccountId, estimateName = "default" });
            }

            return RedirectToRoute("AddApprenticeships",new { hashedAccountId });
        }

        [HttpGet]
        [Route("estimations/{estimateName}/{apprenticeshipRemoved?}", Name = "EstimatedCost")]
        public async Task<ActionResult> CostEstimation(string hashedAccountId, string estimateName, bool? apprenticeshipRemoved)
        {
            var viewModel = await _orchestrator.CostEstimation(hashedAccountId, estimateName, apprenticeshipRemoved);
            return View(viewModel);
        }



    }

}