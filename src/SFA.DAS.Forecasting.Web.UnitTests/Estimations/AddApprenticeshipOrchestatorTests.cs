using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMoq;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Web.Orchestrators.Estimations;
using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.UnitTests.Estimations
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
        
        [TestCase("6,000", 6000, "6,000")]
        [TestCase("6000", 6000, "6,000")]
        [TestCase("6,000.49", 6000, "6,000")]
        [TestCase("6,000.50", 6001, "6,001")]
        [TestCase("£6,000", null, "")]
        [TestCase("A6,000", null, "")]
        [TestCase("600", 600, "600")]
        [TestCase("6600", 6600, "6,600")]
        [TestCase("66,000", 66000, "66,000")]
        [TestCase("66,000,000", 66000000, "66,000,000")]
        [TestCase("66000000", 66000000, "66,000,000")]
        [TestCase("660,00,000", 66000000, "66,000,000")]
        [TestCase("66,000000", 66000000, "66,000,000")]
        public async Task TestingCommasAddedToTotalCostAsStringSetsExcpectedTotalCostValues(string totalCostAsStringInput, decimal? totalCost, string totalCostAsStringOutput)
        {
            var orchestrator = _moqer.Resolve<AddApprenticeshipOrchestrator>();
            var vm = new AddApprenticeshipViewModel {ApprenticeshipToAdd = new ApprenticeshipToAdd {TotalCostAsString = totalCostAsStringInput } };
            var res = await orchestrator.ValidateAddApprenticeship(vm);

            AssertionExtensions.Should((decimal?) res.ApprenticeshipToAdd.TotalCost).Be(totalCost);
            AssertionExtensions.Should((string) res.ApprenticeshipToAdd.TotalCostAsString).Be(totalCostAsStringOutput);      
        }

        [Test]
        public void TestingAddApprenticeSetupReturnsExpectedDefaultSetup()
        {
            var orchestrator = _moqer.Resolve<AddApprenticeshipOrchestrator>();
            var res = orchestrator.GetApprenticeshipAddSetup();
            AssertionExtensions.Should((string) res.Name).Be("Add Apprenticeships");
            res.ApprenticeshipToAdd.ShouldBeEquivalentTo(new ApprenticeshipToAdd());
            res.ValidationResults.ShouldBeEquivalentTo(new List<ValidationResult>());
            AssertionExtensions.Should((int) res.AvailableApprenticeships.Count()).Be(2);
        }

        [Test]
        public void TestingAddApprenticeSetupReturnsDefaultSetupWithAvailableApprenticeshipsOrderedAsExpected()
        {
            var orchestrator = _moqer.Resolve<AddApprenticeshipOrchestrator>();
            var res = orchestrator.GetApprenticeshipAddSetup();
            res.ValidationResults.ShouldBeEquivalentTo(new List<ValidationResult>());
            AssertionExtensions.Should((int) res.AvailableApprenticeships.Count()).Be(2);
            res.AvailableApprenticeships.First().ShouldBeEquivalentTo(_courseCarpentry);
            res.AvailableApprenticeships.ElementAt(1).ShouldBeEquivalentTo(_courseElectrician);
        }
    }
}
