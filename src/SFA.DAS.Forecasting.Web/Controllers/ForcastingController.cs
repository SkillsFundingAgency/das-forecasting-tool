using System.Web.Mvc;

using SFA.DAS.Forecasting.Web.Orchestrators;

namespace SFA.DAS.Forecasting.Web.Controllers
{
    public class ForcastingController : Controller
    {
        private readonly ForcastingOrchestrator _orchestrator;

        public ForcastingController(ForcastingOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        // GET: Forcasting
        public ActionResult Balance(string hashedAccountId)
        {
            var viewModel = _orchestrator.Balance(hashedAccountId);
            return View(viewModel);
        }

        public ActionResult Apprenticeships()
        {
            return View();
        }

        public ActionResult Visualisation()
        {
            return View();
        }
    }
}