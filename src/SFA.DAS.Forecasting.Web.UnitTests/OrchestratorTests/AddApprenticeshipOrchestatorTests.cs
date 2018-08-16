using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMoq;
using FluentAssertions;
using SFA.DAS.Forecasting.Web.Orchestrators.Estimations;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.UnitTests.OrchestratorTests
{
    [TestFixture]
    public class AddApprenticeshipOrchestatorTests
    {
        private AutoMoqer _moqer;
        private List<ApprenticeshipCourse> _apprenticeshipCourses;
        private ApprenticeshipCourse _courseElectrician;
        private ApprenticeshipCourse _courseCarpentry;


        [SetUp]
        public void SetUp()
        {
            _moqer = new AutoMoqer();
           
            _courseElectrician = new ApprenticeshipCourse
            {
                CourseType = ApprenticeshipCourseType.Standard,
                Duration = 23,
                FundingCap = 12000,
                Id = "567",
                Level = 4,
                Title = "Electrician"
            };

            _courseCarpentry =
                new ApprenticeshipCourse
                {
                    CourseType = ApprenticeshipCourseType.Standard,
                    Duration = 36,
                    FundingCap = 9000,
                    Id = "123",
                    Level = 4,
                    Title = "Carpentry"
                };

            _apprenticeshipCourses = new List<ApprenticeshipCourse>
            {
                _courseElectrician,
               _courseCarpentry,
            };

            _moqer.GetMock<IApprenticeshipCourseDataService>()
                .Setup(x => x.GetAllStandardApprenticeshipCourses())
                .Returns(_apprenticeshipCourses);
        }
        
        [TestCase("6,000", "6,000")]
        [TestCase("6000", "6,000")]
        [TestCase("6,000.49", "6,000")]
        [TestCase("6,000.50", "6,001")]
        [TestCase("£6,000", "")]
        [TestCase("A6,000", "")]
        [TestCase("600", "600")]
        [TestCase("6600", "6,600")]
        [TestCase("66,000", "66,000")]
        [TestCase("66,000,000", "66,000,000")]
        [TestCase("66000000", "66,000,000")]
        [TestCase("660,00,000", "66,000,000")]
        [TestCase("66,000000", "66,000,000")]
        public async Task TestingCommasAddedToTotalCostAsStringSetsExcpectedTotalCostValues(string totalCostAsStringInput, string totalCostAsStringOutput)
        {
            var orchestrator = _moqer.Resolve<AddApprenticeshipOrchestrator>();
            var vm = new AddApprenticeshipViewModel()
            {
                NumberOfApprentices = 1,
                TotalCostAsString = totalCostAsStringInput,
                Course = new ApprenticeshipCourse()
            };
            
            // Act
            var res = await orchestrator.UpdateAddApprenticeship(vm);

            // Assert
            AssertionExtensions.Should((string) res.TotalCostAsString).Be(totalCostAsStringOutput);      
        }

        [Test]
        public void TestingAddApprenticeSetupReturnsExpectedDefaultSetup()
        {
            var orchestrator = _moqer.Resolve<AddApprenticeshipOrchestrator>();
            var res = orchestrator.GetApprenticeshipAddSetup(true);
            res.ValidationResults.Should().BeEquivalentTo(new List<ValidationResult>());
            AssertionExtensions.Should((int) res.ApprenticeshipCourses.Count()).Be(2);
        }

        [Test]
        public void TestingAddApprenticeSetupReturnsDefaultSetupWithAvailableApprenticeshipsOrderedAsExpected()
        {
            var orchestrator = _moqer.Resolve<AddApprenticeshipOrchestrator>();
            var res = orchestrator.GetApprenticeshipAddSetup(true);
            res.ValidationResults.Should().BeEquivalentTo(new List<ValidationResult>());
            AssertionExtensions.Should((int) res.ApprenticeshipCourses.Count()).Be(2);

            var courseCarpentry = res.ApprenticeshipCourses.First();
            var courseElectrician = res.ApprenticeshipCourses.ElementAt(1);

            courseCarpentry.Value.Should().Be(_courseCarpentry.Id);
            courseCarpentry.Text.Should().Be($"{_courseCarpentry.Title}, Level: {_courseCarpentry.Level} (Standard)");

            courseElectrician.Value.Should().Be(_courseElectrician.Id);
            courseElectrician.Text.Should().Be($"{_courseElectrician.Title}, Level: {_courseElectrician.Level} (Standard)");
        }
    }
}
