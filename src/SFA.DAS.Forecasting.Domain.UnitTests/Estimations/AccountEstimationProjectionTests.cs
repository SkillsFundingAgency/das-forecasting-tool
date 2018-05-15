using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMoq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Models.Balance;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Estimations
{
    public class AccountEstimationProjectionTests
    {
        private const int EmployerAccountId = 12345;
        private AutoMoq.AutoMoqer _moqer;
        private List<CommitmentModel> _commitments;
        private IList<AccountProjectionModel> _actualTransferCommitments;
        private Account _account;

        [SetUp]
        public void SetUp()
        {
            _moqer = new AutoMoqer();
            _commitments = new List<CommitmentModel>
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
                }
            };
            var employerCommitments = new EmployerCommitments(EmployerAccountId, _commitments);

            _actualTransferCommitments = new List<AccountProjectionModel>
            {
                new AccountProjectionModel
                {
                    EmployerAccountId = 54321,
                    Month = 1,
                    Year = 2018,
                    TransferOutTotalCostOfTraining = 10,
                    TransferOutCompletionPayments = 0
                },
                new AccountProjectionModel
                {
                    EmployerAccountId = 54321,
                    Month = 2,
                    Year = 2018,
                    TransferOutTotalCostOfTraining = 10,
                    TransferOutCompletionPayments = 0
                },
                new AccountProjectionModel
                {
                    EmployerAccountId = 54321,
                    Month = 3,
                    Year = 2018,
                    TransferOutTotalCostOfTraining = 10,
                    TransferOutCompletionPayments = 0
                },
                new AccountProjectionModel
                {
                    EmployerAccountId = 54321,
                    Month = 4,
                    Year = 2018,
                    TransferOutTotalCostOfTraining = 10,
                    TransferOutCompletionPayments = 0
                },
                new AccountProjectionModel
                {
                    EmployerAccountId = 54321,
                    Month = 5,
                    Year = 2018,
                    TransferOutTotalCostOfTraining = 10,
                    TransferOutCompletionPayments = 0
                },
                new AccountProjectionModel
                {
                    EmployerAccountId = 54321,
                    Month = 6,
                    Year = 2018,
                    TransferOutTotalCostOfTraining = 0,
                    TransferOutCompletionPayments = 20
                }
            };

            var accountEstimationProjectionCommitments =
                new AccountEstimationProjectionCommitments(employerCommitments, _actualTransferCommitments);

            _moqer.SetInstance(accountEstimationProjectionCommitments);

            _account = new Account(EmployerAccountId, 10000, 0, 15000, 10000);
            _moqer.SetInstance(_account);
        }

        [Test]
        public void Then_The_Projections_Are_Assigned_To_The_AccountEstimationProjectionModel()
        {
            var estimationProjection = _moqer.Resolve<AccountEstimationProjection>();
            estimationProjection.BuildProjections();

            Assert.IsNotNull(estimationProjection);
            Assert.IsAssignableFrom<ReadOnlyCollection<AccountEstimationProjectionModel>>(estimationProjection.Projections);
        }

        [Test]
        public void First_Month_Should_Be_Earliest_Payment_Date()
        {
            var estimationProjection = _moqer.Resolve<AccountEstimationProjection>();
            estimationProjection.BuildProjections();
            var projection = estimationProjection.Projections.FirstOrDefault();
            Assert.IsNotNull(projection);
            Assert.IsTrue(projection.Month == 2 && projection.Year == 2018);
        }

        [Test]
        public void Last_Month_Should_Be_Last_Commitment_Date()
        {
            var estimationProjection = _moqer.Resolve<AccountEstimationProjection>();
            estimationProjection.BuildProjections();
            var projection = estimationProjection.Projections.LastOrDefault();
            Assert.IsNotNull(projection);
            Assert.AreEqual(8, projection.Month, $"Expected to end in month 8 but last month was {projection.Month}");
        }

        [Test]
        public void Transfer_Balance_Should_Be_Reset_To_Transfer_Allowance_Each_May()
        {
            var estimationProjection = _moqer.Resolve<AccountEstimationProjection>();
            estimationProjection.BuildProjections();
            estimationProjection.Projections.Where(p => p.Month == 5).ToList()
                .ForEach(p => Assert.AreEqual((decimal) (_account.TransferAllowance), p.FutureFunds,
                    $"Invalid transfer projection month. Year: {p.Year}, Expected balance: {_account.TransferAllowance - p.TotalCostOfTraining - p.CompletionPayments}, actual: {p.FutureFunds}"));
        }

        [Test]
        public void First_Month_CommittedTransferCost_Is_Not_Applied_To_The_Balance()
        {
            var estimationProjection = _moqer.Resolve<AccountEstimationProjection>();

            estimationProjection.BuildProjections();
            var actual = estimationProjection.Projections[0].FutureFunds;

            Assert.IsNotNull(actual);
            Assert.AreEqual(10000m, actual);
        }

        [Test]
        public void FutureFunds_Includes_Actual_Transfer_Commitments_Per_Month()
        {
            var estimationProjection = _moqer.Resolve<AccountEstimationProjection>();

            estimationProjection.BuildProjections();
            var projectionFutureFunds = estimationProjection.Projections[1].FutureFunds;

            Assert.IsNotNull(projectionFutureFunds);
            Assert.AreEqual(9940m, projectionFutureFunds);
        }


        [Test]
        public void CommittedTransferCost_Is_Populated_In_The_Projection()
        {
            var estimationProjection = _moqer.Resolve<AccountEstimationProjection>();

            estimationProjection.BuildProjections();
            var projectionCommittedTransferCost =
                estimationProjection.Projections.Take(2).Sum(c => c.ActualCommittedTransferCost);

            Assert.IsNotNull(projectionCommittedTransferCost);
            Assert.AreEqual(20m, projectionCommittedTransferCost);
        }

        [Test]
        public void CommittedTransferCost_Compeletion_Fee_Is_Applied_To_Future_Funds()
        {
            _commitments = new List<CommitmentModel>
            {
                new CommitmentModel
                {
                    CompletionAmount = 100,
                    EmployerAccountId = EmployerAccountId,
                    MonthlyInstallment = 50,
                    PlannedEndDate = new DateTime(2018, 3, 1),
                    StartDate = new DateTime(2018, 1, 1),
                    NumberOfInstallments = 2,
                    FundingSource = Models.Payments.FundingSource.Levy
                }
            };
            var employerCommitments = new EmployerCommitments(EmployerAccountId, _commitments);

            _actualTransferCommitments = new List<AccountProjectionModel>
            {
                new AccountProjectionModel
                {
                    EmployerAccountId = 54321,
                    Month = 1,
                    Year = 2018,
                    TransferOutTotalCostOfTraining = 10,
                    TransferOutCompletionPayments = 0
                },
                new AccountProjectionModel
                {
                    EmployerAccountId = 54321,
                    Month = 2,
                    Year = 2018,
                    TransferOutTotalCostOfTraining = 0,
                    TransferOutCompletionPayments = 20
                }
            };
            
            var accountEstimationProjectionCommitments =
                new AccountEstimationProjectionCommitments(employerCommitments, _actualTransferCommitments);

            _moqer.SetInstance(accountEstimationProjectionCommitments);
            var estimationProjection = _moqer.Resolve<AccountEstimationProjection>();

            estimationProjection.BuildProjections();
            var actual = estimationProjection.Projections.FirstOrDefault(c => c.Month == 4 && c.Year == 2018);

            Assert.IsNotNull(actual);
            Assert.AreEqual(9880, actual.FutureFunds);
        }
    }
}