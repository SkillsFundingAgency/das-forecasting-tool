using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Forecasting.Web.Attributes;
using SFA.DAS.Forecasting.Web.Authentication;
using SFA.DAS.Forecasting.Web.Orchestrators;
using SFA.DAS.Forecasting.Web.Orchestrators.Estimations;
using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.Controllers
{
    [ValidateMembership]
    [AuthorizeForecasting]
    [RoutePrefix("accounts/{hashedaccountId}/forecasting")]
    public class EstimationController : Controller
    {
        private readonly IEstimationOrchestrator _estimationOrchestrator;
        private readonly IAddApprenticeshipOrchestrator _addApprenticeshipOrchestrator;
        private readonly IMembershipService _membershipService;

        public EstimationController(IEstimationOrchestrator estimationOrchestrator, IAddApprenticeshipOrchestrator addApprenticeshipOrchestrator, IMembershipService membershipService)
        {
            _estimationOrchestrator = estimationOrchestrator;
            _membershipService = membershipService;
            _addApprenticeshipOrchestrator = addApprenticeshipOrchestrator;
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
        public async Task<ActionResult> RedirectEstimationStart(string hashedAccountId)
        {
            var accountEstimation = await _estimationOrchestrator.GetEstimation(hashedAccountId);

            if (accountEstimation != null && accountEstimation.HasValidApprenticeships)
            {
                return RedirectToAction(nameof(CostEstimation), new { hashedaccountId = hashedAccountId, estimateName = accountEstimation.Name });
            }

            return RedirectToRoute("AddApprenticeships",new { hashedAccountId });
        }

        [HttpGet]
        [Route("estimations/{estimateName}/{apprenticeshipRemoved?}", Name = "EstimatedCost")]
        public async Task<ActionResult> CostEstimation(string hashedAccountId, string estimateName, bool? apprenticeshipRemoved)
        {
            var viewModel = await _estimationOrchestrator.CostEstimation(hashedAccountId, estimateName, apprenticeshipRemoved);
            return View(viewModel);
        }

        [HttpGet]
        [Route("estimations/{estimationName}/apprenticeship/add", Name = "AddApprenticeships")]
        public async Task<ActionResult> AddApprenticeships(string hashedAccountId, string estimationName)
        {
            var vm = await _addApprenticeshipOrchestrator.GetApprenticeshipAddSetup(hashedAccountId, estimationName);

            return View(vm);
        }


        [HttpPost]
        [Route("estimations/{estimationName}/apprenticeship/add", Name = "SaveApprenticeship")]
        public ActionResult Save(AddApprenticeshipViewModel vm, string hashedAccountId, string estimationName)
        {
            var estimationCostsUrl = $"estimations/{vm.EstimationName}";


            _addApprenticeshipOrchestrator.StoreApprenticeship(vm);

            return RedirectToAction(nameof(CostEstimation), new { hashedaccountId = hashedAccountId, estimateName = estimationName });


        }
    }

}