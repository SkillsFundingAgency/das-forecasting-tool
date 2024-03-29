using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Projections.Services;
using SFA.DAS.Forecasting.Models.Projections;
using SFA.DAS.Forecasting.Web.Orchestrators;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using SFA.DAS.Encoding;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.Forecasting.Domain.Commitments.Services;
using SFA.DAS.Forecasting.Models.Balance;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Payments;
using SFA.DAS.Forecasting.Domain.Balance;

namespace SFA.DAS.Forecasting.Web.UnitTests.OrchestratorTests
{
    [TestFixture]
    public class ForecastingOrchestratorTests
    {
        private EmployerCommitmentsModel _commitments;
        private BalanceModel _balance;
        private Mock<ICommitmentsDataService> _commitmentsDataService;
        private Mock<IEncodingService> _hashingService;
        private Mock<IAccountBalanceService> _accountBalanceService;
        private Mock<ICurrentBalanceRepository> _currentBalanceRepository;
        private ForecastingMapper _forecastingMapper;
        private ForecastingOrchestrator _orchestrator;
        private Mock<IAccountProjectionDataSession> _accountProjectionDataSession;
        private const long ExpectedAccountId = 12345;
        private const long ReceivingEmployerAccountId = 554400;

        [SetUp]
        public void SetUp()
        {
            _commitments = new EmployerCommitmentsModel{ LevyFundedCommitments = new List<CommitmentModel>
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
                    FundingSource = FundingSource.Levy,
                    HasHadPayment = true
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
                    LearnerId = 997,
                    ProviderName = "Test Provider",
                    ProviderId = 99876,
                    FundingSource = FundingSource.Transfer,
                    HasHadPayment = true
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
                    LearnerId = 998,
                    ProviderName = "Test Provider",
                    ProviderId = 99876,
                    FundingSource = FundingSource.Levy,
                    HasHadPayment = false
                },
                new CommitmentModel
                {
                    StartDate = DateTime.Now.AddMonths(-2),
                    PlannedEndDate = DateTime.Now.AddMonths(12),
                    ApprenticeName = "Jane Doe",
                    EmployerAccountId = ReceivingEmployerAccountId,
                    SendingEmployerAccountId = ExpectedAccountId,
                    MonthlyInstallment = 10,
                    NumberOfInstallments = 10,
                    CompletionAmount = 100,
                    CourseName = "My Course",
                    CourseLevel = 1,
                    LearnerId = 999,
                    ProviderName = "Test Provider",
                    ProviderId = 99876,
                    FundingSource = FundingSource.Levy,
                    HasHadPayment = false
                }
            }
            };
            _commitmentsDataService = new Mock<ICommitmentsDataService>();
            _commitmentsDataService.Setup(x => x.GetCurrentCommitments(It.IsAny<long>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(_commitments);

            _hashingService = new Mock<IEncodingService>();
            _hashingService
                .Setup(m => m.Decode("ABBA12", EncodingType.AccountId))
                .Returns(ExpectedAccountId);
            _hashingService
                .Setup(m => m.Decode("CDDC12", EncodingType.AccountId))
                .Returns(ReceivingEmployerAccountId);

            _balance = new BalanceModel { EmployerAccountId = 12345, Amount = 50000, TransferAllowance = 5000, RemainingTransferBalance = 5000, UnallocatedCompletionPayments = 2000 };
            _accountBalanceService = new  Mock<IAccountBalanceService>();
            _accountBalanceService.Setup(m => m.GetAccountBalance(It.IsAny<long>()))
                .ReturnsAsync(_balance);

            var currentBalance = new CurrentBalance(_balance, _accountBalanceService.Object , new Domain.Commitments.EmployerCommitments(12345, new EmployerCommitmentsModel()));

            _currentBalanceRepository = new Mock<ICurrentBalanceRepository>();
            _currentBalanceRepository.Setup(x => x.Get(It.IsAny<long>()))
                .ReturnsAsync(currentBalance);

            _forecastingMapper = new ForecastingMapper();
            _accountProjectionDataSession = new Mock<IAccountProjectionDataSession>();

            _orchestrator = new ForecastingOrchestrator(_hashingService.Object,
                _accountProjectionDataSession.Object, _currentBalanceRepository.Object,
                new ForecastingConfiguration(), _forecastingMapper, _commitmentsDataService.Object);
        }

        [Test]
        public async Task ShouldTake_48_months_projections()
        {
            SetUpProjections(48 + 10);

            var balance = await _orchestrator.Projection("ABBA12");

            balance.BalanceItemViewModels.Count().Should().Be(48);
        }

        [Test]
        public async Task Should_Take_Commitments_Used_For_projection_To_Build_Csv_File_At_The_Active_Commitment_Level()
        {
            var apprenticesCsv = await _orchestrator.ApprenticeshipsCsv("ABBA12");
            apprenticesCsv.Count().Should().Be(4);
            _commitmentsDataService.Verify(x => x.GetCurrentCommitments(It.Is<long>(id => id == ExpectedAccountId), It.IsAny<DateTime?>()), Times.Once);
        }

        [Test]
        public async Task Then_The_Model_Is_Returned_With_The_Costs_Are_Calculated_And_Rounded_Correctly()
        {
            //Act
            var apprenticeshipCsv = (await _orchestrator.ApprenticeshipsCsv("ABBA12")).ToList();

            //Assert
            Assert.IsNotNull(apprenticeshipCsv);
            Assert.IsNotEmpty(apprenticeshipCsv);
            var firstApprenticeship = apprenticeshipCsv.Single(m => m.Uln == "90");
            Assert.IsNotNull(firstApprenticeship);
            Assert.AreEqual(207, firstApprenticeship.TotalCost);
            Assert.AreEqual(11, firstApprenticeship.MonthlyTrainingCost);
            Assert.AreEqual(101, firstApprenticeship.CompletionAmount);
        }

        [Test]
        public async Task Then_The_Model_Is_Returned_With_The_ApprenticeshipName_And_Uln_Are_Blank_If_You_Are_The_Sending_Employer()
        {
            //Act
            var apprenticeshipCsv = (await _orchestrator.ApprenticeshipsCsv("ABBA12")).ToList();

            //Assert
            var lastApprenticeship = apprenticeshipCsv.Single(m => m.Uln == "");
            Assert.IsNotNull(lastApprenticeship);
            Assert.IsEmpty(lastApprenticeship.ApprenticeName);
            Assert.IsEmpty(lastApprenticeship.Uln);
        }

        [Test]
        public async Task Then_If_I_Am_The_Receiving_Employer_Of_A_Transfer_The_Apprentice_Data_Is_Visible()
        {
            //Act
            var apprenticeshipCsv = (await _orchestrator.ApprenticeshipsCsv("CDDC12")).ToList();

            //Assert
            var firstApprenticeship = apprenticeshipCsv.Last();
            Assert.IsNotNull(firstApprenticeship);
            Assert.IsNotEmpty(firstApprenticeship.ApprenticeName);
            Assert.IsNotEmpty(firstApprenticeship.Uln);
        }

        [Test]
        public async Task Then_The_Transfer_Employer_Is_Populated_If_You_Are_The_Sender()
        {
            //Act
            var apprenticeshipCsv = (await _orchestrator.ApprenticeshipsCsv("ABBA12")).ToList();

            //Assert
            Assert.AreEqual("N", apprenticeshipCsv.Single(m => m.Uln == "90").TransferToEmployer);
            Assert.AreEqual("Y", apprenticeshipCsv.Single(m => m.Uln == "").TransferToEmployer);
        }

        [Test]
        public async Task Then_The_Csv_Dates_Are_Formatted_As_Month_And_Year()
        {
            //Act
            var apprenticeshipCsv = (await _orchestrator.ApprenticeshipsCsv("ABBA12")).ToList();

            //Assert
            var actualFirstCommitment = apprenticeshipCsv.Single(m => m.Uln == "90");
            Assert.IsNotNull(actualFirstCommitment);
            Assert.AreEqual(DateTime.Now.AddMonths(-1).ToString("MMM-yy"), actualFirstCommitment.StartDate);
            Assert.AreEqual(DateTime.Now.AddMonths(12).ToString("MMM-yy"), actualFirstCommitment.PlannedEndDate);
        }


        [Test]
        public async Task Then_If_It_Is_Not_A_Transfer_Then_The_Transfer_To_Employer_Field_Is_Empty()
        {
            //Act
            var apprenticeshipCsv = (await _orchestrator.ApprenticeshipsCsv("ABBA12")).ToList();

            //Assert
            var actualFirstCommitment = apprenticeshipCsv.Single(m => m.Uln == "90");
            Assert.IsNotNull(actualFirstCommitment);
            Assert.AreEqual("N", actualFirstCommitment.TransferToEmployer);
        }

        [Test]
        public async Task ShouldTake_48_months_csv_file()
        {
            var expectedProjections = 48;
            SetUpProjections(48 + 10);
            var projections = await _orchestrator.BalanceCsv("ABBA12");

            projections.Count().Should().Be(expectedProjections);
            
            _accountProjectionDataSession.Verify(m => m.Get(ExpectedAccountId), Times.Once);
        }

        [Test]
        public async Task Should_start_with_projections_from_next_month()
        {
            var expectedProjections = 48;
            SetUpProjections(48 + 10);

            var projections = await _orchestrator.BalanceCsv("ABBA12");

            projections.Count().Should().Be(expectedProjections);
            projections
                .First()
                .Date
                .Should().Be(DateTime.Now.AddMonths(1).ToString("MMM yy"));
        }

        [Test]
        public async Task Started_not_paid_commitment_must_only_have_DAS_date()
        {
            //Act
            var apprenticeshipCsv = (await _orchestrator.ApprenticeshipsCsv("ABBA12")).ToList();

            //Assert
            var notStartedNotPaid = apprenticeshipCsv.Single(m => m.Uln == "998");

            Assert.IsNotNull(notStartedNotPaid);

            notStartedNotPaid.StartDate.Should().BeNullOrEmpty();
            notStartedNotPaid.PlannedEndDate.Should().BeNullOrEmpty();

            Assert.AreEqual(DateTime.Now.AddMonths(2).ToString("MMM-yy"), notStartedNotPaid.DasStartDate);
            Assert.AreEqual(DateTime.Now.AddMonths(12).ToString("MMM-yy"), notStartedNotPaid.DasPlannedEndDate);
        }

        [Test]
        public async Task Not_started_not_paid_commitment_must_only_have_DAS_date()
        {
            //Act
            var apprenticeshipCsv = (await _orchestrator.ApprenticeshipsCsv("ABBA12")).ToList();

            //Assert
            var startedNotPaid = apprenticeshipCsv.Single(m => m.Uln == "999");

            Assert.IsNotNull(startedNotPaid);

            startedNotPaid.StartDate.Should().BeNullOrEmpty();
            startedNotPaid.PlannedEndDate.Should().BeNullOrEmpty();

            Assert.AreEqual(DateTime.Now.AddMonths(-2).ToString("MMM-yy"), startedNotPaid.DasStartDate);
            Assert.AreEqual(DateTime.Now.AddMonths(12).ToString("MMM-yy"), startedNotPaid.DasPlannedEndDate);
        }

        [Test]
        public async Task Commitments_must_have_correct_status()
        {
            var apprenticeshipCsv = (await _orchestrator.ApprenticeshipsCsv("ABBA12")).ToList();

            var paid = apprenticeshipCsv.Single(m => m.Uln == "90");
            var notStartedNotPaid = apprenticeshipCsv.Single(m => m.Uln == "998");
            var startedNotPaid = apprenticeshipCsv.Single(m => m.Uln == "999");

            paid.Status.Should().Be("Live and paid");
            notStartedNotPaid.Status.Should().Be("Waiting to start");
            startedNotPaid.Status.Should().Be("Live and not paid");
        }

        /// <summary>
        /// Setting up projections starting from last month. 
        /// </summary>
        /// <param name="count">Amount months to create starting from last month.</param>
        private void SetUpProjections(int count)
        {
            var currentMonthDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
            var projections = new List<AccountProjectionModel>();
            for (var i = 0; i < count; i++)
            {
                projections.Add(new AccountProjectionModel
                {
                    Month = (short) currentMonthDate.Month,
                    Year = currentMonthDate.Year,
                    EmployerAccountId = ExpectedAccountId
                });
                currentMonthDate = currentMonthDate.AddMonths(1);
            }

            _accountProjectionDataSession.Setup(m => m.Get(ExpectedAccountId)).ReturnsAsync(projections);
        }
    }
}