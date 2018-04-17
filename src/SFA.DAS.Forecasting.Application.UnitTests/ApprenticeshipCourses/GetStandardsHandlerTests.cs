using AutoMoq;
using NUnit.Framework;

namespace SFA.DAS.Forecasting.Application.UnitTests.ApprenticeshipCourses
{
    [TestFixture]
    public class GetStandardsHandlerTests
    {
        private AutoMoqer _moqer;

        [SetUp]
        public void SetUp()
        {
            _moqer = new AutoMoqer();
        }
    }
}