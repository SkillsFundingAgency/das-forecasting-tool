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
                TotalCost = _totalCost
            };

            _addApprenticeshipViewModel = new AddApprenticeshipViewModel
            {
                ApprenticeshipToAdd = _apprenticeshipToAdd,
                AvailableApprenticeships = null,
                //CourseId = _courseId,
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

            _apprenticeshipOrchestrator = _autoMoq.Resolve<ApprenticeshipOrchestrator>();

        }

        [Test]
        public async Task ConfirmRemovalShouldReturnValidViewModel()
        {
            //Act
            var confirmRemovalViewModel = await _apprenticeshipOrchestrator.GetVirtualApprenticeshipsForRemoval(HashedAccountId, VirtualApprenticeshipId);
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
            Func<Task> action = async () => { await _apprenticeshipOrchestrator.GetVirtualApprenticeshipsForRemoval(HashedAccountId, VirtualApprenticeshipId); };

            action.ShouldThrow<ApprenticeshipAlreadyRemovedException>();
        }


        [Test]
        public async Task WhenRetrievingGetApprenticeshipAddSetupItShouldCallCourseServiceGetCourses()
        {
            var addApprenticeshipViewModel = await _apprenticeshipOrchestrator.GetApprenticeshipAddSetup();
            _autoMoq.Verify<IHashingService>(o => o.DecodeValue(HashedAccountId), Times.Never());
            _autoMoq.Verify<IApprenticeshipCourseService>(o => o.GetApprenticeshipCourses());
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
        }
    }
}
