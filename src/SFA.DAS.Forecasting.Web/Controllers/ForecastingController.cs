using System.Threading.Tasks;
using System.Web.Mvc;

using SFA.DAS.Forecasting.Web.Mvc;
using SFA.DAS.Forecasting.Web.Orchestrators;

namespace SFA.DAS.Forecasting.Web.Controllers
{
    [ForecastingRoutePrefix("accounts/{hashedaccountId}/forecasting")]
    public class ForecastingController : Controller
    {
        private readonly ForecastingOrchestrator _orchestrator;

        public ForecastingController(ForecastingOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet]
        [Route("balance", Name = "ForecastingBalance")]
        public async Task<ActionResult> Balance(string hashedAccountId)
        {
            var viewModel = await _orchestrator.Balance(hashedAccountId);
            return View(viewModel);
        }

        [HttpGet]
        [Route("apprenticeships", Name = "ForecastingApprenticeships")]
        public ActionResult Apprenticeships()
        {
            return View();
        }

        [HttpGet]
        [Route("visualisation", Name = "ForecastingVisualisation")]
        public ActionResult Visualisation()
        {
            return View();
        }
    }
}