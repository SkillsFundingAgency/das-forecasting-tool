using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Forecasting.Application.Estimations.Services;
using SFA.DAS.Forecasting.Domain.Estimations.Validation.VirtualApprenticeships;
using SFA.DAS.Forecasting.Domain.Shared.Validation;
using SFA.DAS.Forecasting.Web.Attributes;
using SFA.DAS.Forecasting.Web.Authentication;
using SFA.DAS.Forecasting.Web.Mvc;
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
        private readonly IVirtualApprenticeshipAddValidator _addValidator;
        private readonly IApprenticeshipCourseService _apprenticeshipCourseService;
        public EstimationController(IEstimationOrchestrator estimationOrchestrator, IApprenticeshipOrchestrator apprenticeshipOrchestrator, IMembershipService membershipService, IVirtualApprenticeshipAddValidator addValidator, IApprenticeshipCourseService apprenticeshipCourseService)
        {
            _estimationOrchestrator = estimationOrchestrator;
            _membershipService = membershipService;
            _addValidator = addValidator;
            _apprenticeshipCourseService = apprenticeshipCourseService;
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
            var vm = await _apprenticeshipOrchestrator.GetApprenticeshipAddSetup();
            vm.AddApprenticeshipValidationDetail = _addValidator.GetCleanValidationDetail();

            return View(vm);
        }


        [HttpPost]
        [Route("{estimationName}/apprenticeship/add", Name = "SaveApprenticeship")]
        public async Task<ActionResult> Save(AddApprenticeshipViewModel vm, string hashedAccountId, string estimationName)
        {
            var apprenticeshipDetailsToPersist = vm.ApprenticeshipToAdd;

            vm = await _apprenticeshipOrchestrator.GetApprenticeshipAddSetup();
            vm.ApprenticeshipToAdd = apprenticeshipDetailsToPersist;

            if (vm.ApprenticeshipToAdd.CourseId is null)
            {
                vm.ApprenticeshipToAdd.AppenticeshipCourse = null;
            }
            else
            {
                vm.ApprenticeshipToAdd.AppenticeshipCourse = _apprenticeshipCourseService.GetApprenticeshipCourse(vm.ApprenticeshipToAdd.CourseId);
            }

            vm.AddApprenticeshipValidationDetail = _addValidator.ValidateDetails(vm.ApprenticeshipToAdd);

            if (!vm.AddApprenticeshipValidationDetail.IsValid)
            {
                return View("AddApprenticeships",vm);
            }

            await _apprenticeshipOrchestrator.StoreApprenticeship(vm, hashedAccountId, estimationName);

            return RedirectToAction(nameof(CostEstimation), new { hashedaccountId = hashedAccountId, estimateName = estimationName });

        }

        [HttpGet]
        [Route("{estimationName}/apprenticeship/{id}/ConfirmRemoval", Name = "ConfirmRemoval")]
        public async Task<ActionResult> ConfirmApprenticeshipsRemoval(string hashedAccountId, string estimationName, string id)
        {
            try
            {
                var vm = await _apprenticeshipOrchestrator.GetVirtualApprenticeshipsForRemoval(hashedAccountId, id);
                return View(vm);
            }
            catch (ApprenticeshipAlreadyRemovedException)
            {
                return RedirectToAction(nameof(CostEstimation), new { hashedaccountId = hashedAccountId, estimateName = estimationName, apprenticeshipRemoved = true });
            }

        }


        [HttpPost]
        [Route("{estimationName}/apprenticeship/{id}/remove", Name = "RemoveApprenticeships")]
        public async Task<ActionResult> RemoveApprenticeships(string hashedAccountId, string estimationName, string id)
        {
            await _apprenticeshipOrchestrator.RemoveApprenticeship(hashedAccountId, id);
            return RedirectToAction(nameof(CostEstimation), new { hashedaccountId = hashedAccountId, estimateName = estimationName, apprenticeshipRemoved = true });
        }

    }

}