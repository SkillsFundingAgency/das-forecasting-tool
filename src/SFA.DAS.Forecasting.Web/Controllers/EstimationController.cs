using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Forecasting.Web.Attributes;
using SFA.DAS.Forecasting.Web.Authentication;
using SFA.DAS.Forecasting.Web.Extensions;
using SFA.DAS.Forecasting.Web.Orchestrators;
using SFA.DAS.Forecasting.Web.Orchestrators.Estimations;
using SFA.DAS.Forecasting.Web.Orchestrators.Exceptions;
using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.Controllers
{
    [ValidateMembership]
    [AuthorizeForecasting]
    [RoutePrefix("accounts/{hashedAccountId}/forecasting/estimations")]
    public class EstimationController : Controller
    {
        private readonly IEstimationOrchestrator _estimationOrchestrator;
        private readonly IApprenticeshipOrchestrator _apprenticeshipOrchestrator;
        private readonly IMembershipService _membershipService;

        public EstimationController(IEstimationOrchestrator estimationOrchestrator, IApprenticeshipOrchestrator apprenticeshipOrchestrator, IMembershipService membershipService)
        {
            _estimationOrchestrator = estimationOrchestrator;
            _membershipService = membershipService;
            _apprenticeshipOrchestrator = apprenticeshipOrchestrator;
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
            return await _estimationOrchestrator.HasValidApprenticeships(hashedAccountId)
                ? RedirectToAction(nameof(CostEstimation), new { hashedaccountId = hashedAccountId, estimateName = Constants.DefaultEstimationName })
                : RedirectToAction(nameof(AddApprenticeships), new { hashedAccountId, estimationName = Constants.DefaultEstimationName });
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
        public ActionResult AddApprenticeships(string hashedAccountId, string estimationName)
        {
            var vm = _apprenticeshipOrchestrator.GetApprenticeshipAddSetup();

            return View(vm);
        }


        [HttpPost]
        [Route("{estimationName}/apprenticeship/add", Name = "SaveApprenticeship")]
        public async Task<ActionResult> Save(AddApprenticeshipViewModel vm, string hashedAccountId, string estimationName)
        {
            var viewModel = await _apprenticeshipOrchestrator.ValidateAddApprenticeship(vm);

            if (!viewModel.AddApprenticeshipValidationDetail.IsValid)
            {
                ModelState.Clear();
                return View("AddApprenticeships", viewModel);
            }

            await _apprenticeshipOrchestrator.StoreApprenticeship(viewModel, hashedAccountId, estimationName);

            return RedirectToAction(nameof(CostEstimation), new { hashedaccountId = hashedAccountId, estimateName = estimationName });

        }

            
        [HttpPost]
        [Route("CalculateTotalCost", Name = "CalculateTotalCost")]
        public async Task<ActionResult> CalculateTotalCost(string courseId, int numberOfApprentices, decimal? levyValue)
        {
            //TODO: this should be in the orchestrator
            var fundingCap = await _apprenticeshipOrchestrator.GetFundingCapForCourse(courseId);
            var totalValue = (fundingCap * numberOfApprentices);
            var result = new
            {
                FundingCap = fundingCap.FormatCost(),
                TotalFundingCap = totalValue.FormatCost(),
                NumberOfApprentices = numberOfApprentices,
                TotalFundingCapValue = totalValue
            };

            return Json(result);
        }



        [HttpGet]
        [Route("{estimationName}/apprenticeship/{id}/ConfirmRemoval", Name = "ConfirmRemoval")]
        public async Task<ActionResult> ConfirmApprenticeshipsRemoval(string hashedAccountId, string estimationName, string id)
        {
            try
            {
                var vm = await _apprenticeshipOrchestrator.GetVirtualApprenticeshipsForRemoval(hashedAccountId, id, estimationName);
                return View(vm);
            }
            catch (ApprenticeshipAlreadyRemovedException)
            {
                return RedirectToAction(nameof(CostEstimation), new { hashedaccountId = hashedAccountId, estimateName = estimationName, apprenticeshipRemoved = true });
            }

        }


        [HttpPost]
        [Route("{estimationName}/apprenticeship/{id}/remove", Name = "RemoveApprenticeships")]
        public async Task<ActionResult> RemoveApprenticeships(RemoveApprenticeshipViewModel viewModel, string hashedAccountId, string estimationName, string id)
        {
            if (viewModel.ConfirmedDeletion.HasValue && viewModel.ConfirmedDeletion.Value)
            {
                await _apprenticeshipOrchestrator.RemoveApprenticeship(hashedAccountId, id);
                return RedirectToAction(nameof(CostEstimation),
                    new
                    {
                        hashedaccountId = hashedAccountId,
                        estimateName = estimationName,
                        apprenticeshipRemoved = true
                    });
            }
            else if (viewModel.ConfirmedDeletion.HasValue && !viewModel.ConfirmedDeletion.Value)
            {
                return RedirectToAction(nameof(CostEstimation),
                   new
                   {
                       hashedaccountId = hashedAccountId,
                       estimateName = estimationName,
                       apprenticeshipRemoved = false
                   });
            }

            return View(nameof(ConfirmApprenticeshipsRemoval), viewModel);
        }

    }

}