using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Application.Estimations.Validation;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Domain.Shared.Validation;
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
        private readonly IAddApprenticeshipValidator _validator;
        private readonly ILog _logger;
        private readonly IApprenticeshipCourseDataService _apprenticeshipCourseService;

        public AddApprenticeshipOrchestrator(
            IHashingService hashingService, IAccountEstimationRepository accountEstimationRepository, IApprenticeshipCourseDataService apprenticeshipCourseService, IAddApprenticeshipValidator validator, ILog logger)
        {
            _hashingService = hashingService;
            _accountEstimationRepository = accountEstimationRepository;
            _apprenticeshipCourseService = apprenticeshipCourseService;
            _validator = validator;
            _logger = logger;
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
                apprenticeshipToAdd.StartMonth.GetValueOrDefault(), apprenticeshipToAdd.StartYear.GetValueOrDefault(),
                apprenticeshipToAdd.ApprenticesCount.GetValueOrDefault(), apprenticeshipToAdd.NumberOfMonths.GetValueOrDefault(),
                apprenticeshipToAdd.TotalCost.GetValueOrDefault(), Models.Payments.FundingSource.Transfer);

            _logger.Debug($"Storing Apprenticeship for account {hashedAccountId}, estimation name: {estimationName}, Course: {course}");
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

            AdjustNumberOfMonthsApprenticeship(viewModel);

            viewModel.ValidationResults = _validator.ValidateApprenticeship(viewModel.ApprenticeshipToAdd);

            AdjustTotalCostApprenticeship(viewModel);

            return viewModel;
        }

        private static void SetTotalCostWithCommas(ApprenticeshipToAdd apprenticeshipDetailsToPersist)
        {

            if (decimal.TryParse(apprenticeshipDetailsToPersist.TotalCostAsString, out decimal result))
            {
                apprenticeshipDetailsToPersist.TotalCost = Math.Round(result, 0, MidpointRounding.AwayFromZero);
                apprenticeshipDetailsToPersist.TotalCostAsString = result.FormatValue();
            }
            else
            {
                apprenticeshipDetailsToPersist.TotalCostAsString = string.Empty;
                apprenticeshipDetailsToPersist.TotalCost = null;
            }
        }

        private AddApprenticeshipViewModel ResetViewModelDetails(AddApprenticeshipViewModel vm)
        {
            var apprenticeshipDetailsToPersist = vm.ApprenticeshipToAdd;
            SetTotalCostWithCommas(apprenticeshipDetailsToPersist);
            var previousCourseId = vm.PreviousCourseId;
            var viewModel = GetApprenticeshipAddSetup();
            viewModel.PreviousCourseId = previousCourseId;
            viewModel.ApprenticeshipToAdd = apprenticeshipDetailsToPersist;
            return viewModel;
        }
        private void AdjustNumberOfMonthsApprenticeship(AddApprenticeshipViewModel viewModel)
        {
            var apprenticeToAdd = viewModel.ApprenticeshipToAdd;
            if (apprenticeToAdd.AppenticeshipCourse != null && viewModel.PreviousCourseId != apprenticeToAdd.CourseId)
            {
                viewModel.ApprenticeshipToAdd.NumberOfMonths = apprenticeToAdd.AppenticeshipCourse.Duration;
            }
        }


        public void AdjustTotalCostApprenticeship(AddApprenticeshipViewModel viewModel)
        {
            var apprenticeToAdd = viewModel.ApprenticeshipToAdd;

            if (apprenticeToAdd?.CalculatedTotalCap != null &&
                    (apprenticeToAdd.TotalCost.HasValue == false || apprenticeToAdd.TotalCost > apprenticeToAdd.CalculatedTotalCap))
            {
                viewModel.ApprenticeshipToAdd.TotalCost = apprenticeToAdd.CalculatedTotalCap;
            }
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