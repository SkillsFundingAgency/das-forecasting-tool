using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Models.Estimation;
using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Application.UnitTests.ApprenticeshipCourses
{
    [TestFixture]
    public class StandardSummaryMapperTest
    {
        private ApprenticehipsCourseMapper _mapper;
        [SetUp]
        public void SetUp()
        {
            _mapper = new ApprenticehipsCourseMapper();
        }

        [Test]
        public void Maps_Id()
        {
            Assert.AreEqual("1234",_mapper.Map(new StandardSummary {Id = "1234", FundingPeriods = new List<Apprenticeships.Api.Types.FundingPeriod>()}).Id);
        }

        [Test]
        public void Maps_Title()
        {
            Assert.AreEqual("test course", _mapper.Map(new StandardSummary { Title = "test course", FundingPeriods = new List<Apprenticeships.Api.Types.FundingPeriod>() }).Title);
        }

        [Test]
        public void Maps_Level()
        {
            Assert.AreEqual(1, _mapper.Map(new StandardSummary { Level = 1, FundingPeriods = new List<Apprenticeships.Api.Types.FundingPeriod>() }).Level);
        }

        [Test]
        public void Maps_Duration()
        {
            Assert.AreEqual(18, _mapper.Map(new StandardSummary { Duration = 18, FundingPeriods = new List<Apprenticeships.Api.Types.FundingPeriod>() }).Duration);
        }

        [Test]
        public void Maps_CourseType()
        {
            Assert.AreEqual(ApprenticeshipCourseType.Standard, _mapper.Map(new StandardSummary { FundingPeriods = new List<Apprenticeships.Api.Types.FundingPeriod>() } ).CourseType);
        }


        [Test]
        public void Maps_Framework_Id()
        {
            var frameworkSummary = new FrameworkSummary { Id = "1234", FundingPeriods = new List<Apprenticeships.Api.Types.FundingPeriod>() };
            var course = _mapper.Map(frameworkSummary);
            Assert.AreEqual("1234", course.Id);
        }

        [Test]
        public void Maps_Framework_Title()
        {
            var frameworkSummary = new FrameworkSummary { Title = "test course", FundingPeriods = new List<Apprenticeships.Api.Types.FundingPeriod>() };
            var course = _mapper.Map(frameworkSummary);
            Assert.AreEqual("test course", course.Title);
        }

        [Test]
        public void Maps_Framework_Level()
        {
            var frameworkSummary = new FrameworkSummary { Level = 1, FundingPeriods = new List<Apprenticeships.Api.Types.FundingPeriod>() };
            var course = _mapper.Map(frameworkSummary);
            Assert.AreEqual(1, course.Level);
        }

        [Test]
        public void Maps_Framework_Duration()
        {
            var frameworkSummary = new FrameworkSummary { Duration = 18, FundingPeriods = new List<Apprenticeships.Api.Types.FundingPeriod>() };
            var course = _mapper.Map(frameworkSummary);
            Assert.AreEqual(18, course.Duration);
        }

        [Test]
        public void Maps_Framework_CourseType()
        {
            var frameworkSummary = new FrameworkSummary { FundingPeriods = new List<Apprenticeships.Api.Types.FundingPeriod>() };
            var course = _mapper.Map(frameworkSummary);
            Assert.AreEqual(ApprenticeshipCourseType.Framework, course.CourseType);
        }
    }
}