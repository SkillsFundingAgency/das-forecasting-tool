using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMoq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Domain.Estimations.Validation.VirtualApprenticeships;
using SFA.DAS.Forecasting.Domain.Shared;
using SFA.DAS.Forecasting.Domain.Shared.Validation;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Models.Payments;

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
            _moqer.GetMock<IDateTimeService>()
                .Setup(x => x.GetCurrentDateTime())
                .Returns(new DateTimeService().GetCurrentDateTime());
        }

        private AccountEstimation ResolveEstimation()
        {
            return _moqer.Resolve<AccountEstimation>();
        }

        [Test]
        public void Add_Virtual_Apprenticeship_Assigns_Id_To_Apprenticeship()
        {
            var estimation = ResolveEstimation();
            var apprenticeship = estimation.AddVirtualApprenticeship("course-1", "test course", 1, 1, 2019, 5, 18, 1000, FundingSource.Transfer);
            Assert.IsNotNull(apprenticeship, "Invalid virtual apprenticeship generated.");
            Assert.IsNotNull(apprenticeship.Id, "Apprentieship id not populated.");
        }

        [Test]
        public void Throws_Exception_If_Validation_Fails()
        {
            _moqer.GetMock<IVirtualApprenticeshipValidator>()
                .Setup(x => x.Validate(It.IsAny<VirtualApprenticeship>()))
                .Returns(new List<ValidationResult> { ValidationResult.Failed("test fail") });
            var estimation = ResolveEstimation();
            Assert.Throws<InvalidOperationException>(() => estimation.AddVirtualApprenticeship("course-1", "test course", 1, 1, 2019, 5, 18, 1000, FundingSource.Transfer), "Should throw an exception if the apprenticeship fails validation");
        }

        [Test]
        public void Valid_Apprenticeships_Are_Added_To_The_Model()
        {
            var estimation = ResolveEstimation();
            var apprenticeship = estimation.AddVirtualApprenticeship("course-1", "test course", 1, 1, 2019, 5, 18, 1000, FundingSource.Transfer);
            Assert.IsTrue(estimation.Apprenticeships.Any(x => x.Id == apprenticeship.Id));
        }

        [Test]
        public void Remove_Returns_False_If_Apprenticeship_Not_Found()
        {
            var estimation = ResolveEstimation();
            Assert.IsFalse(estimation.RemoveVirtualApprenticeship("apprenticeship-1"));
        }

        [Test]
        public void Remove_Returns_True_If_Apprenticeship_Removed()
        {
            _model.Apprenticeships.Add(new VirtualApprenticeship { Id = "apprenticeship-1" });
            var estimation = ResolveEstimation();
            Assert.IsTrue(estimation.RemoveVirtualApprenticeship("apprenticeship-1"));
        }

        [Test]
        public void Remove_Apprenticeship_Removes_Apprenticeship()
        {
            _model.Apprenticeships.Add(new VirtualApprenticeship { Id = "apprenticeship-1" });
            var estimation = ResolveEstimation();
            estimation.RemoveVirtualApprenticeship("apprenticeship-1");
            Assert.IsTrue(estimation.Apprenticeships.All(x => x.Id != "apprenticeship-1"));
        }

        [Test]
        public void Should_Update_Apprenticeship()
        {
            var a = new VirtualApprenticeship
            {
                Id = Guid.NewGuid().ToString("N"),
                CourseId = "ABBA12",
                CourseTitle = "ABBA 12",
                Level = 1,
                ApprenticesCount = 10,
                StartDate = new DateTime(DateTime.Today.Year + 1, 5, 1),
                TotalInstallments = 24,
                TotalCost = 2000,
            };

            _model.Apprenticeships.Add(a);

            var estimation = ResolveEstimation();

            estimation.UpdateApprenticeship(a.Id, 10, DateTime.Today.Year + 2, 6, 12, 1000);

            estimation.Apprenticeships.Count().Should().Be(1);

            var apprenticeship = estimation.Apprenticeships.First();
            apprenticeship.CourseId.Should().Be("ABBA12");
            apprenticeship.CourseTitle.Should().Be("ABBA 12");
            apprenticeship.Level.Should().Be(1);
            apprenticeship.ApprenticesCount.Should().Be(6);
            apprenticeship.StartDate.Year.Should().Be(DateTime.Today.Year + 2);
            apprenticeship.StartDate.Month.Should().Be(10);
            apprenticeship.TotalCost.Should().Be(1000);
            apprenticeship.TotalInstallments.Should().Be(12);

            apprenticeship.TotalCompletionAmount.Should().Be(200);
            Decimal.Round(apprenticeship.TotalInstallmentAmount, 1).Should().Be(66.7M);
        }

        [Test]
        public void Add_Virtual_Apprenticeship_Assigns_FundingSource_To_Apprenticeship()
        {
            var estimation = ResolveEstimation();

            var apprenticeship = estimation.AddVirtualApprenticeship("course-1", "test course", 1, 1, 2019, 5, 18, 1000, FundingSource.Transfer);
            Assert.AreEqual(apprenticeship.FundingSource, FundingSource.Transfer);

            var apprenticeship2 = estimation.AddVirtualApprenticeship("course-1", "test course", 1, 1, 2019, 5, 18, 1000, FundingSource.Levy);
            Assert.AreEqual(apprenticeship2.FundingSource, FundingSource.Levy);
        }

        
        [TestCase(-20, 12, false)]
        [TestCase(-15, 12, false)]
        [TestCase(-5, 10, true)]
        public void Has_Valid_Apprenticeships_Returns_False_When_No_Active_Apprenticeshp(int numberOfMonthsToAdd, int numberOfMonths, bool isValid)
        {
            // arrange
            var estimation = ResolveEstimation();
            var startDate = DateTime.Now.AddMonths(numberOfMonthsToAdd);
            estimation.AddVirtualApprenticeship("course-1", "test course", 1, startDate.Month, startDate.Year, 5, numberOfMonths, 1000, FundingSource.Transfer);

            //act
            var isAppreniceshipValid = estimation.HasValidApprenticeships;

            //assert
            Assert.AreEqual(isValid, isAppreniceshipValid);
        }
    }
}