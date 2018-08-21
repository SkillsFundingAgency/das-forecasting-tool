using AutoMoq;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Web.Controllers;
using SFA.DAS.Forecasting.Web.Orchestrators.Estimations;
using SFA.DAS.Forecasting.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Web.UnitTests.Controllers
{
    [TestFixture]
    public class EstimationControllerTests
    {
        private AutoMoqer _moqer;
        private EstimationController _controller;

        [SetUp]
        public void SetUp()
        {
            _moqer = new AutoMoqer();

            var courseElectrician = new ApprenticeshipCourse
            {
                CourseType = ApprenticeshipCourseType.Standard,
                Duration = 23,
                FundingCap = 12000,
                Id = "567",
                Level = 4,
                Title = "Electrician"
            };

            var courseCarpentry =
                new ApprenticeshipCourse
                {
                    CourseType = ApprenticeshipCourseType.Standard,
                    Duration = 36,
                    FundingCap = 9000,
                    Id = "123",
                    Level = 4,
                    Title = "Carpentry"
                };

            var apprenticeshipCourses = new List<ApprenticeshipCourse>
            {
               courseElectrician,
               courseCarpentry,
            };

            _moqer.GetMock<IAddApprenticeshipOrchestrator>()
                .Setup(x => x.GetStandardCourses())
                .Returns(apprenticeshipCourses);

            _controller = _moqer.Resolve<EstimationController>();
        }

        [Test]
        public async Task Apprenticeship_Must_Have_Course()
        {
            var vm = new AddApprenticeshipViewModel
            {
                CourseId = "123",
                Course = null,
                TotalCostAsString = "10"
            };

            _moqer.GetMock<IAddApprenticeshipOrchestrator>()
                .Setup(x => x.UpdateAddApprenticeship(vm))
                .Returns(Task.FromResult(vm));

            _moqer.GetMock<IAddApprenticeshipOrchestrator>()
                .Setup(x => x.GetApprenticeshipAddSetup(false))
                .Returns(new AddApprenticeshipViewModel { Courses = new List<ApprenticeshipCourse>() } );

            await _controller.Save(vm, "ABBA12", "default");
            _controller.ViewData.ModelState.Count
                .Should().Be(1);
        }

        [TestCase("0", true)]
        [TestCase("-5", true)]
        [TestCase("ABBA", true)]
        [TestCase("£10", false)]
        [TestCase("10", false)]
        public async Task Apprenticeship_Must_Have_Total_Cost(string costAsString, bool shouldError)
        {
            var fundingPeriods = new List<FundingPeriod>
            {
                new FundingPeriod{ EffectiveFrom = DateTime.MinValue, EffectiveTo = DateTime.MaxValue, FundingCap = 20 * 1000 }
            };

            var vm = new AddApprenticeshipViewModel
            {
                CourseId = "123",
                Course = new ApprenticeshipCourse { FundingPeriods = fundingPeriods },
                TotalCostAsString = costAsString,
                NumberOfApprentices = 2
            };

            _moqer.GetMock<IAddApprenticeshipOrchestrator>()
                .Setup(x => x.UpdateAddApprenticeship(vm))
                .Returns(Task.FromResult(vm));

            _moqer.GetMock<IAddApprenticeshipOrchestrator>()
                .Setup(x => x.GetApprenticeshipAddSetup(false))
                .Returns(new AddApprenticeshipViewModel { Courses = new List<ApprenticeshipCourse>() });

            await _controller.Save(vm, "ABBA12", "default");
            _controller.ViewData.ModelState.Count
                .Should().Be(shouldError ? 1 : 0);
        }

        [TestCase("11000", true)]
        [TestCase("10000", false)]
        public async Task Apprenticeship_Must_Not_Have_higher_Total_Cost_than_FundingBand(string costAsString, bool shouldError)
        {
            var fundingBands = new List<FundingPeriod>
            {
                new FundingPeriod { EffectiveFrom = DateTime.Today.AddMonths(-24), EffectiveTo = DateTime.Today.AddMonths(-2), FundingCap = 1000 },
                new FundingPeriod { EffectiveFrom = DateTime.Today.AddMonths(-1), EffectiveTo = DateTime.Today.AddMonths(24), FundingCap = 5000 },
                new FundingPeriod { EffectiveFrom = DateTime.Today.AddMonths(25), EffectiveTo = DateTime.Today.AddMonths(12 * 4), FundingCap = 10000 }
            };

            var vm = new AddApprenticeshipViewModel
            {
                CourseId = "123",
                Course = new ApprenticeshipCourse
                {
                    FundingPeriods = fundingBands
                },
                TotalCostAsString = costAsString,
                NumberOfApprentices = 2,
                StartDateMonth = DateTime.Today.Month,
                StartDateYear = DateTime.Today.Year,
            };

            _moqer.GetMock<IAddApprenticeshipOrchestrator>()
                .Setup(x => x.UpdateAddApprenticeship(vm))
                .Returns(Task.FromResult(vm));

            _moqer.GetMock<IAddApprenticeshipOrchestrator>()
                .Setup(x => x.GetApprenticeshipAddSetup(false))
                .Returns(new AddApprenticeshipViewModel { Courses = new List<ApprenticeshipCourse>() });

            await _controller.Save(vm, "ABBA12", "default");
            _controller.ViewData.ModelState.Count
                .Should().Be(shouldError ? 1 : 0);
        }
    }
}
