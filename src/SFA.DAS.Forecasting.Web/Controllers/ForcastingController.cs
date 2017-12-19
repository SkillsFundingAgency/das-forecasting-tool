using System.Web.Mvc;

using SFA.DAS.Forecasting.Web.Mvc;
using SFA.DAS.Forecasting.Web.Orchestrators;

namespace SFA.DAS.Forecasting.Web.Controllers
{
    [ForcastingRoutePrefix("accounts/{hashedaccountId}/forcasting")]
    public class ForcastingController : Controller
    {
        private readonly ForcastingOrchestrator _orchestrator;

        public ForcastingController(ForcastingOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet]
        [Route("balance", Name = "ForcastingBalance")]
        public ActionResult Balance(string hashedAccountId)
        {
            var viewModel = _orchestrator.Balance(hashedAccountId);
            return View(viewModel);
        }

        [HttpGet]
        [Route("apprenticeships", Name = "ForcastingApprenticeships")]
        public ActionResult Apprenticeships()
        {
            return View();
        }

        [HttpGet]
        [Route("visualisation", Name = "ForcastingVisualisation")]
        public ActionResult Visualisation()
        {
            return View();
        }
    }
}