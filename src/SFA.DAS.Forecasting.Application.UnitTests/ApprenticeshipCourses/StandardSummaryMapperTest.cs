using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.UnitTests.ApprenticeshipCourses
{
    [TestFixture]
    public class StandardSummaryMapperTest
    {
        private StandardSummaryMapper _mapper;
        [SetUp]
        public void SetUp()
        {
            _mapper = new StandardSummaryMapper();
        }

        [Test]
        public void Maps_Id()
        {
            Assert.AreEqual("1234",_mapper.Map(new StandardSummary {Id = "1234"}).Id);
        }

        [Test]
        public void Maps_Title()
        {
            Assert.AreEqual("test course", _mapper.Map(new StandardSummary { Title = "test course"}).Title);
        }

        [Test]
        public void Maps_Level()
        {
            Assert.AreEqual(1, _mapper.Map(new StandardSummary { Level = 1}).Level);
        }

        [Test]
        public void Maps_Duration()
        {
            Assert.AreEqual(18, _mapper.Map(new StandardSummary { Duration = 18}).Duration);
        }

        [Test]
        public void Maps_CourseType()
        {
            Assert.AreEqual(ApprenticeshipCourseType.Standard, _mapper.Map(new StandardSummary ()).CourseType);
        }
    }
}