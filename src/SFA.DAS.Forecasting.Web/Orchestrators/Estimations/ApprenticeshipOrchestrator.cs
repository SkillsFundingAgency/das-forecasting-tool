using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Estimations.Services;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Domain.Estimations.Validation.VirtualApprenticeships;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Web.Orchestrators.Exceptions;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.Web.Orchestrators.Estimations
{
    public class ApprenticeshipOrchestrator : IApprenticeshipOrchestrator
    {
        private readonly IHashingService _hashingService;
        private readonly IAccountEstimationRepository _accountEstimationRepository;
        private readonly IVirtualApprenticeshipAddValidator _addValidator;
        private readonly IApprenticeshipCourseService _apprenticeshipCourseService;

        public ApprenticeshipOrchestrator(IHashingService hashingService, IAccountEstimationRepository accountEstimationRepository, IApprenticeshipCourseService apprenticeshipCourseService, IVirtualApprenticeshipAddValidator addValidator)
        {
            _hashingService = hashingService;
            _accountEstimationRepository = accountEstimationRepository;
            _apprenticeshipCourseService = apprenticeshipCourseService;
            _addValidator = addValidator;
        }


        public async Task<AddApprenticeshipViewModel> GetApprenticeshipAddSetup()
        {
            var result = new AddApprenticeshipViewModel
            {
                Name = "Add Apprenticeships",
                ApprenticeshipToAdd = new ApprenticeshipToAdd(),
                AvailableApprenticeships = _apprenticeshipCourseService.GetApprenticeshipCourses()
            };

            result.AddApprenticeshipValidationDetail = _addValidator.GetCleanValidationDetail();

            return await Task.FromResult(result);
        }

        public async Task StoreApprenticeship(AddApprenticeshipViewModel vm, string hashedAccountId, string estimationName)
        {
            var apprenticeshipToAdd = vm.ApprenticeshipToAdd;
            var course = vm.ApprenticeshipToAdd.AppenticeshipCourse;
            var courseId = course.Id;
            var courseTitle = course.Title;
            var level = course.Level;

            var accountEstimation = await GetAccountAutomation(hashedAccountId);

            accountEstimation.AddVirtualApprenticeship(courseId,
                                                        courseTitle,
                                                        level,
                                                        apprenticeshipToAdd.StartMonth.GetValueOrDefault(),
                                                        apprenticeshipToAdd.StartYear.GetValueOrDefault(),
                                                        apprenticeshipToAdd.ApprenticesCount.GetValueOrDefault(),
                                                        apprenticeshipToAdd.NumberOfMonths.GetValueOrDefault(),
                                                        apprenticeshipToAdd.TotalCost.GetValueOrDefault());

            await _accountEstimationRepository.Store(accountEstimation);
        }
      
        public async Task<AddApprenticeshipViewModel> ValidateAddApprenticeship(AddApprenticeshipViewModel vm)
        {
            var apprenticeshipDetailsToPersist = vm.ApprenticeshipToAdd;

            var viewModel = await GetApprenticeshipAddSetup();
            viewModel.ApprenticeshipToAdd = apprenticeshipDetailsToPersist;

            if (viewModel.ApprenticeshipToAdd.CourseId is null)
            {
                viewModel.ApprenticeshipToAdd.AppenticeshipCourse = null;
            }
            else
            {
                viewModel.ApprenticeshipToAdd.AppenticeshipCourse =
                    _apprenticeshipCourseService.GetApprenticeshipCourse(viewModel.ApprenticeshipToAdd.CourseId);
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




        public async Task<RemoveApprenticeshipViewModel> GetVirtualApprenticeshipsForRemoval(string hashedAccountId, string apprenticeshipsId)
        {
            var estimations = await GetAccountAutomation(hashedAccountId);
            var virtualApprenticeships = estimations.FindVirtualApprenticeship(apprenticeshipsId);

            if (virtualApprenticeships == null) throw new ApprenticeshipAlreadyRemovedException();

            var vm = new RemoveApprenticeshipViewModel
            {
                ApprenticeshipId = apprenticeshipsId,
                HashedAccountId = hashedAccountId,
                NumberOfApprentices = virtualApprenticeships.ApprenticesCount,
                CourseTitle = virtualApprenticeships.CourseTitle,
                Level = virtualApprenticeships.Level
            };

            return vm;

        }

        public async Task RemoveApprenticeship(string hashedAccountId, string apprenticeshipId)
        {
            var estimations = await GetAccountAutomation(hashedAccountId);
            estimations?.RemoveVirtualApprenticeship(apprenticeshipId);
            await _accountEstimationRepository.Store(estimations);
        }

        private async Task<AccountEstimation> GetAccountAutomation(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            return await _accountEstimationRepository.Get(accountId);
        }

        
    }
}