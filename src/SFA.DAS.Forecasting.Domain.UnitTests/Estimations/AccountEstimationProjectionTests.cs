using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMoq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Domain.Shared;
using SFA.DAS.Forecasting.Models.Balance;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Estimations
{
    public class AccountEstimationProjectionTests
    {
        private const int EmployerAccountId = 12345;
        private const int TransferAllowance = 15000;
        private AutoMoqer _moqer;
        private EmployerCommitmentsModel _commitments;
        private List<AccountProjectionModel> _accountProjection;
        private Account _account;

        [SetUp]
        public void SetUp()
        {
            _moqer = new AutoMoqer();
            _commitments = new EmployerCommitmentsModel
            {
                SendingEmployerTransferCommitments = 
                {
                    new CommitmentModel
                    {
                        CompletionAmount = 100,
                        EmployerAccountId = EmployerAccountId,
                        MonthlyInstallment = 50,
                        PlannedEndDate = new DateTime(2018, 5, 1),
                        StartDate = new DateTime(2018, 1, 1),
                        NumberOfInstallments = 5,
                        FundingSource = Models.Payments.FundingSource.Levy
                    },
                    new CommitmentModel
                    {
                        CompletionAmount = 100,
                        EmployerAccountId = EmployerAccountId,
                        MonthlyInstallment = 50,
                        PlannedEndDate = new DateTime(2019, 7, 1),
                        StartDate = new DateTime(2019, 3, 1),
                        NumberOfInstallments = 5,
                        FundingSource = Models.Payments.FundingSource.Levy
                    },
                    new CommitmentModel
                    {
                        CompletionAmount = 100,
                        EmployerAccountId = EmployerAccountId,
                        MonthlyInstallment = 50,
                        PlannedEndDate = new DateTime(2018, 5, 1),
                        StartDate = new DateTime(2018, 1, 1),
                        NumberOfInstallments = 5,
                        FundingSource = Models.Payments.FundingSource.Transfer
                    },
                }
            };
            var employerCommitments = new EmployerCommitments(EmployerAccountId, _commitments);

            _accountProjection = new List<AccountProjectionModel>
            {
                new AccountProjectionModel
                {
                    EmployerAccountId = 54321,
                    Month = 1,
                    Year = 2018,
                    LevyFundsIn = 100,
                    TransferOutCostOfTraining = 10,
                    TransferOutCompletionPayments = 0,
                    LevyFundedCompletionPayments = 50,
                    LevyFundedCostOfTraining = 50,
                    FutureFunds = 500
                },
                new AccountProjectionModel
                {
                    EmployerAccountId = 54321,
                    Month = 2,
                    Year = 2018,
                    LevyFundsIn = 100,
                    TransferOutCostOfTraining = 10,
                    TransferOutCompletionPayments = 0,
                    LevyFundedCompletionPayments = 0,
                    LevyFundedCostOfTraining = 50,
                    FutureFunds = 540
                },
                new AccountProjectionModel
                {
                    EmployerAccountId = 54321,
                    Month = 3,
                    Year = 2018,
                    LevyFundsIn = 100,
                    TransferOutCostOfTraining = 10,
                    TransferOutCompletionPayments = 0,
                    LevyFundedCompletionPayments = 0,
                    LevyFundedCostOfTraining = 50,
                    FutureFunds = 580
                },
                new AccountProjectionModel
                {
                    EmployerAccountId = 54321,
                    Month = 4,
                    Year = 2018,
                    LevyFundsIn = 100,
                    TransferOutCostOfTraining = 10,
                    TransferOutCompletionPayments = 0,
                    LevyFundedCompletionPayments = 0,
                    LevyFundedCostOfTraining = 50,
                    FutureFunds = 620
                },
                new AccountProjectionModel
                {
                    EmployerAccountId = 54321,
                    Month = 5,
                    Year = 2018,
                    LevyFundsIn = 100,
                    TransferOutCostOfTraining = 10,
                    TransferOutCompletionPayments = 0,
                    LevyFundedCompletionPayments = 0,
                    LevyFundedCostOfTraining = 50,
                    FutureFunds = 660
                },
                new AccountProjectionModel
                {
                    EmployerAccountId = 54321,
                    Month = 6,
                    Year = 2018,
                    LevyFundsIn = 100,
                    TransferOutCostOfTraining = 0,
                    TransferOutCompletionPayments = 20,
                    LevyFundedCompletionPayments = 0,
                    LevyFundedCostOfTraining = 50,
                    FutureFunds = 690
                }
            };

            var accountEstimationProjectionCommitments =
                new AccountEstimationProjectionCommitments(employerCommitments, _accountProjection.AsReadOnly());

            _moqer.GetMock<IDateTimeService>()
                .Setup(x => x.GetCurrentDateTime()).Returns(new DateTime(2018, 2, 1));

            _moqer.SetInstance(accountEstimationProjectionCommitments);

            _account = new Account(EmployerAccountId, 10000, 0, TransferAllowance, 10000);
            _moqer.SetInstance(_account);
        }

        [Test]
        public void Then_The_Projections_Are_Assigned_To_The_AccountEstimationProjectionModel()
        {
            var estimationProjection = _moqer.Resolve<AccountEstimationProjection>();
            estimationProjection.BuildProjections();

            Assert.IsNotNull(estimationProjection);
            Assert.IsAssignableFrom<ReadOnlyCollection<AccountEstimationProjectionModel>>(estimationProjection
                .Projections);
        }

        [Test]
        public void First_Month_Is_Current_Month()
        {
            _moqer.SetInstance<IDateTimeService>(new DateTimeService());
            var estimationProjection = _moqer.Resolve<AccountEstimationProjection>();

            estimationProjection.BuildProjections();
            var projection = estimationProjection.Projections.FirstOrDefault();

            Assert.IsNotNull(projection);
            Assert.IsTrue(projection.Month == DateTime.Now.Month && projection.Year == DateTime.Now.Year);
        }

        [Test]
        public void Last_Month_Should_Be_Last_Commitment_Date()
        {
            var estimationProjection = _moqer.Resolve<AccountEstimationProjection>();
            estimationProjection.BuildProjections();
            var projection = estimationProjection.Projections.LastOrDefault();
            Assert.IsNotNull(projection);
            Assert.AreEqual(9, projection.Month, $"Expected to end in month 9 but last month was {projection.Month}");
        }

        [Test]
        public void Transfer_Balance_Should_Be_Reset_To_Transfer_Allowance_Each_May()
        {
            var estimationProjection = _moqer.Resolve<AccountEstimationProjection>();
            estimationProjection.BuildProjections();
            estimationProjection.Projections.Where(p => p.Month == 5).ToList()
                .ForEach(p => Assert.AreEqual(_account.TransferAllowance + p.TransferFundsIn - p.TransferFundsOut,
                    p.AvailableTransferFundsBalance,
                    $"Invalid transfer projection month. Year: {p.Year}, Expected balance: {_account.TransferAllowance + p.TransferFundsIn - p.TransferModelledCosts.FundsOut}, actual: {p.AvailableTransferFundsBalance}"));
        }

        [Test]
        public void Ensure_Correct_Future_Funds()
        {
            var estimationProjection = _moqer.Resolve<AccountEstimationProjection>();

            estimationProjection.BuildProjections();
            var lastBalance = _account.RemainingTransferBalance;
            foreach (var estimation in estimationProjection.Projections)
            {
                Assert.IsNotNull(estimation);
                Assert.AreEqual(
                    (estimation.Month == 5 ? _account.TransferAllowance : lastBalance) + estimation.TransferFundsIn - estimation.TransferFundsOut,
                    estimation.AvailableTransferFundsBalance);
                lastBalance = estimation.AvailableTransferFundsBalance;
            }
        }

        [Test]
        public void Then_The_AccountFunds_Estimation_Uses_The_Projected_Balance()
        {
            //Arrange
            _moqer.GetMock<IDateTimeService>()
                .Setup(x => x.GetCurrentDateTime()).Returns(new DateTime(2018, 1, 1));
            var estimationProjection = _moqer.Resolve<AccountEstimationProjection>();

            //Act
            estimationProjection.BuildProjections();

            //Assert
            var actual = estimationProjection.Projections.OrderBy(c=>c.Year).ThenBy(c=>c.Month).FirstOrDefault();
            Assert.IsNotNull(actual);
            Assert.AreEqual(_accountProjection.First().FutureFunds, actual.EstimatedProjectionBalance);
        }

        [Test]
        public void Then_The_Estimation_Uses_Actual_And_Modelled_Costs_Against_My_Transfer_Allowance()
        {
            //Arrange
            var estimationProjection = _moqer.Resolve<AccountEstimationProjection>();

            //Act
            estimationProjection.BuildProjections();

            //Assert
            var actual = estimationProjection.Projections.OrderBy(c => c.Year).ThenBy(c => c.Month).FirstOrDefault();
            Assert.IsNotNull(actual);
            Assert.AreEqual(9940 , actual.AvailableTransferFundsBalance);
        }

        [Test]
        public void Then_The_Estimation_Only_Includes_Modelled_Transfer_Estimates_And_Not_Projection_Estimates()
        {
            //Arrange
            var commitments = new EmployerCommitmentsModel
            {
                SendingEmployerTransferCommitments =
                {
                    new CommitmentModel
                    {
                        CompletionAmount = 100,
                        EmployerAccountId = EmployerAccountId,
                        MonthlyInstallment = 50,
                        PlannedEndDate = new DateTime(2018, 5, 1),
                        StartDate = new DateTime(2018, 1, 1),
                        NumberOfInstallments = 5,
                        FundingSource = Models.Payments.FundingSource.Levy
                    }
                }
            };
            var employerCommitments = new EmployerCommitments(EmployerAccountId, commitments);
            var accountEstimationProjectionCommitments =
                new AccountEstimationProjectionCommitments(employerCommitments, _accountProjection.AsReadOnly());
            _moqer.SetInstance(accountEstimationProjectionCommitments);
            var estimationProjection = _moqer.Resolve<AccountEstimationProjection>();

            //Act
            estimationProjection.BuildProjections();

            //Assert
            var actual = estimationProjection.Projections.OrderBy(c => c.Year).ThenBy(c => c.Month).FirstOrDefault();
            Assert.IsNotNull(actual);
            Assert.AreEqual(9990, actual.AvailableTransferFundsBalance);
        }

        [Test]
        public void Then_Transfer_Funds_In_Are_Reflected_In_My_Balance()
        {
            //Arrange
            _accountProjection = new List<AccountProjectionModel>
            {
                new AccountProjectionModel
                {
                    EmployerAccountId = 54321,
                    Month = 1,
                    Year = 2018,
                    LevyFundsIn = 100,
                    TransferOutCostOfTraining = 10,
                    TransferInCostOfTraining = 10,
                    TransferOutCompletionPayments = 0,
                    LevyFundedCompletionPayments = 50,
                    LevyFundedCostOfTraining = 50,
                    FutureFunds = 500
                },
                new AccountProjectionModel
                {
                    EmployerAccountId = 54321,
                    Month = 2,
                    Year = 2018,
                    LevyFundsIn = 100,
                    TransferOutCostOfTraining = 10,
                    TransferInCostOfTraining = 10,
                    TransferOutCompletionPayments = 0,
                    LevyFundedCompletionPayments = 0,
                    LevyFundedCostOfTraining = 50,
                    FutureFunds = 540
                }
            };
            _moqer.GetMock<IDateTimeService>()
                .Setup(x => x.GetCurrentDateTime()).Returns(new DateTime(2018, 1, 1));
            var employerCommitments = new EmployerCommitments(EmployerAccountId, _commitments);
            var accountEstimationProjectionCommitments =
                new AccountEstimationProjectionCommitments(employerCommitments, _accountProjection.AsReadOnly());
            _moqer.SetInstance(accountEstimationProjectionCommitments);
            var estimationProjection = _moqer.Resolve<AccountEstimationProjection>();

            //Act
            estimationProjection.BuildProjections();

            //Assert
            var actual = estimationProjection.Projections.OrderBy(c => c.Year).ThenBy(c => c.Month).Skip(1).FirstOrDefault();
            Assert.IsNotNull(actual);
            Assert.AreEqual(350, actual.EstimatedProjectionBalance);
        }
    }
}