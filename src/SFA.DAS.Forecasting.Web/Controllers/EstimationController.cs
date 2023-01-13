using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Employer.Shared.UI.Attributes;
using SFA.DAS.Forecasting.Web.Authentication;
using SFA.DAS.Forecasting.Web.Configuration;
using SFA.DAS.Forecasting.Web.Orchestrators.Estimations;
using SFA.DAS.Forecasting.Web.Orchestrators.Exceptions;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.Forecasting.Web.ViewModels.Validation;

namespace SFA.DAS.Forecasting.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasEmployerAccount))]
    [Route("accounts/{hashedAccountId}/forecasting/estimations")]
    [SetNavigationSection(NavigationSection.AccountsFinance)]
    public class EstimationController : Controller
    {
        private readonly IEstimationOrchestrator _estimationOrchestrator;
        private readonly IAddApprenticeshipOrchestrator _addApprenticeshipOrchestrator;
        
        private readonly IValidator<AddEditApprenticeshipsViewModel> _validator;
        

        public EstimationController(
            IEstimationOrchestrator estimationOrchestrator, 
            IAddApprenticeshipOrchestrator addApprenticeshipOrchestrator, 
            IValidator<AddEditApprenticeshipsViewModel> validator)
        {
            _estimationOrchestrator = estimationOrchestrator;
            _validator = validator;
            _addApprenticeshipOrchestrator = addApprenticeshipOrchestrator;
        }

        [HttpGet]
        [Route("start", Name = RouteNames.StartEstimation)]
        public ActionResult StartEstimation(string hashedAccountId)
        {
            ViewBag.HashedAccountId = hashedAccountId;
            return View("StartEstimation");
        }

        [HttpGet]
        [Route("start-transfer", Name = RouteNames.StartTransferEstimation)]
        public ActionResult StartTransferEstimation(string hashedAccountId)
        {
            ViewBag.HashedAccountId = hashedAccountId;
            ViewBag.FromTransfers = true;
            return View("StartEstimation");
        }

        [HttpGet]
        [Route("start-redirect", Name = RouteNames.EstimationStartRedirect)]
        public async Task<ActionResult> RedirectEstimationStart(string hashedAccountId)
        {
            return await _estimationOrchestrator.HasValidApprenticeships(hashedAccountId)
                ? RedirectToAction(nameof(CostEstimation), new { hashedaccountId = hashedAccountId, estimateName = Startup.Constants.DefaultEstimationName })
                : RedirectToAction(nameof(AddApprenticeships), new { hashedAccountId, estimationName = Startup.Constants.DefaultEstimationName });
        }

        [HttpGet]
        [Route("{estimateName}/{apprenticeshipRemoved?}", Name = RouteNames.EstimatedCost)]
        public async Task<ActionResult> CostEstimation(string hashedAccountId, string estimateName, bool? apprenticeshipRemoved)
        {
            var viewModel = await _estimationOrchestrator.CostEstimation(hashedAccountId, estimateName, apprenticeshipRemoved);
            return View(viewModel);
        }

        [HttpGet]
        [Route("{estimationName}/apprenticeship/add", Name = RouteNames.AddApprenticeships)]
        public ActionResult AddApprenticeships(string hashedAccountId, string estimationName)
        {
            var vm = _addApprenticeshipOrchestrator.GetApprenticeshipAddSetup(false);
            vm.IsTransferFunded = "";
            vm.HashedAccountId = hashedAccountId;
            vm.EstimationName = estimationName;

            return View(vm);
        }

        [HttpGet]
        [Route("{estimationName}/apprenticeship/{apprenticeshipsId}/EditApprenticeships", Name = RouteNames.EditApprenticeships)]
        public async Task<ActionResult> EditApprenticeships(string hashedAccountId, string estimationName, string apprenticeshipsId)
        {
            var model = await _estimationOrchestrator.EditApprenticeshipModel(hashedAccountId, apprenticeshipsId, estimationName);
            
            return View("AddApprenticeships",model);
        }

        [HttpPost]
        [Route("{estimationName}/apprenticeship/add", Name = RouteNames.SaveApprenticeship)]
        public async Task<ActionResult> Save(AddEditApprenticeshipsViewModel vm, string hashedAccountId, string estimationName)
        {
            var viewModel = await _addApprenticeshipOrchestrator.UpdateAddApprenticeship(vm);

                var result = vm.ValidateAdd(vm);//TODO this used to call ValidateAdd

                foreach (var r in result)
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
        [Route("{estimationName}/apprenticeship/cancel", Name = RouteNames.CancelAddApprenticeship)]
        public ActionResult Cancel(string hashedAccountId, string estimationName)
        { 
            return RedirectToAction(nameof(CostEstimation), new { hashedaccountId = hashedAccountId, estimateName = estimationName });
        }


        [HttpPost]
        [Route("{estimationName}/apprenticeship/course", Name = RouteNames.GetCourseInfo)]
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
        [Route("{estimationName}/apprenticeship/{id}/ConfirmRemoval", Name = RouteNames.ConfirmRemoval)]
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
        [Route("{estimationName}/apprenticeship/{id}/remove", Name = RouteNames.RemoveApprenticeships)]
        public async Task<ActionResult> RemoveApprenticeships(RemoveApprenticeshipViewModel viewModel, string hashedAccountId, string estimationName, string id)
        {
            if (viewModel.ConfirmedDeletion.HasValue && viewModel.ConfirmedDeletion.Value)
            {
                await _addApprenticeshipOrchestrator.RemoveApprenticeship(hashedAccountId, id);
                return await _estimationOrchestrator.HasValidApprenticeships(hashedAccountId)
                ? RedirectToAction(nameof(CostEstimation), new { hashedaccountId = hashedAccountId, estimateName = Startup.Constants.DefaultEstimationName, apprenticeshipRemoved = true })
                : RedirectToAction(nameof(StartEstimation), new { hashedaccountId = hashedAccountId });
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