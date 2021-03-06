﻿using AutoMoq;
using FluentAssertions;
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
            var vm = new AddEditApprenticeshipsViewModel
            {
                Course = null,
                TotalCostAsString = "10",
                StartDateYear = DateTime.Now.Year,
                StartDateMonth = DateTime.Now.Month
            };

            _moqer.GetMock<IAddApprenticeshipOrchestrator>()
                .Setup(x => x.UpdateAddApprenticeship(vm))
                .Returns(Task.FromResult(vm));

            _moqer.GetMock<IAddApprenticeshipOrchestrator>()
                .Setup(x => x.GetApprenticeshipAddSetup(false))
                .Returns(new AddEditApprenticeshipsViewModel { Courses = new List<ApprenticeshipCourse>() });

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

            var vm = new AddEditApprenticeshipsViewModel
            {
                Course = new ApprenticeshipCourse { Id = "123", FundingPeriods = fundingPeriods },
                TotalCostAsString = costAsString,
                NumberOfApprentices = 2,
                StartDateYear = DateTime.Now.Year,
                StartDateMonth = DateTime.Now.Month
            };

            _moqer.GetMock<IAddApprenticeshipOrchestrator>()
                .Setup(x => x.UpdateAddApprenticeship(vm))
                .Returns(Task.FromResult(vm));

            _moqer.GetMock<IAddApprenticeshipOrchestrator>()
                .Setup(x => x.GetApprenticeshipAddSetup(false))
                .Returns(new AddEditApprenticeshipsViewModel { Courses = new List<ApprenticeshipCourse>() });

            await _controller.Save(vm, "ABBA12", "default");
            _controller.ViewData.ModelState.Count
                .Should().Be(shouldError ? 1 : 0);
        }

    }
}
