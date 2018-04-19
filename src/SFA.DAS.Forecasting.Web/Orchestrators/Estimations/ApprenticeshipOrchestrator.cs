
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Domain.Estimations.Validation.VirtualApprenticeships;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Web.Orchestrators.Exceptions;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.Web.Orchestrators.Estimations
{
    public class ApprenticeshipOrchestrator : IApprenticeshipOrchestrator
    {
        private readonly IHashingService _hashingService;
        private readonly IAccountEstimationRepository _accountEstimationRepository;
        private readonly IVirtualApprenticeshipAddValidator _addValidator;
        private readonly IApprenticeshipCourseDataService _apprenticeshipCourseService;

        public ApprenticeshipOrchestrator(IHashingService hashingService, IAccountEstimationRepository accountEstimationRepository, IApprenticeshipCourseDataService apprenticeshipCourseService, IVirtualApprenticeshipAddValidator addValidator)
        {
            _hashingService = hashingService;
            _accountEstimationRepository = accountEstimationRepository;
            _apprenticeshipCourseService = apprenticeshipCourseService;
            _addValidator = addValidator;
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
                    AddApprenticeshipValidationDetail = _addValidator.GetCleanValidationDetail()
                    
                    
        };
            return result;
        }

        public async Task StoreApprenticeship(AddApprenticeshipViewModel vm, string hashedAccountId, string estimationName)
        {
            var apprenticeshipToAdd = vm.ApprenticeshipToAdd;
            var course = vm.ApprenticeshipToAdd.AppenticeshipCourse;

            var courseId = course.Id;
            var courseTitle = course.Title;
            var level = course.Level;

            var accountEstimation = await GetAccountEstimation(hashedAccountId);
            accountEstimation.AddVirtualApprenticeship(courseId,courseTitle,level,apprenticeshipToAdd.StartMonth.GetValueOrDefault(),
                apprenticeshipToAdd.StartYear.GetValueOrDefault(),apprenticeshipToAdd.ApprenticesCount.GetValueOrDefault(),
                apprenticeshipToAdd.NumberOfMonths.GetValueOrDefault(),apprenticeshipToAdd.TotalCost.GetValueOrDefault());
         
            await _accountEstimationRepository.Store(accountEstimation);
        }
      
        public async Task<AddApprenticeshipViewModel> ValidateAddApprenticeship(AddApprenticeshipViewModel vm)
        {
            var apprenticeshipDetailsToPersist = vm.ApprenticeshipToAdd;

            var viewModel = GetApprenticeshipAddSetup();
            viewModel.ApprenticeshipToAdd = apprenticeshipDetailsToPersist;

            if (viewModel.ApprenticeshipToAdd.CourseId is null)
            {
                viewModel.ApprenticeshipToAdd.AppenticeshipCourse = null;
            }
            else
            {
                viewModel.ApprenticeshipToAdd.AppenticeshipCourse =
                   await _apprenticeshipCourseService.GetApprenticeshipCourse(viewModel.ApprenticeshipToAdd.CourseId);
            }

            viewModel.AddApprenticeshipValidationDetail = _addValidator.ValidateDetails(viewModel.ApprenticeshipToAdd);

            viewModel = AdjustTotalCostApprenticeship(viewModel);

            return viewModel;
        }

        public AddApprenticeshipViewModel AdjustTotalCostApprenticeship(AddApprenticeshipViewModel viewModel)
        {
            var fundingCap = viewModel.ApprenticeshipToAdd?.AppenticeshipCourse?.FundingCap;
            var noOfApprenticeships = viewModel.ApprenticeshipToAdd?.ApprenticesCount;

                if (fundingCap.HasValue &&
            viewModel.ApprenticeshipToAdd.TotalCost.HasValue &&
            noOfApprenticeships.HasValue && viewModel.ApprenticeshipToAdd.TotalCost > (fundingCap* noOfApprenticeships))
            {
                viewModel.ApprenticeshipToAdd.TotalCost = fundingCap* noOfApprenticeships;
            }

            return viewModel;
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