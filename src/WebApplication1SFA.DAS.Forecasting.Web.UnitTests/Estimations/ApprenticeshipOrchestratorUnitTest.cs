using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using AutoMoq;
using SFA.DAS.Forecasting.Application.Estimations.Services;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Domain.Estimations.Validation.VirtualApprenticeships;
using SFA.DAS.Forecasting.Domain.Shared.Validation;
using SFA.DAS.HashingService;
using SFA.DAS.Forecasting.Web.Orchestrators.Estimations;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.Forecasting.Web.Orchestrators.Exceptions;

namespace SFA.DAS.Forecasting.Web.UnitTests.Estimations
{
    [TestFixture]
    public class ApprenticeshipOrchestratorUnitTest
    {

        private ApprenticeshipOrchestrator _apprenticeshipOrchestrator;

        private AutoMoqer _autoMoq;

        private const string HashedAccountId = "VT6098";
        private const string EstimationName = "default";
        private const long AccountId = 12345;
        private const string VirtualApprenticeshipId = "10000ABC";
        private ApprenticeshipToAdd _apprenticeshipToAdd;
        private string _courseId;
        private int? _apprenticesCount;
        private int? _numberOfMonths;
        private int? _startYear;
        private int? _startMonth;
        private ApprenticeshipCourse _apprenticeshipCourse;
        private string _courseTitle;
        private int _level;
        private decimal? _totalCost;
        private List<ApprenticeshipCourse> _apprenticeshipCourses;
        private AddApprenticeshipViewModel _addApprenticeshipViewModel;
        private AddApprenticeshipValidationDetail _addApprenticeshipValidationDetail;

        [SetUp]
        public void Setup()
        {
            _autoMoq = new AutoMoqer();

            var model = new AccountEstimationModel
            {
                Id = Guid.NewGuid().ToString("N"),
                Apprenticeships = new List<VirtualApprenticeship>
                {
                    new VirtualApprenticeship
                    {
                        Id = VirtualApprenticeshipId,
                        CourseTitle = "Wood work",
                        Level = 2,
                        ApprenticesCount = 5
                    }
                },
                EmployerAccountId = AccountId,
                EstimationName = "default"
            };
            _autoMoq.SetInstance(model);

            _courseTitle = "Seafaring Level 2";
            _level = 2;
            _courseId = "ABC";
            _totalCost = 1234;

            _addApprenticeshipValidationDetail = new AddApprenticeshipValidationDetail();

            _apprenticeshipCourses = new List<ApprenticeshipCourse>
            {
                new ApprenticeshipCourse
                {
                    Duration = 12,
                    FundingCap = 7000,
                    Id = _courseId,
                    Level = _level,
                    Title = _courseTitle
                }
            };

            _apprenticesCount = 5;
            _numberOfMonths = 12;
            _startYear = DateTime.Now.Year;
            _startMonth = 12;


            _apprenticeshipCourse = new ApprenticeshipCourse { Id = _courseId, Title = _courseTitle, Level = _level };
            _apprenticeshipToAdd = new ApprenticeshipToAdd
            {
                ApprenticesCount = _apprenticesCount,
                NumberOfMonths = _numberOfMonths,
                StartYear = _startYear,
                StartMonth = _startMonth,
                TotalCost = _totalCost,
                AppenticeshipCourse = _apprenticeshipCourse,
                CourseId = _courseId
            };

            _addApprenticeshipViewModel = new AddApprenticeshipViewModel
            {
                ApprenticeshipToAdd = _apprenticeshipToAdd,
                AvailableApprenticeships = null,
                Name = ""
            };

            _autoMoq.GetMock<IHashingService>()
                    .Setup(o => o.DecodeValue(HashedAccountId))
                    .Returns(AccountId);

            _autoMoq.GetMock<IAccountEstimationRepository>()
                    .Setup(x => x.Get(It.IsAny<long>()))
                    .Returns(Task.FromResult(_autoMoq.Resolve<AccountEstimation>()));

            _autoMoq.GetMock<IApprenticeshipCourseService>()
                .Setup(x => x.GetApprenticeshipCourses())
                .Returns(_apprenticeshipCourses);
            
            _autoMoq.GetMock<IApprenticeshipCourseService>()
                .Setup(x => x.GetApprenticeshipCourse(_courseId))
                .Returns(_apprenticeshipCourse);

            _autoMoq.GetMock<IVirtualApprenticeshipAddValidator>()
                .Setup(x => x.GetCleanValidationDetail())
                .Returns(_addApprenticeshipValidationDetail);
          

            _apprenticeshipOrchestrator = _autoMoq.Resolve<ApprenticeshipOrchestrator>();

        }

        [Test]
        public async Task ConfirmRemovalShouldReturnValidViewModel()
        {
            //Act
            var confirmRemovalViewModel = await _apprenticeshipOrchestrator.GetVirtualApprenticeshipsForRemoval(HashedAccountId, VirtualApprenticeshipId, EstimationName);
            // Assert
            _autoMoq.Verify<IHashingService>(o => o.DecodeValue(HashedAccountId));
            _autoMoq.Verify<IAccountEstimationRepository>(o => o.Get(It.IsAny<long>()));

            confirmRemovalViewModel.Should().NotBeNull();
            confirmRemovalViewModel.Should().BeOfType<RemoveApprenticeshipViewModel>();
        }

        [Test]
        public void ConfirmRemovalShouldThrowExceptionIfApprenticeshipIsAlreadyRemoved()
        {
            // Arrange
            _autoMoq.Resolve<AccountEstimationModel>().Apprenticeships = null;

            //Act
            Func<Task> action = async () => { await _apprenticeshipOrchestrator.GetVirtualApprenticeshipsForRemoval(HashedAccountId, VirtualApprenticeshipId, EstimationName); };

            action.ShouldThrow<ApprenticeshipAlreadyRemovedException>();
        }


        [Test]
        public async Task WhenRetrievingGetApprenticeshipAddSetupItShouldCallCourseServiceGetCourses()
        {
            var addApprenticeshipViewModel = await _apprenticeshipOrchestrator.GetApprenticeshipAddSetup();
            _autoMoq.Verify<IHashingService>(o => o.DecodeValue(HashedAccountId), Times.Never());
            _autoMoq.Verify<IApprenticeshipCourseService>(o => o.GetApprenticeshipCourses());
            _autoMoq.Verify<IVirtualApprenticeshipAddValidator>(o => o.GetCleanValidationDetail());
        }


        [Test]
        public async Task WhenRetrievingGetApprenticeshipAddSetupItShouldReturnExpectedViewModel()
        {
            var addApprenticeshipViewModel = await _apprenticeshipOrchestrator.GetApprenticeshipAddSetup();
            addApprenticeshipViewModel.Should().NotBeNull();
            addApprenticeshipViewModel.Name.Should().NotBeNull();
            addApprenticeshipViewModel.ApprenticeshipToAdd.CourseId.Should().BeNull();
            addApprenticeshipViewModel.ApprenticeshipToAdd.ShouldBeEquivalentTo(new ApprenticeshipToAdd());
            addApprenticeshipViewModel.AvailableApprenticeships.ShouldBeEquivalentTo(_apprenticeshipCourses);
            addApprenticeshipViewModel.AddApprenticeshipValidationDetail.ShouldBeEquivalentTo(_addApprenticeshipValidationDetail);
        }

        [Test]
        public void WhenStoringTheApprenticeshipItShouldStoreWithSuccessfulValidation()
        {

            var validationResults = new List<ValidationResult>();

            _autoMoq.GetMock<IVirtualApprenticeshipValidator>()
                .Setup(x => x.Validate(It.IsAny<VirtualApprenticeship>()))
                .Returns(validationResults);

            _apprenticeshipOrchestrator.StoreApprenticeship(_addApprenticeshipViewModel, HashedAccountId, EstimationName);
            _autoMoq.Verify<IHashingService>(o => o.DecodeValue(HashedAccountId), Times.Once());
            _autoMoq.Verify<IAccountEstimationRepository>(o => o.Get(It.IsAny<long>()));
            _autoMoq.Verify<IVirtualApprenticeshipValidator>(o => o.Validate(It.IsAny<VirtualApprenticeship>()));
            _autoMoq.Verify<IAccountEstimationRepository>(o => o.Store(It.IsAny<AccountEstimation>()));
        }

        [Test]
        public void WhenValidatingTheApprenticeshipWithCourseItShouldCallValidatorFunctions()
        {
            _autoMoq.GetMock<IVirtualApprenticeshipAddValidator>()
                .Setup(x => x.ValidateDetails(It.IsAny<ApprenticeshipToAdd>()))
                .Returns(_addApprenticeshipValidationDetail);

            var vm = _apprenticeshipOrchestrator.ValidateAddApprenticeship(_addApprenticeshipViewModel);
            _autoMoq.Verify<IVirtualApprenticeshipAddValidator>(o => o.GetCleanValidationDetail(), Times.Once());
            _autoMoq.Verify<IVirtualApprenticeshipAddValidator>(o => o.ValidateDetails(It.IsAny<ApprenticeshipToAdd>()), Times.Once());
            _autoMoq.Verify<IApprenticeshipCourseService>(o => o.GetApprenticeshipCourse(It.IsAny<string>()), Times.Once());       
        }

        [Test]
        public void WhenValidatingTheApprenticeshipWithoutCourseItShouldCallValidatorFunctions()
        {
            var vmViewModel = new AddApprenticeshipViewModel
            {
                ApprenticeshipToAdd = _apprenticeshipToAdd,
                AvailableApprenticeships = null,
                Name = ""
            };

            vmViewModel.ApprenticeshipToAdd.CourseId = null;
            vmViewModel.ApprenticeshipToAdd.AppenticeshipCourse = null;


            _autoMoq.GetMock<IVirtualApprenticeshipAddValidator>()
                .Setup(x => x.ValidateDetails(It.IsAny<ApprenticeshipToAdd>()))
                .Returns(_addApprenticeshipValidationDetail);

            var vm = _apprenticeshipOrchestrator.ValidateAddApprenticeship(vmViewModel);
            _autoMoq.Verify<IVirtualApprenticeshipAddValidator>(o => o.GetCleanValidationDetail(), Times.Once());
            _autoMoq.Verify<IVirtualApprenticeshipAddValidator>(o => o.ValidateDetails(It.IsAny<ApprenticeshipToAdd>()), Times.Once());
            _autoMoq.Verify<IApprenticeshipCourseService>(o => o.GetApprenticeshipCourse(It.IsAny<string>()), Times.Never());
        }


        [TestCase(10000,1,5000,5000)]
        [TestCase(5000, 3, 14000, 14000)]
        [TestCase(5000, 3, 16000, 15000)]
        [TestCase(null, 7, 16000, 16000)]
        [TestCase(5000, null, 16000, 16000)]
        [TestCase(null, null, 16000, 16000)]
        [TestCase(5000, 3, null, null)]
        public void WhenAdjustingTotalCostGiveExpectedResults(decimal? fundingCap,int? noOfApprenticeships, decimal? totalCost, decimal? expectedTotalCost)
        {
            ApprenticeshipCourse apprenticeshipCourse = null;

            if (fundingCap.HasValue)
                apprenticeshipCourse = new ApprenticeshipCourse {FundingCap = fundingCap.Value};

            var model = new AddApprenticeshipViewModel
            {
                ApprenticeshipToAdd = new ApprenticeshipToAdd
                {
                    AppenticeshipCourse = apprenticeshipCourse,
                    ApprenticesCount = noOfApprenticeships,
                    TotalCost = totalCost
                }
            };


           var vm = _apprenticeshipOrchestrator.AdjustTotalCostApprenticeship(model);
            vm.ApprenticeshipToAdd.TotalCost.Should().Be(expectedTotalCost);


        }
    }
}
