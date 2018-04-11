using AutoMoq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Estimations
{
    public class AccountEstimationTests
    {
        private AutoMoq.AutoMoqer _moqer;
        private AccountEstimationModel _model;

        [SetUp]
        public void SetUp()
        {
            _moqer = new AutoMoqer();
        }

        private AccountEstimation ResolveEstimation()
        {
            return _moqer.Resolve<AccountEstimation>();
        }

        [Test]
        public void Add_Virtual_Apprenticeship_Assigns_Id_To_Apprenticeship()
        {
            var estimation = ResolveEstimation();
            var apprenticeship = estimation.AddVirtualAppreniceship("course-1", "test course", 1, 1, 2019, 5, 18, 1000);
            Assert.IsNotNull(apprenticeship, "Invalid virtual apprenticeship generated.");
            Assert.IsNotNull(apprenticeship.Id, "Apprentieship id not populated.");
        }
    }
}