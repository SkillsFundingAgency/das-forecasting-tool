using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Estimations.Services;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.Web.Orchestrators
{
    public class ApprenticeshipOrchestrator : IApprenticeshipOrchestrator
    {
        private readonly IHashingService _hashingService;
        private readonly Mapper _mapper;
        private readonly IAccountEstimationRepository _accountEstimationRepository;
        private readonly IApprenticeshipCourseService _apprenticeshipCourseService;

        public ApprenticeshipOrchestrator(IHashingService hashingService, Mapper mapper, IAccountEstimationRepository accountEstimationRepository, IApprenticeshipCourseService apprenticeshipCourseService)
        {
            _hashingService = hashingService;
            _mapper = mapper;
            _accountEstimationRepository = accountEstimationRepository;
            _apprenticeshipCourseService = apprenticeshipCourseService;
        }


        public async Task<ApprenticeshipAddViewModel> GetApprenticeshipAddSetup(string hashedAccountId, string estimationName)
        {
            var result = new ApprenticeshipAddViewModel
            {
                Name = "Add Apprenticeships",
                HashedAccountId = hashedAccountId,
                EstimationName = estimationName,
                ApprenticeshipToAdd = new ApprenticeshipToAdd(),
                AvailableApprenticeships = _apprenticeshipCourseService.GetApprenticeshipCourses()
            };

            return await Task.FromResult(result);
        }

     

        public void StoreApprenticeship(ApprenticeshipAddViewModel vm)
        {
            var hashedAccountId = vm.HashedAccountId;
            var estimationName = vm.EstimationName;
            // TODO: estimationName ignored for now as 'default' is assumed, but needs wiring in to the repo.Get call at some point

            var courseId = vm.CourseId;

            var apprenticeshipToAdd = vm.ApprenticeshipToAdd;
            var course = _apprenticeshipCourseService.GetApprenticeshipCourse(vm.CourseId);
            var courseTitle = course.Title;
            var level = course.Level;

            var accountId = _hashingService.DecodeValue(hashedAccountId);

        
            //var currentEstimationDetails = _accountEstimationRepository.Get(accountId).ConfigureAwait(false);

            var task = Task.Run(async () => await _accountEstimationRepository.Get(accountId).ConfigureAwait(false));


            var accountEstimation = task.Result;


            accountEstimation.AddVirtualApprenticeship(courseId,
                                                        courseTitle,
                                                        level,
                                                        apprenticeshipToAdd.StartMonth,
                                                        apprenticeshipToAdd.StartYear,
                                                        apprenticeshipToAdd.ApprenticesCount,
                                                        apprenticeshipToAdd.NumberOfMonths,
                                                        apprenticeshipToAdd.TotalCost);

            _accountEstimationRepository.Store(accountEstimation);
        }
    }
}