using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using CsvHelper;
using SFA.DAS.Forecasting.Web.Attributes;
using SFA.DAS.Forecasting.Web.Authentication;
using SFA.DAS.Forecasting.Web.Mvc;
using SFA.DAS.Forecasting.Web.Orchestrators;

namespace SFA.DAS.Forecasting.Web.Controllers
{
    [ValidateMembership]
    [AuthorizeForecasting]
    [Route("accounts/{hashedaccountId}/estimation")]
    public class EstimationController : Controller
    {
        private readonly EstimationOrchestrator _orchestrator;
        private readonly IMembershipService _membershipService;

        public EstimationController(EstimationOrchestrator orchestrator, IMembershipService membershipService)
        {
            _orchestrator = orchestrator;
            _membershipService = membershipService;
        }

        [HttpGet]
        [Route("", Name = "EstimatedCost")]
        public async Task<ActionResult> CostEstimations(string hashedAccountId)
        {
            var viewModel = await _orchestrator.Balance(hashedAccountId);
            return View(viewModel);
        }

    }

}