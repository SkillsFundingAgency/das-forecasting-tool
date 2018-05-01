using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Application.Estimations.Validation;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Domain.Shared.Validation;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Web.Orchestrators.Exceptions;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.Web.Orchestrators.Estimations
{
    public class AddApprenticeshipOrchestrator : IAddApprenticeshipOrchestrator
    {
        private readonly IHashingService _hashingService;
        private readonly IAccountEstimationRepository _accountEstimationRepository;
        private readonly IAddApprenticeshipValidator _validator;
        private readonly IApprenticeshipCourseDataService _apprenticeshipCourseService;

        public AddApprenticeshipOrchestrator(IHashingService hashingService, IAccountEstimationRepository accountEstimationRepository, IApprenticeshipCourseDataService apprenticeshipCourseService, IAddApprenticeshipValidator validator)
        {
            _hashingService = hashingService;
            _accountEstimationRepository = accountEstimationRepository;
            _apprenticeshipCourseService = apprenticeshipCourseService;
            _validator = validator;
        }

        public AddApprenticeshipViewModel GetApprenticeshipAddSetup()
          {
            var result = new AddApprenticeshipViewModel
            {
                Name = "Add Apprenticeships",
                ApprenticeshipToAdd = new ApprenticeshipToAdd(),
                AvailableApprenticeships = _apprenticeshipCourseService
                    .GetAllStandardApprenticeshipCourses()
                    .OrderBy(course => course.Title)
                    .ToList(),
                ValidationResults = new List<ValidationResult>()     
        };
            return result;
        }

        public async Task StoreApprenticeship(AddApprenticeshipViewModel vm, string hashedAccountId, string estimationName)
        {
            var apprenticeshipToAdd = vm.ApprenticeshipToAdd;
            var course = vm.ApprenticeshipToAdd.AppenticeshipCourse;
         
            var accountEstimation = await GetAccountEstimation(hashedAccountId);
            accountEstimation.AddVirtualApprenticeship(course.Id, course.Title, course.Level, 
                apprenticeshipToAdd.StartMonth.GetValueOrDefault(),apprenticeshipToAdd.StartYear.GetValueOrDefault(),
                apprenticeshipToAdd.ApprenticesCount.GetValueOrDefault(), apprenticeshipToAdd.NumberOfMonths.GetValueOrDefault(),
                apprenticeshipToAdd.TotalCost.GetValueOrDefault());
         
            await _accountEstimationRepository.Store(accountEstimation);
        }
      
        public async Task<AddApprenticeshipViewModel> ValidateAddApprenticeship(AddApprenticeshipViewModel vm)
        {
            
            var viewModel = ResetViewModelDetails(vm);

            if (viewModel.ApprenticeshipToAdd.CourseId is null)
            {
                viewModel.ApprenticeshipToAdd.AppenticeshipCourse = null;
            }
            else
            {
                viewModel.ApprenticeshipToAdd.AppenticeshipCourse =
                   await _apprenticeshipCourseService.GetApprenticeshipCourse(viewModel.ApprenticeshipToAdd.CourseId);
            }

            viewModel = AdjustNumberOfMonthsApprenticeship(viewModel);

            viewModel.ValidationResults = _validator.ValidateApprenticeship(viewModel.ApprenticeshipToAdd);

            viewModel = AdjustTotalCostApprenticeship(viewModel);

            return viewModel;
        }

        private AddApprenticeshipViewModel ResetViewModelDetails(AddApprenticeshipViewModel vm)
        {
            var apprenticeshipDetailsToPersist = vm.ApprenticeshipToAdd;
            var previousCourseId = vm.PreviousCourseId;    
            var viewModel = GetApprenticeshipAddSetup();
            viewModel.ApprenticeshipToAdd = apprenticeshipDetailsToPersist;
            viewModel.PreviousCourseId = previousCourseId;
            return viewModel;
        }

        private AddApprenticeshipViewModel AdjustNumberOfMonthsApprenticeship(AddApprenticeshipViewModel viewModel)
        {
            var apprenticeToAdd = viewModel.ApprenticeshipToAdd;

            if (apprenticeToAdd.AppenticeshipCourse != null 
                && viewModel.PreviousCourseId != apprenticeToAdd.CourseId)
            {
                viewModel.ApprenticeshipToAdd.NumberOfMonths = apprenticeToAdd.AppenticeshipCourse.Duration;
            }

            return viewModel;

        }

        public AddApprenticeshipViewModel AdjustTotalCostApprenticeship(AddApprenticeshipViewModel viewModel)
        {
            var apprenticeToAdd = viewModel.ApprenticeshipToAdd;

            if (apprenticeToAdd is null)
                return viewModel;
            
            if (apprenticeToAdd.CalculatedTotalCap.HasValue &&
                    (apprenticeToAdd.TotalCost.HasValue == false || apprenticeToAdd.TotalCost > apprenticeToAdd.CalculatedTotalCap))
                {
                viewModel.ApprenticeshipToAdd.TotalCost = apprenticeToAdd.CalculatedTotalCap;
                }

            return viewModel;
        }

        public async Task<decimal?> GetFundingCapForCourse(string courseId)
        {
            var course = await _apprenticeshipCourseService.GetApprenticeshipCourse(courseId);
            var res = course.FundingCap;
            return res;
        }

        public async Task<dynamic> GetDefaultNumberOfMonths(string courseId)
        {
            var course = await _apprenticeshipCourseService.GetApprenticeshipCourse(courseId); 

            return new
            {
                NumberOfMonths = course.Duration
            };
        }

        public async Task<RemoveApprenticeshipViewModel> GetVirtualApprenticeshipsForRemoval(string hashedAccountId, string apprenticeshipsId, string estimationName)
        {
            var estimations = await GetAccountEstimation(hashedAccountId);

            var virtualApprenticeships = estimations.FindVirtualApprenticeship(apprenticeshipsId);

            if (virtualApprenticeships == null) throw new ApprenticeshipAlreadyRemovedException();
            var vm = new RemoveApprenticeshipViewModel
            {
                ApprenticeshipId = apprenticeshipsId,
                HashedAccountId = hashedAccountId,
                NumberOfApprentices = virtualApprenticeships.ApprenticesCount,
                CourseTitle = virtualApprenticeships.CourseTitle,
                Level = virtualApprenticeships.Level,
                EstimationName = estimationName
            };

            return vm;
        }

        public async Task RemoveApprenticeship(string hashedAccountId, string apprenticeshipId)
        {
            var estimations = await GetAccountEstimation(hashedAccountId);

            estimations?.RemoveVirtualApprenticeship(apprenticeshipId);
            await _accountEstimationRepository.Store(estimations);
        }

        private async Task<AccountEstimation> GetAccountEstimation(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            return await _accountEstimationRepository.Get(accountId);
        }
    }
}