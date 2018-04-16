using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Forecasting.Web.Attributes;
using SFA.DAS.Forecasting.Web.Authentication;
using SFA.DAS.Forecasting.Web.Mvc;
using SFA.DAS.Forecasting.Web.Orchestrators;
using SFA.DAS.Forecasting.Web.Orchestrators.Estimations;
using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.Controllers
{
    [ValidateMembership]
    [AuthorizeForecasting]
    [RoutePrefix("accounts/{hashedAccountId}/forecasting/estimations")]
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
        [Route("start-transfer", Name = "EstimationStart")]
        public ActionResult StartEstimation(string hashedAccountId)
        {
            ViewBag.HashedAccountId = hashedAccountId;
            return View();
        }

        [HttpGet]
        [Route("start-redirect", Name = "EstimationStartRedirect")]
        public async Task<ActionResult> RedirectEstimationStart(string hashedAccountId)
        {
            var accountEstimation = await _estimationOrchestrator.GetEstimation(hashedAccountId);

            if (accountEstimation != null && accountEstimation.HasValidApprenticeships)
            {
                return RedirectToAction(nameof(CostEstimation), new { hashedaccountId = hashedAccountId, estimateName = accountEstimation.Name });
            }

            return RedirectToAction(nameof(AddApprenticeships), new { hashedAccountId, estimationName = Constants.DefaultEstimationName });
        }

        [HttpGet]
        [Route("{estimateName}/{apprenticeshipRemoved?}", Name = "EstimatedCost")]
        public async Task<ActionResult> CostEstimation(string hashedAccountId, string estimateName, bool? apprenticeshipRemoved)
        {
            var viewModel = await _estimationOrchestrator.CostEstimation(hashedAccountId, estimateName, apprenticeshipRemoved);
            return View(viewModel);
        }

        [HttpGet]
        [Route("{estimationName}/apprenticeship/add", Name = "AddApprenticeships")]
        public async Task<ActionResult> AddApprenticeships(string hashedAccountId, string estimationName)
        {
            var vm = await _addApprenticeshipOrchestrator.GetApprenticeshipAddSetup(hashedAccountId, estimationName);

            return View(vm);
        }


        [HttpPost]
        [Route("{estimationName}/apprenticeship/add", Name = "SaveApprenticeship")]
        public ActionResult Save(AddApprenticeshipViewModel vm, string hashedAccountId, string estimationName)
        {
            var estimationCostsUrl = $"estimations/{vm.EstimationName}";


            _addApprenticeshipOrchestrator.StoreApprenticeship(vm);

            return RedirectToAction(nameof(CostEstimation), new { hashedaccountId = hashedAccountId, estimateName = estimationName });


        }

        [HttpGet]
        [Route("{estimationName}/apprenticeship/{id}/remove", Name = "RemoveApprenticeships")]
        public async Task<ActionResult> RemoveApprenticeships(string hashedAccountId, string estimationName, string id)
        {
            var vm = await _addApprenticeshipOrchestrator.GetApprenticeshipAddSetup(hashedAccountId, estimationName);
            return View(vm);
        }

    }

}