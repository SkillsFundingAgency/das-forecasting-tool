using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Web.Extensions;
using SFA.DAS.Forecasting.Web.Orchestrators.Exceptions;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Web.Orchestrators.Estimations
{
    public class AddApprenticeshipOrchestrator : IAddApprenticeshipOrchestrator
    {
        private readonly IHashingService _hashingService;
        private readonly IAccountEstimationRepository _accountEstimationRepository;
        private readonly ILog _logger;
        private readonly IApprenticeshipCourseDataService _apprenticeshipCourseService;

        public AddApprenticeshipOrchestrator(
            IHashingService hashingService, 
            IAccountEstimationRepository accountEstimationRepository, 
            IApprenticeshipCourseDataService apprenticeshipCourseService,
            ILog logger)
        {
            _hashingService = hashingService;
            _accountEstimationRepository = accountEstimationRepository;
            _apprenticeshipCourseService = apprenticeshipCourseService;
            _logger = logger;
        }

        public AddApprenticeshipViewModel GetApprenticeshipAddSetup()
        {
            var courses = _apprenticeshipCourseService.GetAllStandardApprenticeshipCourses();
            var result = new AddApprenticeshipViewModel { Courses = courses };
            return result;
        }

        public async Task StoreApprenticeship(AddApprenticeshipViewModel vm, string hashedAccountId, string estimationName)
        {
            var accountEstimation = await GetAccountEstimation(hashedAccountId);
            accountEstimation.AddVirtualApprenticeship(vm.Course.Id, vm.Course.Title, vm.Course.Level,
                vm.StartDateMonth, vm.StartDateYear,
                vm.NumberOfApprentices, vm.TotalInstallments,
                vm.TotalCostAsString.ToDecimal(), Models.Payments.FundingSource.Transfer);

            _logger.Debug($"Storing Apprenticeship for account {hashedAccountId}, estimation name: {estimationName}, Course: {vm.Course}");
            await _accountEstimationRepository.Store(accountEstimation);
        }

        public async Task<AddApprenticeshipViewModel> UpdateAddApprenticeship(AddApprenticeshipViewModel viewModel)
        {
            var course = (!int.TryParse(viewModel.CourseId, out int courseId) && courseId < 1)
                ? null
                : await _apprenticeshipCourseService.GetApprenticeshipCourse(viewModel.CourseId);

            var totalCostAsString = (decimal.TryParse(viewModel.TotalCostAsString, out decimal result))
                ? result.FormatValue()
                : viewModel.TotalCostAsString = string.Empty;

            viewModel.Course = course;
            viewModel.TotalCostAsString = totalCostAsString;

            return viewModel;
        }

        public async Task<CourseViewModel> GetCourse(string courseId)
        {
            var course = await _apprenticeshipCourseService.GetApprenticeshipCourse(courseId);
            return new CourseViewModel
            {
                CourseId = courseId,
                NumberOfMonths = course.Duration,
                FundingPeriods = course.FundingPeriods
                                    .Select(m =>
                                        new FundingPeriodViewModel
                                        {
                                            FromDate = m.EffectiveFrom,
                                            ToDate = m.EffectiveTo,
                                            FundingCap = m.FundingCap
                                        })
                                    .ToList()
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

        public List<ApprenticeshipCourse> GetStandardCourses()
        {
            return _apprenticeshipCourseService.GetAllStandardApprenticeshipCourses();
        }
    }
}