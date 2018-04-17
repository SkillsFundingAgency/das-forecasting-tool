using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Estimations.Services;
using SFA.DAS.Forecasting.Domain.Estimations;
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
        private readonly IApprenticeshipCourseService _apprenticeshipCourseService;

        public ApprenticeshipOrchestrator(IHashingService hashingService, IAccountEstimationRepository accountEstimationRepository, IApprenticeshipCourseService apprenticeshipCourseService)
        {
            _hashingService = hashingService;
            _accountEstimationRepository = accountEstimationRepository;
            _apprenticeshipCourseService = apprenticeshipCourseService;
        }


        public async Task<AddApprenticeshipViewModel> GetApprenticeshipAddSetup()
        {
            var result = new AddApprenticeshipViewModel
            {
                Name = "Add Apprenticeships",
                ApprenticeshipToAdd = new ApprenticeshipToAdd(),
                AvailableApprenticeships = _apprenticeshipCourseService.GetApprenticeshipCourses()
            };

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