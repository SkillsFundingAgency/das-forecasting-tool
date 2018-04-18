
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
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
        private readonly IApprenticeshipCourseDataService _apprenticeshipCourseService;

        public ApprenticeshipOrchestrator(IHashingService hashingService, IAccountEstimationRepository accountEstimationRepository, IApprenticeshipCourseDataService apprenticeshipCourseService)
        {
            _hashingService = hashingService;
            _accountEstimationRepository = accountEstimationRepository;
            _apprenticeshipCourseService = apprenticeshipCourseService;
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
                    .ToList()
            };
            return result;
        }

        public async Task StoreApprenticeship(AddApprenticeshipViewModel vm, string hashedAccountId, string estimationName)
        {
            var courseId = vm.CourseId;
            var apprenticeshipToAdd = vm.ApprenticeshipToAdd;
            var course = await _apprenticeshipCourseService.GetApprenticeshipCourse(vm.CourseId);
            var courseTitle = course.Title;
            var level = course.Level;

            var accountEstimation = await GetAccountEstimation(hashedAccountId);
            accountEstimation.AddVirtualApprenticeship(courseId,courseTitle,level,apprenticeshipToAdd.StartMonth.GetValueOrDefault(),
                apprenticeshipToAdd.StartYear.GetValueOrDefault(),apprenticeshipToAdd.ApprenticesCount.GetValueOrDefault(),
                apprenticeshipToAdd.NumberOfMonths.GetValueOrDefault(),apprenticeshipToAdd.TotalCost.GetValueOrDefault());

            await _accountEstimationRepository.Store(accountEstimation);
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