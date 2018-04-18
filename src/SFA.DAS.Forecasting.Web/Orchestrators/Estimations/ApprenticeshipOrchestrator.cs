using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Application.Estimations.Services;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.Web.Orchestrators.Estimations
{
    public class ApprenticeshipOrchestrator : IApprenticeshipOrchestrator
    {
        private readonly IHashingService _hashingService;
        private readonly Mapper _mapper;
        private readonly IAccountEstimationRepository _accountEstimationRepository;
        private readonly IApprenticeshipCourseDataService _apprenticeshipCourseService;

        public ApprenticeshipOrchestrator(IHashingService hashingService, Mapper mapper, IAccountEstimationRepository accountEstimationRepository, IApprenticeshipCourseDataService apprenticeshipCourseService)
        {
            _hashingService = hashingService;
            _mapper = mapper;
            _accountEstimationRepository = accountEstimationRepository;
            _apprenticeshipCourseService = apprenticeshipCourseService;
        }


        public async Task<AddApprenticeshipViewModel> GetApprenticeshipAddSetup(string hashedAccountId, string estimationName)
        {
            var result = new AddApprenticeshipViewModel
            {
                Name = "Add Apprenticeships",
                ApprenticeshipToAdd = new ApprenticeshipToAdd(),
                AvailableApprenticeships = _apprenticeshipCourseService.GetAllStandardApprenticeshipCourses()
            };

            return await Task.FromResult(result);
        }



        public void StoreApprenticeship(AddApprenticeshipViewModel vm, string hashedAccountId, string estimationName)
        {
            var courseId = vm.CourseId;
            var apprenticeshipToAdd = vm.ApprenticeshipToAdd;
            var course = Task.Run(async () => await _apprenticeshipCourseService.GetApprenticeshipCourse(vm.CourseId).ConfigureAwait(false))
                .Result;
            var courseTitle = course.Title;
            var level = course.Level;

            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var task = Task.Run(async () => await _accountEstimationRepository.Get(accountId).ConfigureAwait(false));


            var accountEstimation = task.Result;


            accountEstimation.AddVirtualApprenticeship(courseId,
                                                        courseTitle,
                                                        level,
                                                        apprenticeshipToAdd.StartMonth.GetValueOrDefault(),
                                                        apprenticeshipToAdd.StartYear.GetValueOrDefault(),
                                                        apprenticeshipToAdd.ApprenticesCount.GetValueOrDefault(),
                                                        apprenticeshipToAdd.NumberOfMonths.GetValueOrDefault(),
                                                        apprenticeshipToAdd.TotalCost.GetValueOrDefault());

            _accountEstimationRepository.Store(accountEstimation);
        }
    }
}