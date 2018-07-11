﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMoq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Models.Balance;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Projection
{
    [TestFixture]
    public class AccountProjectionTests
    {
        protected AutoMoqer Moqer { get; private set; }
        private Account _account;
        private CommitmentModel _commitment;
        private EmployerCommitmentsModel _commitments;

        [SetUp]
        public void SetUp()
        {
            Moqer = new AutoMoqer();
            _account = new Account(1, 12000, 300, 0, 0);
            Moqer.SetInstance(_account);
            _commitment = new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = DateTime.Today.AddMonths(-1),
                PlannedEndDate = DateTime.Today.AddMonths(25),
                MonthlyInstallment = 2100,
                NumberOfInstallments = 24,
                CompletionAmount = 3000,
                FundingSource = Models.Payments.FundingSource.Levy
            };
            _commitments = new EmployerCommitmentsModel
            {
                LevyFundedCommitments = new List<CommitmentModel>
                {
                    _commitment
                }
            };
            var employerCommitments = new EmployerCommitments(1, _commitments);
            Moqer.SetInstance(employerCommitments);
        }

        [Test]
        public void Starts_From_This_Month()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            accountProjection.BuildLevyTriggeredProjections(DateTime.Today, 1);

            accountProjection.Projections.First().Month.Should().Be((short)DateTime.Today.Month);
        }

        [Test]
        public void Projects_Requested_Number_Of_Months()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            accountProjection.BuildLevyTriggeredProjections(DateTime.Today, 48);

            accountProjection.Projections.Count.Should().Be(49);
        }

        [Test]
        public void Projects_Levy_Forward_For_Requested_Number_Of_Months()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            accountProjection.BuildLevyTriggeredProjections(DateTime.Today, 48);

            accountProjection.Projections.Count(projection => projection.LevyFundsIn == 300).Should().Be(49);
        }

        [Test]
        public void Does_Not_Include_Levy_In_First_Month_For_Levy_Triggered_Projection()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            accountProjection.BuildLevyTriggeredProjections(DateTime.Today, 2);

            accountProjection.Projections.First().FutureFunds.Should().Be(_account.Balance);
        }

        [Test]
        public void Includes_Levy_In_Months_After_First_Month_For_Levy_Triggered_Projection()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            accountProjection.BuildLevyTriggeredProjections(DateTime.Today, 2);

            var expected = accountProjection.Projections.FirstOrDefault()?.FutureFunds + _account.LevyDeclared - _commitment.MonthlyInstallment;
            var funds2ndMonth = accountProjection.Projections.Skip(1).FirstOrDefault()?.FutureFunds;
            funds2ndMonth.Should().Be(expected);
        }

        [Test]
        public void Includes_Completion_Payments()
        {
            _commitment.StartDate = DateTime.Today.AddMonths(-1);
            _commitment.PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(1);

            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            accountProjection.BuildLevyTriggeredProjections(DateTime.Today, 2);

            accountProjection.Projections.Skip(2).First().LevyFundedCompletionPayments
                .Should().Be(_commitment.CompletionAmount);
        }

        [Test]
        public void Includes_Installments()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            accountProjection.BuildLevyTriggeredProjections(DateTime.Today, 2);

            accountProjection.Projections.First().FutureFunds
                .ShouldBeEquivalentTo(_account.Balance, because: "First month should be the same as current balance");

            accountProjection.Projections.Skip(1).First().FutureFunds
                .ShouldBeEquivalentTo(accountProjection.Projections.FirstOrDefault()?.FutureFunds + _account.LevyDeclared - _commitment.MonthlyInstallment);
        }

        [Test]
        public void Includes_Co_Investment()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();

            accountProjection.BuildLevyTriggeredProjections(DateTime.Today, 7);
            accountProjection.Projections[6].CoInvestmentEmployer.Should().Be(0);
            accountProjection.Projections[6].CoInvestmentGovernment.Should().Be(0);
            accountProjection.Projections[6].FutureFunds.Should().Be(1200);

            accountProjection.Projections[7].CoInvestmentEmployer.Should().Be(60);
            accountProjection.Projections[7].CoInvestmentGovernment.Should().Be(540);
            accountProjection.Projections[7].FutureFunds.Should().Be(0);
        }

        [Test]
        public void Receiving_employer_account_has_transfers_in()
        {
            _commitments.LevyFundedCommitments = new List<CommitmentModel>
            {
                new CommitmentModel
                {
                    EmployerAccountId = 1,
                    SendingEmployerAccountId = 1,
                    ApprenticeshipId = 21,
                    LearnerId = 31,
                    StartDate = DateTime.Today,
                    PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(6),
                    MonthlyInstallment = 2000,
                    NumberOfInstallments = 6,
                    CompletionAmount = 1200,
                    FundingSource = Models.Payments.FundingSource.Levy
                },
                new CommitmentModel
                {
                    EmployerAccountId = 1,
                    SendingEmployerAccountId = 1,
                    ApprenticeshipId = 22,
                    LearnerId = 32,
                    StartDate = DateTime.Today,
                    PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(6),
                    MonthlyInstallment = 2000,
                    NumberOfInstallments = 6,
                    CompletionAmount = 1200,
                    FundingSource = Models.Payments.FundingSource.Levy
                }
            };
            _commitments.SendingEmployerTransferCommitments = new List<CommitmentModel>
            {
                new CommitmentModel
                {
                    EmployerAccountId = 1,
                    SendingEmployerAccountId = 999,
                    ApprenticeshipId = 23,
                    LearnerId = 33,
                    StartDate = DateTime.Today,
                    PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(6),
                    MonthlyInstallment = 2000,
                    NumberOfInstallments = 6,
                    CompletionAmount = 1200,
                    FundingSource = Models.Payments.FundingSource.Transfer
                }
            };
            _commitments.ReceivingEmployerTransferCommitments = new List<CommitmentModel>
            {
                new CommitmentModel
                {
                    EmployerAccountId = 999,
                    SendingEmployerAccountId = 1,
                    ApprenticeshipId = 34,
                    LearnerId = 34,
                    StartDate = DateTime.Today,
                    PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(6),
                    MonthlyInstallment = 2000,
                    NumberOfInstallments = 6,
                    CompletionAmount = 1200,
                    FundingSource = Models.Payments.FundingSource.Transfer
                }
            };
        

        var employerCommitments = new EmployerCommitments(1, _commitments);
            Moqer.SetInstance(employerCommitments);

            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();

            accountProjection.BuildLevyTriggeredProjections(DateTime.Today, 12);

            var projections = accountProjection.Projections.ToArray();

            projections[6].LevyFundedCostOfTraining.Should().Be(4000);
            projections[6].TransferInCostOfTraining.Should().Be(2000);
            projections[6].TransferOutCostOfTraining.Should().Be(4000);
            projections[7].LevyFundedCostOfTraining.Should().Be(0);
            projections[7].LevyFundedCompletionPayments.Should().Be(2400);
            projections[7].TransferInCompletionPayments.Should().Be(1200);
            projections[7].TransferOutCompletionPayments.Should().Be(2400);
            projections[8].LevyFundedCostOfTraining.Should().Be(0);
            projections[8].LevyFundedCompletionPayments.Should().Be(0);
        }


        // ------------------------------
        // Payrole Period End Triggered -
        // ------------------------------

        [Test]
        public void Includes_Levy_In_First_Months_For_Payroll_Period_End_Triggered_Projection()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            accountProjection.BuildPayrollPeriodEndTriggeredProjections(DateTime.Today, 2);

            accountProjection.Projections.First().FutureFunds
                .Should().Be(_account.Balance + _account.LevyDeclared);
        }

        [Test]
        public void Includes_Levy_In_First_Months_and_costa_has_been_removed_For_Payroll_Period_End_Triggered_Projection()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            accountProjection.BuildPayrollPeriodEndTriggeredProjections(DateTime.Today, 2);

            var expected = accountProjection.Projections.FirstOrDefault()?.FutureFunds + _account.LevyDeclared - _commitment.MonthlyInstallment;

            accountProjection.Projections.Skip(1).First().FutureFunds
                .Should().Be(expected);
        }

        [Test]
        public void Then_The_Receiving_Employer_FutureFunds_Does_Not_Change_With_Only_Transfers_In()
        {
            //Arrange
            _account = new Account(1, 200, 0, 0, 0);
            Moqer.SetInstance(_account);
            _commitments.LevyFundedCommitments = new List<CommitmentModel>();
            _commitments.SendingEmployerTransferCommitments = new List<CommitmentModel>();
            _commitments.ReceivingEmployerTransferCommitments = new List<CommitmentModel>
            {
                new CommitmentModel
                {
                    EmployerAccountId = 999,
                    SendingEmployerAccountId = 1,
                    ApprenticeshipId = 23,
                    LearnerId = 33,
                    StartDate = DateTime.Today,
                    PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(6),
                    MonthlyInstallment = 2000,
                    NumberOfInstallments = 6,
                    CompletionAmount = 1200,
                    FundingSource = Models.Payments.FundingSource.Transfer
                }
            };
            var employerCommitments = new EmployerCommitments(1, _commitments);
            Moqer.SetInstance(employerCommitments);
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            
            //Act
            accountProjection.BuildPayrollPeriodEndTriggeredProjections(DateTime.Today, 12);

            //Assert
            Assert.IsTrue(accountProjection.Projections.All(c => c.FutureFunds == 200));
        }

        [Test]
        public void Then_When_You_Are_A_Sending_Employer_Your_FutureFunds_Are_Updated()
        {
            //Arrange
            _account = new Account(1, 2000, 0, 0, 0);
            Moqer.SetInstance(_account);
            _commitments.LevyFundedCommitments = new List<CommitmentModel>();
            _commitments.ReceivingEmployerTransferCommitments = new List<CommitmentModel>();
            _commitments.SendingEmployerTransferCommitments = new List<CommitmentModel>
            {
                new CommitmentModel
                {
                    EmployerAccountId = 1,
                    SendingEmployerAccountId = 999,
                    ApprenticeshipId = 23,
                    LearnerId = 33,
                    StartDate = DateTime.Today,
                    PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(6),
                    MonthlyInstallment = 100,
                    NumberOfInstallments = 6,
                    CompletionAmount = 400,
                    FundingSource = Models.Payments.FundingSource.Transfer
                }
            };

            var employerCommitments = new EmployerCommitments(1, _commitments);
            Moqer.SetInstance(employerCommitments);

            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();

            accountProjection.BuildPayrollPeriodEndTriggeredProjections(DateTime.Today, 12);

            //Assert
            Assert.AreEqual(1000m,accountProjection.Projections[7].FutureFunds);
            Assert.AreEqual(1000m,accountProjection.Projections.Last().FutureFunds);
        }

        [Test]
        public void Then_If_I_Am_A_Sending_And_Receiving_Employer_My_Funds_Are_Updated()
        {
            //Arrange
            _account = new Account(1, 2000, 0, 0, 0);
            Moqer.SetInstance(_account);
            _commitments.LevyFundedCommitments = new List<CommitmentModel>();
            _commitments.ReceivingEmployerTransferCommitments = new List<CommitmentModel>
            {
                new CommitmentModel
                {
                    EmployerAccountId = 999,
                    SendingEmployerAccountId = 1,
                    ApprenticeshipId = 23,
                    LearnerId = 33,
                    StartDate = DateTime.Today,
                    PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(6),
                    MonthlyInstallment = 2000,
                    NumberOfInstallments = 6,
                    CompletionAmount = 1200,
                    FundingSource = Models.Payments.FundingSource.Transfer
                }
            };
            _commitments.SendingEmployerTransferCommitments = new List<CommitmentModel>
            {
                new CommitmentModel
                {
                    EmployerAccountId = 1,
                    SendingEmployerAccountId = 999,
                    ApprenticeshipId = 23,
                    LearnerId = 33,
                    StartDate = DateTime.Today,
                    PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(6),
                    MonthlyInstallment = 100,
                    NumberOfInstallments = 6,
                    CompletionAmount = 400,
                    FundingSource = Models.Payments.FundingSource.Transfer
                }
            };

            var employerCommitments = new EmployerCommitments(1, _commitments);
            Moqer.SetInstance(employerCommitments);

            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();

            accountProjection.BuildPayrollPeriodEndTriggeredProjections(DateTime.Today, 12);

            //Assert
            Assert.AreEqual(1000m, accountProjection.Projections[7].FutureFunds);
            Assert.AreEqual(1000m, accountProjection.Projections.Last().FutureFunds);
        }

    }
}