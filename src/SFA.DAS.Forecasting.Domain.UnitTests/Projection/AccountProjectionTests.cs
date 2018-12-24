using System;
using System.Collections.Generic;
using System.Linq;
using AutoMoq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Models.Balance;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Projections;

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

            accountProjection.Projections.First().Month.Should().Be((short) DateTime.Today.Month);
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
            accountProjection.BuildLevyTriggeredProjections(new DateTime(2018, 10, 20), 2);

            accountProjection.Projections.First().FutureFunds.Should().Be(_account.Balance);
        }

        [Test]
        public void Includes_Levy_In_Months_After_First_Month_For_Levy_Triggered_Projection()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            accountProjection.BuildLevyTriggeredProjections(DateTime.Today, 2);

            var expected = accountProjection.Projections.FirstOrDefault()?.FutureFunds + _account.LevyDeclared -
                           _commitment.MonthlyInstallment;
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
            accountProjection.BuildLevyTriggeredProjections(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 20), 2);

            accountProjection.Projections.First().FutureFunds
                .ShouldBeEquivalentTo(_account.Balance, because: "First month should be the same as current balance");

            accountProjection.Projections.Skip(1).First().FutureFunds
                .ShouldBeEquivalentTo(accountProjection.Projections.FirstOrDefault()?.FutureFunds +
                                      _account.LevyDeclared - _commitment.MonthlyInstallment);
        }

        [Test]
        public void Includes_Co_Investment()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();

            accountProjection.BuildLevyTriggeredProjections(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 20), 7);

            accountProjection.Projections[6].CoInvestmentEmployer.Should().Be(0);
            accountProjection.Projections[6].CoInvestmentGovernment.Should().Be(0);
            accountProjection.Projections[6].FutureFunds.Should().Be(1200);

            accountProjection.Projections[7].CoInvestmentEmployer.Should().Be(90);
            accountProjection.Projections[7].CoInvestmentGovernment.Should().Be(810);
            accountProjection.Projections[7].FutureFunds.Should().Be(300);
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
        public void Includes_Levy_In_First_Months_For_Payroll_Period_End_Triggered_Projection_Less_Costs()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            accountProjection.BuildPayrollPeriodEndTriggeredProjections(DateTime.Today, 2);

            accountProjection.Projections.First().FutureFunds
                .Should().Be(_account.Balance + _account.LevyDeclared); // - _commitment.MonthlyInstallment);
        }

        [Test]
        public void Includes_Levy_In_First_Months_and_costa_has_been_removed_For_Payroll_Period_End_Triggered_Projection()
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            accountProjection.BuildPayrollPeriodEndTriggeredProjections(DateTime.Today, 2);

            var expected = accountProjection.Projections.FirstOrDefault()?.FutureFunds + _account.LevyDeclared -
                           _commitment.MonthlyInstallment;

            accountProjection.Projections.Skip(1).First().FutureFunds
                .Should().Be(expected);
        }


        [Test]
        public void Then_The_CoInvestment_Is_Calculated_Correctly_For_The_First_Month_With_A_Postive_Balance_And_An_Affordable_Commitment_For_Payment_Run()
        {
            //Arrange
            CreateCommitmentModel(200, 300);

            _account = new Account(1, 300, 400, 0, 0);
            Moqer.SetInstance(_account);
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();

            //Act
            accountProjection.BuildPayrollPeriodEndTriggeredProjections(DateTime.Today, 2);

            //Assert
            var expectedMonth1 = accountProjection.Projections.FirstOrDefault();
            Assert.AreEqual(0, expectedMonth1?.CoInvestmentGovernment);
            Assert.AreEqual(0, expectedMonth1?.CoInvestmentEmployer);
            Assert.AreEqual(700, expectedMonth1?.FutureFunds);

            var expectedMonth2 = accountProjection.Projections.Skip(1).FirstOrDefault();
            Assert.AreEqual(0, expectedMonth2?.CoInvestmentGovernment);
            Assert.AreEqual(0, expectedMonth2?.CoInvestmentEmployer);
            Assert.AreEqual(900, expectedMonth2?.FutureFunds);

            var expectedMonth3 = accountProjection.Projections.Skip(2).FirstOrDefault();
            Assert.AreEqual(0, expectedMonth3?.CoInvestmentGovernment);
            Assert.AreEqual(0, expectedMonth3?.CoInvestmentEmployer);
            Assert.AreEqual(1100, expectedMonth3?.FutureFunds);
        }

        [Test]
        public void Then_The_CoInvestment_Is_Calculated_Correctly_For_The_First_Month_With_A_Postive_Balance_And_An_Affordable_Commitment_For_Levy_Run()
        {
            //Arrange
            CreateCommitmentModel(200, 300);

            _account = new Account(1, 300, 400, 0, 0);
            Moqer.SetInstance(_account);
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();

            //Act
            accountProjection.BuildLevyTriggeredProjections(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 20), 2);

            //Assert
            var expectedMonth1 = accountProjection.Projections.FirstOrDefault();
            Assert.AreEqual(0, expectedMonth1?.CoInvestmentGovernment);
            Assert.AreEqual(0, expectedMonth1?.CoInvestmentEmployer);
            Assert.AreEqual(300, expectedMonth1?.FutureFunds);

            var expectedMonth2 = accountProjection.Projections.Skip(1).FirstOrDefault();
            Assert.AreEqual(0, expectedMonth2?.CoInvestmentGovernment);
            Assert.AreEqual(0, expectedMonth2?.CoInvestmentEmployer);
            Assert.AreEqual(500, expectedMonth2?.FutureFunds);

            var expectedMonth3 = accountProjection.Projections.Skip(2).FirstOrDefault();
            Assert.AreEqual(0, expectedMonth3?.CoInvestmentGovernment);
            Assert.AreEqual(0, expectedMonth3?.CoInvestmentEmployer);
            Assert.AreEqual(700, expectedMonth3?.FutureFunds);
        }


        [Test]
        public void Then_The_CoInvestment_Is_Calculated_Correctly_For_The_First_Month_With_A_Postive_Balance_And_An_Nonaffordable_Commitment_For_Levy_Run()
        {
            //Arrange
            CreateCommitmentModel(600, 700);

            _account = new Account(1, 300, 400, 0, 0);
            Moqer.SetInstance(_account);
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();

            //Act
            accountProjection.BuildLevyTriggeredProjections(new DateTime(DateTime.Today.Year, DateTime.Today.Month,20), 2);

            //Assert
            var expectedMonth1 = accountProjection.Projections.FirstOrDefault();
            Assert.AreEqual(0.9 * 300, expectedMonth1?.CoInvestmentGovernment);
            Assert.AreEqual(0.1 * 300, expectedMonth1?.CoInvestmentEmployer);
            Assert.AreEqual(300, expectedMonth1?.FutureFunds);

            var expectedMonth2 = accountProjection.Projections.Skip(1).FirstOrDefault();
            Assert.AreEqual(0.9 * 300, expectedMonth2?.CoInvestmentGovernment);
            Assert.AreEqual(0.1 * 300, expectedMonth2?.CoInvestmentEmployer);
            Assert.AreEqual(400, expectedMonth2?.FutureFunds);

            var expectedMonth3 = accountProjection.Projections.Skip(2).FirstOrDefault();
            Assert.AreEqual(0.9 * 200, expectedMonth3?.CoInvestmentGovernment);
            Assert.AreEqual(0.1 * 200, expectedMonth3?.CoInvestmentEmployer);
            Assert.AreEqual(400, expectedMonth3?.FutureFunds);
        }

        [Test]
        public void Then_The_CoInvestment_Is_Calculated_Correctly_For_The_First_Month_With_A_Negative_Balance()
        {
            //Arrange
            CreateCommitmentModel(600m, 700m);

            _account = new Account(1, -300, 400, 0, 0);
            Moqer.SetInstance(_account);
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();

            //Act
            accountProjection.BuildPayrollPeriodEndTriggeredProjections(DateTime.Today, 2);

            //Assert
            var expectedMonth1 = accountProjection.Projections.FirstOrDefault();
            Assert.AreEqual(600 * .9, expectedMonth1?.CoInvestmentGovernment);
            Assert.AreEqual(600 * .1, expectedMonth1?.CoInvestmentEmployer);
            Assert.AreEqual(100, expectedMonth1?.FutureFunds);

            var expectedMonth2 = accountProjection.Projections.Skip(1).FirstOrDefault();
            Assert.AreEqual(500 * .9, expectedMonth2?.CoInvestmentGovernment);
            Assert.AreEqual(500 * .1, expectedMonth2?.CoInvestmentEmployer);
            Assert.AreEqual(400, expectedMonth2?.FutureFunds);

            var expectedMonth3 = accountProjection.Projections.Skip(2).FirstOrDefault();
            Assert.AreEqual(200 * .9, expectedMonth3?.CoInvestmentGovernment);
            Assert.AreEqual(200 * .1, expectedMonth3?.CoInvestmentEmployer);
            Assert.AreEqual(400, expectedMonth3?.FutureFunds);
        }

        [Test]
        public void Then_The_CoInvestment_Is_Calculated_Correctly_For_The_First_Month_With_A_Balance_That_Does_Not_Cover_The_Cost_Of_Training()
        {
            //Arrange
            CreateCommitmentModel(600m, 700m);

            _account = new Account(1, 300, 400, 0, 0);
            Moqer.SetInstance(_account);
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();

            //Act
            accountProjection.BuildPayrollPeriodEndTriggeredProjections(DateTime.Today, 2);

            //Assert
            var expectedMonth1 = accountProjection.Projections.FirstOrDefault();
            Assert.AreEqual(300 * .9, expectedMonth1?.CoInvestmentGovernment);
            Assert.AreEqual(300 * .1, expectedMonth1?.CoInvestmentEmployer);
            Assert.AreEqual(400, expectedMonth1?.FutureFunds);

            var expectedMonth2 = accountProjection.Projections.Skip(1).FirstOrDefault();
            Assert.AreEqual(200 * .9, expectedMonth2?.CoInvestmentGovernment);
            Assert.AreEqual(200 * .1, expectedMonth2?.CoInvestmentEmployer);
            Assert.AreEqual(400, expectedMonth2?.FutureFunds);

            var expectedMonth3 = accountProjection.Projections.Skip(2).FirstOrDefault();
            Assert.AreEqual(200 * .9, expectedMonth3?.CoInvestmentGovernment);
            Assert.AreEqual(200 * .1, expectedMonth3?.CoInvestmentEmployer);
            Assert.AreEqual(400, expectedMonth3?.FutureFunds);
        }

        private void CreateCommitmentModel(decimal monthlyInstallment, decimal completionAmount)
        {
            _commitment = new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = DateTime.Today.AddMonths(-1),
                PlannedEndDate = DateTime.Today.AddMonths(25),
                MonthlyInstallment = monthlyInstallment,
                NumberOfInstallments = 4,
                CompletionAmount = completionAmount,
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
        public void Then_When_You_Are_A_Sending_Employer_Your_Balance_First_Month_Does_not_Include_Transfer_Out_Payments_For_First_Month_For_Payment_Run()
        {
            //Arrange
            _account = new Account(1, 2000, 100, 0, 0);
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
                    StartDate = DateTime.Today.AddMonths(-1),
                    PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(5),
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
            Assert.AreEqual(2100m, accountProjection.Projections[0].FutureFunds);
        }


        [Test]
        public void Then_When_You_Are_A_Sending_Employer_Your_Balance_First_Month_Does_not_Include_Transfer_Out_Payments_For_First_Month_For_Levy_Run()
        {
            //Arrange
            _account = new Account(1, 2000, 100, 0, 0);
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
                    StartDate = DateTime.Today.AddMonths(-1),
                    PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(5),
                    MonthlyInstallment = 100,
                    NumberOfInstallments = 6,
                    CompletionAmount = 400,
                    FundingSource = Models.Payments.FundingSource.Transfer
                }
            };

            var employerCommitments = new EmployerCommitments(1, _commitments);
            Moqer.SetInstance(employerCommitments);

            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();

            accountProjection.BuildLevyTriggeredProjections(DateTime.Today, 12);

            //Assert
            Assert.AreEqual(2000m, accountProjection.Projections[0].FutureFunds);
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
            Assert.AreEqual(1000m, accountProjection.Projections[7].FutureFunds);
            Assert.AreEqual(1000m, accountProjection.Projections.Last().FutureFunds);
        }


        [Test]
        public void Then_If_I_Am_A_Receiving_Employer_My_Balance_Is_Equal_To_My_Current_Balance_For_All_Months_For_A_Levy_Run()
        {
            //Arrange
            _account = new Account(1, 2000, 0, 0, 0);
            Moqer.SetInstance(_account);
            _commitments.LevyFundedCommitments = new List<CommitmentModel>();
            _commitments.SendingEmployerTransferCommitments = new List<CommitmentModel>();
            _commitments.ReceivingEmployerTransferCommitments = new List<CommitmentModel>
            {
                new CommitmentModel
                {
                    EmployerAccountId = 1,
                    SendingEmployerAccountId = 999,
                    ApprenticeshipId = 23,
                    LearnerId = 33,
                    StartDate = DateTime.Today.AddMonths(-2),
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

            //Assert
            Assert.IsTrue(accountProjection.Projections.All(c => c.FutureFunds.Equals(2000m)));
        }


        [Test]
        public void Then_If_I_Am_A_Receiving_Employer_My_Balance_Is_Equal_To_My_Current_Balance_For_All_Months_For_A_Payment_Run_And_I_Have_No_Coinvestment()
        {
            //Arrange
            _account = new Account(1, 400, 0, 0, 0);
            Moqer.SetInstance(_account);
            _commitments.LevyFundedCommitments = new List<CommitmentModel>();
            _commitments.SendingEmployerTransferCommitments = new List<CommitmentModel>();
            _commitments.ReceivingEmployerTransferCommitments = new List<CommitmentModel>
            {
                new CommitmentModel
                {
                    EmployerAccountId = 1,
                    SendingEmployerAccountId = 999,
                    ApprenticeshipId = 23,
                    LearnerId = 33,
                    StartDate = DateTime.Today.AddMonths(-2),
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

            accountProjection.BuildPayrollPeriodEndTriggeredProjections(DateTime.Today, 12);

            //Assert
            Assert.IsTrue(accountProjection.Projections.All(c => c.FutureFunds.Equals(400m)));
            Assert.IsTrue(accountProjection.Projections.All(c => c.CoInvestmentEmployer.Equals(0m)));
            Assert.IsTrue(accountProjection.Projections.All(c => c.CoInvestmentGovernment.Equals(0m)));
        }

        [Test]
        public void Then_If_I_Am_Calculating_The_First_Month_After_A_Levy_Run_It_Is_Calculated_Correctly_As_The_Accounts_Balance()
        {
            //Arrange
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();

            //Act
            accountProjection.BuildLevyTriggeredProjections(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 20), 12);

            //Assert
            Assert.AreEqual(12000m, accountProjection.Projections.First().FutureFunds);
        }

        [Test]
        public void Then_If_I_Am_Calculating_The_First_Month_After_A_Payment_Run_It_Is_Calculated_As_The_Account_Balance_Plus_Levy()
        {
            //Arrange
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();

            //Act
            accountProjection.BuildPayrollPeriodEndTriggeredProjections(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 18), 12);

            //Assert
            Assert.AreEqual(12300m, accountProjection.Projections.First().FutureFunds);
        }

        [Test]
        public void Then_If_I_Am_Calculating_The_First_Month_Following_A_Previous_Levy_Run_In_The_Next_Month_It_Is_Calculated_As_Balance_Plus_Levy_Minus_All_Costs()
        {
            //Arrange
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();

            //Act
            accountProjection.BuildLevyTriggeredProjections(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 18), 12);

            //Assert
            Assert.AreEqual(10200m, accountProjection.Projections.First().FutureFunds);
        }


        [TestCase(800, 200, 400, true, 200)]
        [TestCase(400, 200, 400, true, 0)]
        [TestCase(-500, 200, 400, true, -500)]
        [TestCase(800, 200, 400, false, 800)]
        [TestCase(400, 200, 400, false, 400)]
        public void ShouldDetermineBalanceForCoInvestmentAfterTransferCosts(decimal lastBalance, decimal completionPaymentsTransferOut, decimal trainingCostTransferOut, bool isSendingEmployer, decimal expected)
        {
            //Arrange
            if (isSendingEmployer)
            {
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
                        StartDate = DateTime.Today.AddMonths(-2),
                        PlannedEndDate = DateTime.Today.GetStartOfMonth().AddMonths(6),
                        MonthlyInstallment = 2000,
                        NumberOfInstallments = 6,
                        CompletionAmount = 1200,
                        FundingSource = Models.Payments.FundingSource.Transfer
                    }
                };

                var employerCommitments = new EmployerCommitments(1, _commitments);
                Moqer.SetInstance(employerCommitments);
            }

            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();

            //Act
            var balance = accountProjection.GetCurrentBalance(lastBalance, completionPaymentsTransferOut,
                trainingCostTransferOut, false);

            //Assert
            Assert.AreEqual(expected, balance);
        }

        [TestCase(500, 200, 0)]
        [TestCase(100, 200, 100)]
        [TestCase(0, 200, 200)]
        [TestCase(-100, 200, 200)]
        public void ShouldDetermineCoInvestedAmountBasedOnCurrentBalanceAndMoneyOut(decimal currentBalance, decimal moneyOut, decimal expected)
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            var coInvestmentAmount =
                accountProjection.GetCoInvestmentAmountBasedOnCurrentBalanceAndTrainingCosts(currentBalance, moneyOut);

            Assert.AreEqual(expected, coInvestmentAmount);
        }

        [TestCase(500, 200, 400, 700)]
        [TestCase(100, 200, 400, 400)]
        [TestCase(0, 200, 400, 400)]
        [TestCase(-100, 200, 400, 300)]
        public void ShouldDetermineMonthEndBalance(decimal currentBalance, decimal moneyOut, decimal fundsIn, decimal expected)
        {
            var accountProjection = Moqer.Resolve<Projections.AccountProjection>();
            var monthEndBalance = accountProjection.GetMonthEndBalance(currentBalance, moneyOut, fundsIn,
                ProjectionGenerationType.PayrollPeriodEnd, false, 22);

            Assert.AreEqual(expected, monthEndBalance);
        }
    }
}