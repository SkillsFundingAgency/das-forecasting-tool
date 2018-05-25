using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Domain.Projections.Services;
using SFA.DAS.Forecasting.Models.Projections;
using SFA.DAS.Forecasting.Web.Orchestrators;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;
using SFA.DAS.HashingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Commitments.Services;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Web.UnitTests.OrchestratorTests
{
    [TestFixture]
    public class ForecastingOrchestratorTests
    {
        private ForecastingOrchestrator _sut;
        private Mock<IAccountProjectionDataSession> _accountProjection;
        private Mock<IApplicationConfiguration> _applicationConfiguration;
        private Fixture _fixture;
        private Mock<ICommitmentsDataService> _commitmentDataService;

        private const long ExpectedAccountId = 12345;
        private const long SendingEmployerAccountId = 554400;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var hashingService = new Mock<IHashingService>();
            _accountProjection = new Mock<IAccountProjectionDataSession>();
            _applicationConfiguration = new Mock<IApplicationConfiguration>();
            _commitmentDataService = new Mock<ICommitmentsDataService>();

            _commitmentDataService.Setup(x => x.GetCurrentCommitments(ExpectedAccountId)).ReturnsAsync(
                new List<CommitmentModel>
                {
                    new CommitmentModel
                    {
                        StartDate = DateTime.Now.AddMonths(-1),
                        PlannedEndDate = DateTime.Now.AddMonths(12),
                        ApprenticeName = "Alan Wake",
                        EmployerAccountId = ExpectedAccountId,
                        SendingEmployerAccountId = ExpectedAccountId,
                        MonthlyInstallment = 10.6m,
                        NumberOfInstallments = 10,
                        CompletionAmount = 100.8m,
                        CourseName = "My Course",
                        CourseLevel = 1,
                        LearnerId = 90,
                        ProviderName = "Test Provider",
                        ProviderId = 99876
                    },
                    new CommitmentModel
                    {
                        StartDate = DateTime.Now.AddMonths(2),
                        PlannedEndDate = DateTime.Now.AddMonths(12),
                        ApprenticeName = "Jane Doe",
                        EmployerAccountId = ExpectedAccountId,
                        SendingEmployerAccountId = SendingEmployerAccountId,
                        MonthlyInstallment = 10,
                        NumberOfInstallments = 10,
                        CompletionAmount = 100,
                        CourseName = "My Course",
                        CourseLevel = 1,
                        LearnerId = 90,
                        ProviderName = "Test Provider",
                        ProviderId = 99876
                    }
                });

            _commitmentDataService.Setup(x => x.GetCurrentCommitments(SendingEmployerAccountId)).ReturnsAsync(
                new List<CommitmentModel>
                {
                    new CommitmentModel
                    {
                        StartDate = DateTime.Now.AddMonths(-1),
                        PlannedEndDate = DateTime.Now.AddMonths(12),
                        ApprenticeName = "Marcus Fenix",
                        EmployerAccountId = ExpectedAccountId,
                        SendingEmployerAccountId = SendingEmployerAccountId,
                        MonthlyInstallment = 10.6m,
                        NumberOfInstallments = 10,
                        CompletionAmount = 100.8m,
                        CourseName = "My Course",
                        CourseLevel = 1,
                        LearnerId = 90,
                        ProviderName = "Test Provider",
                        ProviderId = 99876
                    }
                }
            );

            hashingService.Setup(m => m.DecodeValue("ABBA12"))
                .Returns(ExpectedAccountId);
            hashingService.Setup(m => m.DecodeValue("CDDC12"))
                .Returns(SendingEmployerAccountId);

            _sut = new ForecastingOrchestrator(
                hashingService.Object,
                _accountProjection.Object,
                _applicationConfiguration.Object,
                new Mapper(),
                _commitmentDataService.Object);
        }

        [Test]
        public async Task ShouldTake_48_months_projections()
        {
            SetUpProjections(48 + 10);

            var balance = await _sut.Balance("ABBA12");

            balance.BalanceItemViewModels.Count().Should().Be(48);
        }

        [Test]
        public async Task Should_Take_Commitments_Used_For_projection_To_Build_Csv_File_At_The_Active_Commitment_Level()
        {
            var balanceCsv = await _sut.BalanceCsv("ABBA12");

            balanceCsv.Count().Should().Be(2);
            _commitmentDataService.Verify(x => x.GetCurrentCommitments(ExpectedAccountId), Times.Once);
        }

        [Test]
        public async Task Then_The_Model_Is_Returned_With_The_Costs_Are_Calculated_And_Rounded_Correctly()
        {
            //Act
            var balanceCsv = (await _sut.BalanceCsv("ABBA12")).ToList();

            //Assert
            Assert.IsNotNull(balanceCsv);
            Assert.IsNotEmpty(balanceCsv);
            var actualFirstCommitment = balanceCsv.First();
            Assert.IsNotNull(actualFirstCommitment);
            Assert.AreEqual(207, actualFirstCommitment.TotalCost);
            Assert.AreEqual(11, actualFirstCommitment.MonthlyTrainingCost);
            Assert.AreEqual(101, actualFirstCommitment.CompletionAmount);
        }

        [Test]
        public async Task Then_The_Model_Is_Returned_With_The_ApprenticeshipName_And_Uln_Are_Blank_If_Your_The_Sending_Employer()
        {
            //Act
            var balanceCsv = (await _sut.BalanceCsv("ABBA12")).ToList();

            //Assert
            var actualFirstCommitment = balanceCsv.Last();
            Assert.IsNotNull(actualFirstCommitment);
            Assert.IsEmpty(actualFirstCommitment.ApprenticeName);
            Assert.IsEmpty(actualFirstCommitment.Uln);
        }

        [Test]
        public async Task Then_If_I_Am_The_Receiving_Employer_Of_A_Transfer_The_Apprentice_Data_Is_Visible()
        {
            //Act
            var balanceCsv = (await _sut.BalanceCsv("CDDC12")).ToList();

            //Assert
            var actualFirstCommitment = balanceCsv.Last();
            Assert.IsNotNull(actualFirstCommitment);
            Assert.IsNotEmpty(actualFirstCommitment.ApprenticeName);
            Assert.IsNotEmpty(actualFirstCommitment.Uln);
        }

        [Test]
        public async Task Then_The_Csv_Dates_Are_Formatted_As_Month_And_Year()
        {
            //Act
            var balanceCsv = (await _sut.BalanceCsv("ABBA12")).ToList();

            //Assert
            var actualFirstCommitment = balanceCsv.First();
            Assert.IsNotNull(actualFirstCommitment);
            Assert.AreEqual(DateTime.Now.AddMonths(-1).ToString("MMM-yy"), actualFirstCommitment.StartDate);
            Assert.AreEqual(DateTime.Now.AddMonths(12).ToString("MMM-yy"), actualFirstCommitment.PlannedEndDate);
        }


        [Test]
        public async Task Then_If_It_Is_Not_A_Transfer_Then_The_Transfer_To_Employer_Field_Is_Empty()
        {
            //Act
            var balanceCsv = (await _sut.BalanceCsv("ABBA12")).ToList();

            //Assert
            var actualFirstCommitment = balanceCsv.First();
            Assert.IsNotNull(actualFirstCommitment);
            Assert.IsEmpty(actualFirstCommitment.TransferToEmployerName);
        }

        [Test]
        public async Task Should_not_Take_months_after_2019_05_01()
        {
            var balanceMaxDate = new DateTime(2019, 05, 01);

            SetUpProjections(48 + 10);
            _applicationConfiguration.Setup(m => m.LimitForecast)
                .Returns(true);

            var balance = await _sut.Balance("ABBA12");

            balance.BalanceItemViewModels.All(m => m.Date < balanceMaxDate).Should().BeTrue();
            balance.BalanceItemViewModels.Count().Should().BeGreaterOrEqualTo(1);
            if (DateTime.Today >= balanceMaxDate)
                Assert.IsTrue(false,
                    $"balanceMaxDate is out of date ({balanceMaxDate.ToString()}) and test can be removed");
        }

        [Test]
        public async Task Should_not_Take_months_after_2019_05_01_csv()
        {
            var balanceMaxDate = new DateTime(2019, 05, 01);
            SetUpProjections(48 + 10);
            _applicationConfiguration.Setup(m => m.LimitForecast)
                .Returns(true);

            var balanceCsv = await _sut.BalanceCsv("ABBA12");

            balanceCsv.All(m => (DateTime.Parse(m.StartDate)) < balanceMaxDate).Should().BeTrue();
            balanceCsv.Count().Should().BeGreaterOrEqualTo(1);
            if (DateTime.Today >= balanceMaxDate)
                Assert.IsTrue(false,
                    $"balanceMaxDate is out of date ({balanceMaxDate.ToString()}) and test can be removed");
        }


        /// <summary>
        /// Setting up projections starting from last month. 
        /// </summary>
        /// <param name="count">Amount months to create starting from last month.</param>
        private void SetUpProjections(int count)
        {
            var projections = new List<AccountProjectionModel>();
            _fixture.AddManyTo(projections, count);

            var currentMonthDate = (new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)).AddMonths(-1);
            projections.ForEach(m =>
            {
                var date = DateTime.Parse(currentMonthDate.ToString());
                m.Month = (short) date.Month;
                m.Year = date.Year;

                currentMonthDate = currentMonthDate.AddMonths(1);
            });

            _accountProjection.Setup(m => m.Get(ExpectedAccountId)).ReturnsAsync(projections);
        }
    }
}