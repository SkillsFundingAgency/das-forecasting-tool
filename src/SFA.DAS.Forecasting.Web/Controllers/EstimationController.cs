using System.Threading.Tasks;
using System.Web.Mvc;
using FluentValidation.Mvc;
using SFA.DAS.Forecasting.Web.Attributes;
using SFA.DAS.Forecasting.Web.Authentication;
using SFA.DAS.Forecasting.Web.Extensions;
using SFA.DAS.Forecasting.Web.Orchestrators.Estimations;
using SFA.DAS.Forecasting.Web.Orchestrators.Exceptions;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.Forecasting.Web.ViewModels.Validation;

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
        private readonly EditApprenticeshipsViewModelValidator _validator;

        public EstimationController(
            IEstimationOrchestrator estimationOrchestrator, 
            IAddApprenticeshipOrchestrator addApprenticeshipOrchestrator, 
            IMembershipService membershipService,
            EditApprenticeshipsViewModelValidator validator)
        {
            _estimationOrchestrator = estimationOrchestrator;
            _membershipService = membershipService;
            _validator = validator;
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
            var vm = _addApprenticeshipOrchestrator.GetApprenticeshipAddSetup();

            return View(vm);
        }

        [HttpGet]
        [Route("{estimationName}/apprenticeship/{apprenticeshipsId}/EditApprenticeships", Name = "EditApprenticeships")]
        public async Task<ActionResult> EditApprenticeships(string hashedAccountId, string estimationName, string apprenticeshipsId)
        {
            var model = await _estimationOrchestrator.EditApprenticeshipModel(hashedAccountId, apprenticeshipsId, estimationName);
            
            return View(model);
        }

        [HttpPost]
        [Route("{estimationName}/apprenticeship/{apprenticeshipsId}/edit", Name = "PostEditApprenticeships")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PostEditApprenticeships(EditApprenticeshipsViewModel editmodel)
        {
            var results = _validator.Validate(editmodel);
            results.AddToModelState(ModelState, null);

            if (!ModelState.IsValid)
            {
                editmodel.CalculatedTotalCap = editmodel.FundingCap * editmodel.NumberOfApprentices;
                return View("EditApprenticeships", editmodel);
            }

            await _estimationOrchestrator.UpdateApprenticeshipModel(editmodel);


            return RedirectToAction(nameof(CostEstimation),
                   new
                   {
                       hashedaccountId = editmodel.HashedAccountId,
                       estimateName = editmodel.EstimationName
                   });
            
        }
        

        [HttpPost]
        [Route("{estimationName}/apprenticeship/add", Name = "SaveApprenticeship")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(AddApprenticeshipViewModel vm, string hashedAccountId, string estimationName)
        {
            var viewModel = await _addApprenticeshipOrchestrator.ValidateAddApprenticeship(vm);

            if (viewModel.ValidationResults.Count > 0)
            {
                viewModel.PreviousCourseId = viewModel.ApprenticeshipToAdd?.CourseId;
                ModelState.Clear();        
                return View("AddApprenticeships", viewModel);
            }

            await _addApprenticeshipOrchestrator.StoreApprenticeship(viewModel, hashedAccountId, estimationName);

            return RedirectToAction(nameof(CostEstimation), new { hashedaccountId = hashedAccountId, estimateName = estimationName });
        }

        [HttpGet]
        [Route("{estimationName}/apprenticeship/cancel", Name = "CancelAddApprenticeship")]
        public ActionResult Cancel(string hashedAccountId, string estimationName)
        { 
            return RedirectToAction(nameof(CostEstimation), new { hashedaccountId = hashedAccountId, estimateName = estimationName });
        }


        [HttpPost]
        [Route("{estimationName}/apprenticeship/CalculateTotalCost", Name = "CalculateTotalCost")]
        public async Task<ActionResult> CalculateTotalCost(string courseId, int numberOfApprentices, decimal? levyValue, string estimationName)
        {
            var fundingCap = await _addApprenticeshipOrchestrator.GetFundingCapForCourse(courseId);
            var totalValue = fundingCap * numberOfApprentices;
            var totalValueAsString = totalValue.FormatValue();
            var result = new
            {
                FundingCap = fundingCap.FormatCost(),
                TotalFundingCap = totalValue.FormatCost(),
                NumberOfApprentices = numberOfApprentices,
                TotalFundingCapValue = totalValueAsString
            };

            return Json(result);
        }

        [HttpPost]
        [Route("{estimationName}/apprenticeship/GetDefaultNumberOfMonths", Name = "GetDefaultNumberOfMonths")]
        public async Task<ActionResult> GetDefaultNumberOfMonths(string courseId, string estimationName)
        {
            var result = await _addApprenticeshipOrchestrator.GetDefaultNumberOfMonths(courseId);
            return Json(result);
        }

        [HttpGet]
        [Route("{estimationName}/apprenticeship/{id}/ConfirmRemoval", Name = "ConfirmRemoval")]
        public async Task<ActionResult> ConfirmApprenticeshipsRemoval(string hashedAccountId, string estimationName, string id)
        {
            try
            {
                var vm = await _addApprenticeshipOrchestrator.GetVirtualApprenticeshipsForRemoval(hashedAccountId, id, estimationName);
                return View(vm);
            }
            catch (ApprenticeshipAlreadyRemovedException)
            {
                return RedirectToAction(nameof(CostEstimation), new { hashedaccountId = hashedAccountId, estimateName = estimationName, apprenticeshipRemoved = true });
            }
        }

        [HttpPost]
        [Route("{estimationName}/apprenticeship/{id}/remove", Name = "RemoveApprenticeships")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveApprenticeships(RemoveApprenticeshipViewModel viewModel, string hashedAccountId, string estimationName, string id)
        {
            if (viewModel.ConfirmedDeletion.HasValue && viewModel.ConfirmedDeletion.Value)
            {
                await _addApprenticeshipOrchestrator.RemoveApprenticeship(hashedAccountId, id);
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