using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Forecasting.Web.Attributes;
using SFA.DAS.Forecasting.Web.Authentication;
using SFA.DAS.Forecasting.Web.Orchestrators;

namespace SFA.DAS.Forecasting.Web.Controllers
{
    [ValidateMembership]
    [AuthorizeForecasting]
    [Route("accounts/{hashedaccountId}/forecasting")]
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
        public async Task<ActionResult> StartEstimation(string hashedAccountId, string estimateName)
        {
            var viewModel = await _orchestrator.CostEstimation(hashedAccountId, estimateName);
            return View(viewModel);
        }

        [HttpGet]
        [Route("estimations/start-redirect", Name = "EstimationStartRedirect")]
        public ActionResult RedirectEstimationStart(string hashedAccountId)
        {
            var foundEstimation = true;

            if (foundEstimation)
            {
                return RedirectToAction("CostEstimation", new { hashedaccountId = hashedAccountId, estimateName = "default" });
            }

            return RedirectToAction("", "", null);
        }

        [HttpGet]
        [Route("estimations/{estimateName}", Name = "EstimatedCost")]
        public async Task<ActionResult> CostEstimation(string hashedAccountId, string estimateName)
        {
            var viewModel = await _orchestrator.CostEstimation(hashedAccountId, estimateName);
            return View(viewModel);
        }


    }

}