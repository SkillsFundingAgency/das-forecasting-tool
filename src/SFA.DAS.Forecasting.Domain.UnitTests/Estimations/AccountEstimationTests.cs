using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMoq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Domain.Estimations.Validation.VirtualApprenticeships;
using SFA.DAS.Forecasting.Domain.Shared.Validation;
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
            _model = new AccountEstimationModel
            {
                Id = Guid.NewGuid().ToString("N"),
                Apprenticeships = new List<VirtualApprenticeship>(),
                EmployerAccountId = 12345,
                EstimationName = "default"
            };
            _moqer.SetInstance(_model);
            _moqer.GetMock<IVirtualApprenticeshipValidator>()
                .Setup(x => x.Validate(It.IsAny<VirtualApprenticeship>()))
                .Returns(new List<ValidationResult>());
            _moqer.GetMock<IAccountEstimationRepository>()
                .Setup(x => x.Get(It.IsAny<long>()))
                .Returns(Task.FromResult(_moqer.Resolve<AccountEstimation>()));

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

        [Test]
        public void Throws_Exception_If_Validation_Fails()
        {
            _moqer.GetMock<IVirtualApprenticeshipValidator>()
                .Setup(x => x.Validate(It.IsAny<VirtualApprenticeship>()))
                .Returns(new List<ValidationResult>{new ValidationResult {}});
            var estimation = ResolveEstimation();
            var apprenticeship = estimation.AddVirtualAppreniceship("course-1", "test course", 1, 1, 2019, 5, 18, 1000);
            Assert.IsNotNull(apprenticeship, "Invalid virtual apprenticeship generated.");
            Assert.IsNotNull(apprenticeship.Id, "Apprentieship id not populated.");

        }
    }
}