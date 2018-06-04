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
using AutoMoq;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.Forecasting.Domain.Commitments.Services;
using SFA.DAS.Forecasting.Models.Balance;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Web.UnitTests.OrchestratorTests
{
    [TestFixture]
    public class ForecastingOrchestratorTests
    {
        private AutoMoqer _moqer;

        private List<CommitmentModel> _commitments;
        private Fixture _fixture;
        private BalanceModel _balance;
        private const long ExpectedAccountId = 12345;
        private const long ReceivingEmployerAccountId = 554400;

        [SetUp]
        public void SetUp()
        {
            _moqer = new AutoMoqer();
            //TODO: Is fixture really needed? it seems to only be used to add items to a list. Enumerable.Range or Repeat can be used for this purpose.
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _commitments = new List<CommitmentModel>
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
                    ProviderId = 99876,
                    FundingSource = FundingSource.Levy
                },
                new CommitmentModel
                {
                    StartDate = DateTime.Now.AddMonths(2),
                    PlannedEndDate = DateTime.Now.AddMonths(12),
                    ApprenticeName = "Jane Doe",
                    EmployerAccountId = ReceivingEmployerAccountId,
                    SendingEmployerAccountId = ExpectedAccountId,
                    MonthlyInstallment = 10,
                    NumberOfInstallments = 10,
                    CompletionAmount = 100,
                    CourseName = "My Course",
                    CourseLevel = 1,
                    LearnerId = 90,
                    ProviderName = "Test Provider",
                    ProviderId = 99876,
                    FundingSource = FundingSource.Transfer
                }
            };
            _moqer.GetMock<IAccountProjectionDataSession>()
                .Setup(x => x.GetCommitments(It.IsAny<long>(), It.IsAny<DateTime?>()))
                .Returns(Task.FromResult(_commitments));

            var hashingService = _moqer.GetMock<IHashingService>();
            hashingService
                .Setup(m => m.DecodeValue("ABBA12"))
                .Returns(ExpectedAccountId);
            hashingService
                .Setup(m => m.DecodeValue("CDDC12"))
                .Returns(ReceivingEmployerAccountId);
            _balance = new BalanceModel { EmployerAccountId = 12345, Amount = 50000, TransferAllowance = 5000, RemainingTransferBalance = 5000, UnallocatedCompletionPayments = 2000 };

            _moqer.GetMock<IBalanceDataService>()
                .Setup(x => x.Get(It.IsAny<long>()))
                .ReturnsAsync(_balance);

            _moqer.SetInstance<IForecastingMapper>(new ForecastingMapper());
        }

        [Test]
        public async Task ShouldTake_48_months_projections()
        {
            SetUpProjections(48 + 10);

            var balance = await _moqer.Resolve<ForecastingOrchestrator>().Balance("ABBA12");

            balance.BalanceItemViewModels.Count().Should().Be(48);
        }

        [Test]
        public async Task Should_Take_Commitments_Used_For_projection_To_Build_Csv_File_At_The_Active_Commitment_Level()
        {
            var apprenticesCsv = await _moqer.Resolve<ForecastingOrchestrator>().ApprenticeshipsCsv("ABBA12");
            apprenticesCsv.Count().Should().Be(2);
            _moqer.GetMock<IAccountProjectionDataSession>().Verify(x => x.GetCommitments(It.Is<long>(id => id == ExpectedAccountId), It.IsAny<DateTime?>()), Times.Once);
        }

        [Test]
        public async Task Then_The_Model_Is_Returned_With_The_Costs_Are_Calculated_And_Rounded_Correctly()
        {
            //Act
            var apprenticeshipCsv = (await _moqer.Resolve<ForecastingOrchestrator>().ApprenticeshipsCsv("ABBA12")).ToList();

            //Assert
            Assert.IsNotNull(apprenticeshipCsv);
            Assert.IsNotEmpty(apprenticeshipCsv);
            var firstApprenticeship = apprenticeshipCsv.First();
            Assert.IsNotNull(firstApprenticeship);
            Assert.AreEqual(207, firstApprenticeship.TotalCost);
            Assert.AreEqual(11, firstApprenticeship.MonthlyTrainingCost);
            Assert.AreEqual(101, firstApprenticeship.CompletionAmount);
        }

        [Test]
        public async Task Then_The_Model_Is_Returned_With_The_ApprenticeshipName_And_Uln_Are_Blank_If_You_Are_The_Sending_Employer()
        {
            //Act
            var apprenticeshipCsv = (await _moqer.Resolve<ForecastingOrchestrator>().ApprenticeshipsCsv("ABBA12")).ToList();

            //Assert
            var lastApprenticeship = apprenticeshipCsv.Last();
            Assert.IsNotNull(lastApprenticeship);
            Assert.IsEmpty(lastApprenticeship.ApprenticeName);
            Assert.IsEmpty(lastApprenticeship.Uln);
        }

        [Test]
        public async Task Then_If_I_Am_The_Receiving_Employer_Of_A_Transfer_The_Apprentice_Data_Is_Visible()
        {
            //Act
            var apprenticeshipCsv = (await _moqer.Resolve<ForecastingOrchestrator>().ApprenticeshipsCsv("CDDC12")).ToList();

            //Assert
            var firstApprenticeship = apprenticeshipCsv.Last();
            Assert.IsNotNull(firstApprenticeship);
            Assert.IsNotEmpty(firstApprenticeship.ApprenticeName);
            Assert.IsNotEmpty(firstApprenticeship.Uln);
        }

        [Test]
        public async Task Then_Commitments_Outside_Of_The_Projection_Date_Are_Not_Used()
        {
            //Arrange
            _commitments.Clear();
            _commitments.Add(new CommitmentModel
            {
                StartDate = DateTime.Now.AddMonths(-12),
                PlannedEndDate = DateTime.Now.AddMonths(-1),
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
                ProviderId = 99876,
                FundingSource = FundingSource.Levy
            });
            _commitments.Add(new CommitmentModel
            {
                StartDate = DateTime.Now.AddMonths(50),
                PlannedEndDate = DateTime.Now.AddMonths(62),
                ApprenticeName = "Jane Doe",
                EmployerAccountId = ExpectedAccountId,
                SendingEmployerAccountId = ReceivingEmployerAccountId,
                MonthlyInstallment = 10,
                NumberOfInstallments = 10,
                CompletionAmount = 100,
                CourseName = "My Course",
                CourseLevel = 1,
                LearnerId = 90,
                ProviderName = "Test Provider",
                ProviderId = 99876,
                FundingSource = FundingSource.Transfer
            });
            _moqer.GetMock<IApplicationConfiguration>().Setup(x => x.LimitForecast).Returns(true);
            _moqer.GetMock<IAccountProjectionDataSession>()
                .Setup(x => x.GetCommitments(It.IsAny<long>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(new List<CommitmentModel>());
            //Act
            var balanceCsv = (await _moqer.Resolve<ForecastingOrchestrator>().ApprenticeshipsCsv("ABBA12")).ToList();

            //Assert
            _moqer.GetMock<IAccountProjectionDataSession>()
                .Verify(x => x.GetCommitments(It.Is<long>(accountId => accountId == ExpectedAccountId), It.Is<DateTime?>(date => date.HasValue && date == DateTime.Parse("2019-05-01"))));
            Assert.IsNotNull(balanceCsv);
            Assert.IsEmpty(balanceCsv);

        }

        [Test]
        public async Task Then_The_Transfer_Employer_Is_Populated_If_You_Are_The_Sender()
        {
            //Act
            var apprenticeshipCsv = (await _moqer.Resolve<ForecastingOrchestrator>().ApprenticeshipsCsv("ABBA12")).ToList();

            //Assert
            Assert.AreEqual("N", apprenticeshipCsv[0].TransferToEmployer);
            Assert.AreEqual("Y", apprenticeshipCsv[1].TransferToEmployer);
        }

        [Test]
        public async Task Then_The_Csv_Dates_Are_Formatted_As_Month_And_Year()
        {
            //Act
            var apprenticeshipCsv = (await _moqer.Resolve<ForecastingOrchestrator>().ApprenticeshipsCsv("ABBA12")).ToList();

            //Assert
            var actualFirstCommitment = apprenticeshipCsv.First();
            Assert.IsNotNull(actualFirstCommitment);
            Assert.AreEqual(DateTime.Now.AddMonths(-1).ToString("MMM-yy"), actualFirstCommitment.StartDate);
            Assert.AreEqual(DateTime.Now.AddMonths(12).ToString("MMM-yy"), actualFirstCommitment.PlannedEndDate);
        }


        [Test]
        public async Task Then_If_It_Is_Not_A_Transfer_Then_The_Transfer_To_Employer_Field_Is_Empty()
        {
            //Act
            var apprenticeshipCsv = (await _moqer.Resolve<ForecastingOrchestrator>().ApprenticeshipsCsv("ABBA12")).ToList();

            //Assert
            var actualFirstCommitment = apprenticeshipCsv.First();
            Assert.IsNotNull(actualFirstCommitment);
            Assert.AreEqual("N", actualFirstCommitment.TransferToEmployer);
        }

        [Test]
        public async Task Should_not_Take_months_after_2019_05_01()
        {
            var balanceMaxDate = new DateTime(2019, 05, 01);

            SetUpProjections(48 + 10);
            _moqer.GetMock<IApplicationConfiguration>().Setup(m => m.LimitForecast)
                .Returns(true);

            var balance = await _moqer.Resolve<ForecastingOrchestrator>().Balance("ABBA12");

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
            _moqer.GetMock<IApplicationConfiguration>().Setup(m => m.LimitForecast)
                .Returns(true);

            var balanceCsv = await _moqer.Resolve<ForecastingOrchestrator>().BalanceCsv("ABBA12");

            balanceCsv.All(m => (DateTime.Parse(m.Date)) < balanceMaxDate).Should().BeTrue();
            balanceCsv.Count().Should().BeGreaterOrEqualTo(1);
            if (DateTime.Today >= balanceMaxDate)
                Assert.IsTrue(false,
                    $"balanceMaxDate is out of date ({balanceMaxDate.ToString()}) and test can be removed");
        }

        [TestCase]
        public async Task ShouldTake_48_months_csv_file()
        {
            var expectedProjections = 48;
            SetUpProjections(48 + 10);
            var projections = await _moqer.Resolve<ForecastingOrchestrator>().BalanceCsv("ABBA12");

            projections.Count().Should().Be(expectedProjections);
            
            _moqer.GetMock<IAccountProjectionDataSession>().Verify(m => m.Get(ExpectedAccountId), Times.Once);
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
                m.Month = (short)date.Month;
                m.Year = date.Year;

                currentMonthDate = currentMonthDate.AddMonths(1);
            });

            _moqer.GetMock<IAccountProjectionDataSession>().Setup(m => m.Get(ExpectedAccountId)).ReturnsAsync(projections);
        }
    }
}