using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Forecasting.Web.Attributes;
using SFA.DAS.Forecasting.Web.Authentication;
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
        private readonly AddApprenticeshipViewModelValidator _validator;

        public EstimationController(
            IEstimationOrchestrator estimationOrchestrator, 
            IAddApprenticeshipOrchestrator addApprenticeshipOrchestrator, 
            IMembershipService membershipService,
            AddApprenticeshipViewModelValidator validator)
        {
            _estimationOrchestrator = estimationOrchestrator;
            _membershipService = membershipService; 
            _validator = validator;
            _addApprenticeshipOrchestrator = addApprenticeshipOrchestrator;
        }

        [HttpGet]
        [Route("start", Name = "EstimationStart")]
        public ActionResult StartEstimation(string hashedAccountId)
        {
            ViewBag.HashedAccountId = hashedAccountId;
            return View("StartEstimation");
        }

        [HttpGet]
        [Route("start-transfer", Name = "StartTransferEstimation")]
        public ActionResult StartTransferEstimation(string hashedAccountId)
        {
            ViewBag.HashedAccountId = hashedAccountId;
            return View("StartEstimation");
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
            var vm = _addApprenticeshipOrchestrator.GetApprenticeshipAddSetup(false);
            vm.IsTransferFunded = "";

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
            if (!ModelState.IsValid)
            {
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
            var viewModel = await _addApprenticeshipOrchestrator.UpdateAddApprenticeship(vm);

            var result = _validator.ValidateAdd(vm);

            foreach(var r in result)
            {
                ModelState.AddModelError(r.Key, r.Value);
            }

            if (!ModelState.IsValid)
            {
                viewModel.Courses = _addApprenticeshipOrchestrator.GetApprenticeshipAddSetup(false).Courses;

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
        [Route("{estimationName}/apprenticeship/course")]
        public async Task<ActionResult> GetCourseInfo(string courseId, string estimationName)
        {
            var course = await _addApprenticeshipOrchestrator.GetCourse(courseId);
            var result = new
            {
                CourseId = course?.CourseId,
                NumberOfMonths = course?.NumberOfMonths,
                FundingBands = course?.FundingPeriods
            };

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