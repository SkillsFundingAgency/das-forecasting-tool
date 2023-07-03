using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Extensions;
using SFA.DAS.Forecasting.Domain.Projections;
using SFA.DAS.Forecasting.Models.Balance;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Payments;
using SFA.DAS.Forecasting.Models.Projections;
using CalendarPeriod = SFA.DAS.EmployerFinance.Types.Models.CalendarPeriod;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Projection
{
    [TestFixture]
    public class AccountProjectionTests
    {
        private Account _account;
        private CommitmentModel _commitment;
        private EmployerCommitmentsModel _commitments;
        private EmployerCommitments _employerCommitments;
        private AccountProjection _accountProjection;

        private DateTime _projectionStartDate;
        private DateTime _currentDate;

        [SetUp]
        public void SetUp()
        {
            _projectionStartDate = new DateTime(2022, 4, 1);
            _currentDate = _projectionStartDate;

            _account = new Account(1, 12000, 300, 0, 0);
            _commitment = new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = _projectionStartDate,
                PlannedEndDate = _projectionStartDate.AddMonths(24),
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
            _employerCommitments = new EmployerCommitments(1, _commitments);
            _accountProjection = new AccountProjection(_account, _employerCommitments);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(48)]
        public void Starts_From_First_Month(int projectionDurationInMonths)
        {
            _accountProjection.BuildLevyTriggeredProjections(_projectionStartDate, projectionDurationInMonths, _currentDate);
            _accountProjection.Projections.First().Month.Should().Be((short)_projectionStartDate.Month);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(48)]
        public void Projects_Requested_Number_Of_Months_Plus_One(int numberOfMonths)
        {
            _accountProjection.BuildLevyTriggeredProjections(_projectionStartDate, numberOfMonths, _currentDate);
            _accountProjection.Projections.Count.Should().Be(numberOfMonths+1);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(48)]
        public void Projects_Levy_Forward_For_Requested_Number_Of_Months(int numberOfMonths)
        {
            _accountProjection.BuildLevyTriggeredProjections(_projectionStartDate, numberOfMonths, _currentDate);
            _accountProjection.Projections.Count(projection => projection.LevyFundsIn == 300).Should().Be(numberOfMonths+1);
        }

        [Test]
        public void Does_Not_Include_Levy_In_Past_Months()
        {
            _currentDate = _projectionStartDate.AddMonths(3);
            _accountProjection.BuildLevyTriggeredProjections(_projectionStartDate, 12, _currentDate);

            _accountProjection.Projections.Count(projection => projection.LevyFundsIn == 0).Should().Be(3);
            _accountProjection.Projections.Count(projection => projection.LevyFundsIn == 300).Should().Be(10);
        }

        [Test]
        public void Does_Not_Include_Levy_In_First_Month_For_Levy_Triggered_Projection()
        {
            _accountProjection.BuildLevyTriggeredProjections(new DateTime(_projectionStartDate.Year, _projectionStartDate.Month, 20), 2, _currentDate);
            _accountProjection.Projections.First().FutureFunds.Should().Be(_account.Balance);
        }

        [Test]
        public void Includes_Levy_In_Months_After_First_Month_For_Levy_Triggered_Projection()
        {
            
            _accountProjection.BuildLevyTriggeredProjections(_projectionStartDate, 2, _currentDate);

            var expected = _accountProjection.Projections.FirstOrDefault()?.FutureFunds + _account.LevyDeclared -
                           _commitment.MonthlyInstallment;
            var funds2ndMonth = _accountProjection.Projections.Skip(1).FirstOrDefault()?.FutureFunds;
            funds2ndMonth.Should().Be(expected);
        }

        [Test]
        public void Includes_Completion_Payments()
        {
            _commitment.StartDate = _projectionStartDate.AddMonths(-1);
            _commitment.PlannedEndDate = _projectionStartDate.GetStartOfMonth().AddMonths(1);

            _accountProjection = new AccountProjection(_account, _employerCommitments);
            
            _accountProjection.BuildLevyTriggeredProjections(_projectionStartDate, 2, _currentDate);

            _accountProjection.Projections.Skip(2).First().LevyFundedCompletionPayments
                .Should().Be(_commitment.CompletionAmount);
        }

        [Test]
        public void Includes_Installments()
        {
            _account = new Account(1, 12000, 0, 0, 0);
            _accountProjection = new AccountProjection(_account, _employerCommitments);

            _accountProjection.BuildLevyTriggeredProjections(new DateTime(_projectionStartDate.Year, _projectionStartDate.Month, 20), 2, _currentDate);

            _accountProjection.Projections.First().FutureFunds
                .Should().Be(_account.Balance, because: "First month should be the same as current balance");

            _accountProjection.Projections.Skip(1).First().FutureFunds
                .Should().Be(_accountProjection.Projections.FirstOrDefault()?.FutureFunds +
                                      _account.LevyDeclared - _commitment.MonthlyInstallment);
        }

        [Test]
        public void Does_Not_Include_Past_Installments()
        {
            _currentDate = _projectionStartDate.AddMonths(3);
            _accountProjection.BuildLevyTriggeredProjections(_projectionStartDate, 12, _currentDate);

            _accountProjection.Projections.Take(3).All(x => x.FutureFunds == _account.Balance).Should().Be(true);
        }

        [Test]
        public void Includes_Past_Installments_Relating_To_Pledges()
        {
            _currentDate = _projectionStartDate.AddMonths(3);
            _commitment.FundingSource = FundingSource.AcceptedPledgeApplication;
            _commitment.PledgeApplicationId = 123;
            _commitments.LevyFundedCommitments.Clear();
            _commitments.SendingEmployerTransferCommitments.Add(_commitment);

            _account = new Account(1, 12000, 0, 0, 0);
            _accountProjection = new AccountProjection(_account, _employerCommitments);

            _accountProjection.BuildLevyTriggeredProjections(new DateTime(_projectionStartDate.Year, _projectionStartDate.Month, 20), 2, _currentDate);

            _accountProjection.Projections.First().FutureFunds
                .Should().Be(_account.Balance, because: "First month should be the same as current balance");

            _accountProjection.Projections.Skip(1).First().FutureFunds
                .Should().Be(_accountProjection.Projections.FirstOrDefault()?.FutureFunds - _commitment.MonthlyInstallment);
        }

        [Test]
        public void Includes_Co_Investment()
        {
            _accountProjection.BuildLevyTriggeredProjections(new DateTime(_projectionStartDate.Year, _projectionStartDate.Month, 20), 7, _currentDate);

            _accountProjection.Projections[6].CoInvestmentEmployer.Should().Be(0);
            _accountProjection.Projections[6].CoInvestmentGovernment.Should().Be(0);
            _accountProjection.Projections[6].FutureFunds.Should().Be(1200);

            _accountProjection.Projections[7].CoInvestmentEmployer.Should().Be(45);
            _accountProjection.Projections[7].CoInvestmentGovernment.Should().Be(855);
            _accountProjection.Projections[7].FutureFunds.Should().Be(300);
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
                    StartDate = _projectionStartDate,
                    PlannedEndDate = _projectionStartDate.GetStartOfMonth().AddMonths(6),
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
                    StartDate = _projectionStartDate,
                    PlannedEndDate = _projectionStartDate.GetStartOfMonth().AddMonths(6),
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
                    StartDate = _projectionStartDate,
                    PlannedEndDate = _projectionStartDate.GetStartOfMonth().AddMonths(6),
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
                    StartDate = _projectionStartDate,
                    PlannedEndDate = _projectionStartDate.GetStartOfMonth().AddMonths(6),
                    MonthlyInstallment = 2000,
                    NumberOfInstallments = 6,
                    CompletionAmount = 1200,
                    FundingSource = Models.Payments.FundingSource.Transfer
                }
            };


            var employerCommitments = new EmployerCommitments(1, _commitments);
            _accountProjection = new AccountProjection(_account, employerCommitments);

            _accountProjection.BuildLevyTriggeredProjections(_projectionStartDate, 12, _currentDate);

            var projections = _accountProjection.Projections.ToArray();

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
        // Payroll Period End Triggered -
        // ------------------------------

        [Test]
        public void Includes_Levy_In_First_Months_For_Payroll_Period_End_Triggered_Projection_Less_Costs()
        {
            _accountProjection.BuildPayrollPeriodEndTriggeredProjections(_projectionStartDate, 2, _currentDate);

            _accountProjection.Projections.First().FutureFunds
                .Should().Be(_account.Balance + _account.LevyDeclared); // - _commitment.MonthlyInstallment);
        }
        
        [Test]
        public void Includes_Levy_In_First_Months_and_cost_has_been_removed_For_Payroll_Period_End_Triggered_Projection()
        {
            _accountProjection.BuildPayrollPeriodEndTriggeredProjections(_projectionStartDate, 2, _currentDate);

            var expected = _accountProjection.Projections.FirstOrDefault()?.FutureFunds + _account.LevyDeclared -
                           _commitment.MonthlyInstallment;

            _accountProjection.Projections.Skip(1).First().FutureFunds
                .Should().Be(expected);
        }


        [Test]
        public void Then_The_CoInvestment_Is_Calculated_Correctly_For_The_First_Month_With_A_Postive_Balance_And_An_Affordable_Commitment_For_Payment_Run()
        {
            //Arrange
            CreateCommitmentModel(200, 300);

            _account = new Account(1, 300, 400, 0, 0);
            _accountProjection = new AccountProjection(_account, _employerCommitments);
            
            //Act
            _accountProjection.BuildPayrollPeriodEndTriggeredProjections(_projectionStartDate, 2, _currentDate);

            //Assert
            var expectedMonth1 = _accountProjection.Projections.FirstOrDefault();
            Assert.AreEqual(0, expectedMonth1?.CoInvestmentGovernment);
            Assert.AreEqual(0, expectedMonth1?.CoInvestmentEmployer);
            Assert.AreEqual(700, expectedMonth1?.FutureFunds);

            var expectedMonth2 = _accountProjection.Projections.Skip(1).FirstOrDefault();
            Assert.AreEqual(0, expectedMonth2?.CoInvestmentGovernment);
            Assert.AreEqual(0, expectedMonth2?.CoInvestmentEmployer);
            Assert.AreEqual(900, expectedMonth2?.FutureFunds);

            var expectedMonth3 = _accountProjection.Projections.Skip(2).FirstOrDefault();
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
            _accountProjection = new AccountProjection(_account, _employerCommitments);

            //Act
            _accountProjection.BuildLevyTriggeredProjections(new DateTime(_projectionStartDate.Year, _projectionStartDate.Month, 20), 2, _currentDate);

            //Assert
            var expectedMonth1 = _accountProjection.Projections.FirstOrDefault();
            Assert.AreEqual(0, expectedMonth1?.CoInvestmentGovernment);
            Assert.AreEqual(0, expectedMonth1?.CoInvestmentEmployer);
            Assert.AreEqual(300, expectedMonth1?.FutureFunds);

            var expectedMonth2 = _accountProjection.Projections.Skip(1).FirstOrDefault();
            Assert.AreEqual(0, expectedMonth2?.CoInvestmentGovernment);
            Assert.AreEqual(0, expectedMonth2?.CoInvestmentEmployer);
            Assert.AreEqual(500, expectedMonth2?.FutureFunds);

            var expectedMonth3 = _accountProjection.Projections.Skip(2).FirstOrDefault();
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
            _accountProjection = new AccountProjection(_account, _employerCommitments);

            //Act
            _accountProjection.BuildLevyTriggeredProjections(new DateTime(_projectionStartDate.Year, _projectionStartDate.Month, 20), 2, _currentDate);

            //Assert
            var expectedMonth1 = _accountProjection.Projections.FirstOrDefault();
            Assert.AreEqual(0.95 * 300, expectedMonth1?.CoInvestmentGovernment);
            Assert.AreEqual(0.05 * 300, expectedMonth1?.CoInvestmentEmployer);
            Assert.AreEqual(300, expectedMonth1?.FutureFunds);

            var expectedMonth2 = _accountProjection.Projections.Skip(1).FirstOrDefault();
            Assert.AreEqual(0.95 * 300, expectedMonth2?.CoInvestmentGovernment);
            Assert.AreEqual(0.05 * 300, expectedMonth2?.CoInvestmentEmployer);
            Assert.AreEqual(400, expectedMonth2?.FutureFunds);

            var expectedMonth3 = _accountProjection.Projections.Skip(2).FirstOrDefault();
            Assert.AreEqual(0.95 * 200, expectedMonth3?.CoInvestmentGovernment);
            Assert.AreEqual(0.05 * 200, expectedMonth3?.CoInvestmentEmployer);
            Assert.AreEqual(400, expectedMonth3?.FutureFunds);
        }

        [Test]
        public void Then_The_CoInvestment_Is_Calculated_Correctly_For_The_First_Month_With_A_Negative_Balance()
        {
            //Arrange
            CreateCommitmentModel(600m, 700m);

            _account = new Account(1, -300, 400, 0, 0);
            _accountProjection = new AccountProjection(_account, _employerCommitments);

            //Act
            _accountProjection.BuildPayrollPeriodEndTriggeredProjections(_projectionStartDate, 2, _currentDate);

            //Assert
            var expectedMonth1 = _accountProjection.Projections.FirstOrDefault();
            Assert.AreEqual(600 * .95, expectedMonth1?.CoInvestmentGovernment);
            Assert.AreEqual(600 * .05, expectedMonth1?.CoInvestmentEmployer);
            Assert.AreEqual(100, expectedMonth1?.FutureFunds);

            var expectedMonth2 = _accountProjection.Projections.Skip(1).FirstOrDefault();
            Assert.AreEqual(500 * .95, expectedMonth2?.CoInvestmentGovernment);
            Assert.AreEqual(500 * .05, expectedMonth2?.CoInvestmentEmployer);
            Assert.AreEqual(400, expectedMonth2?.FutureFunds);

            var expectedMonth3 = _accountProjection.Projections.Skip(2).FirstOrDefault();
            Assert.AreEqual(200 * .95, expectedMonth3?.CoInvestmentGovernment);
            Assert.AreEqual(200 * .05, expectedMonth3?.CoInvestmentEmployer);
            Assert.AreEqual(400, expectedMonth3?.FutureFunds);
        }

        [Test]
        public void Then_The_CoInvestment_Is_Calculated_Correctly_For_The_First_Month_With_A_Balance_That_Does_Not_Cover_The_Cost_Of_Training()
        {
            //Arrange
            CreateCommitmentModel(600m, 700m);

            _account = new Account(1, 300, 400, 0, 0);
            _accountProjection = new AccountProjection(_account, _employerCommitments);

            //Act
            _accountProjection.BuildPayrollPeriodEndTriggeredProjections(_projectionStartDate, 2, _currentDate);

            //Assert
            var expectedMonth1 = _accountProjection.Projections.FirstOrDefault();
            Assert.AreEqual(300 * .95, expectedMonth1?.CoInvestmentGovernment);
            Assert.AreEqual(300 * .05, expectedMonth1?.CoInvestmentEmployer);
            Assert.AreEqual(400, expectedMonth1?.FutureFunds);

            var expectedMonth2 = _accountProjection.Projections.Skip(1).FirstOrDefault();
            Assert.AreEqual(200 * .95, expectedMonth2?.CoInvestmentGovernment);
            Assert.AreEqual(200 * .05, expectedMonth2?.CoInvestmentEmployer);
            Assert.AreEqual(400, expectedMonth2?.FutureFunds);

            var expectedMonth3 = _accountProjection.Projections.Skip(2).FirstOrDefault();
            Assert.AreEqual(200 * .95, expectedMonth3?.CoInvestmentGovernment);
            Assert.AreEqual(200 * .05, expectedMonth3?.CoInvestmentEmployer);
            Assert.AreEqual(400, expectedMonth3?.FutureFunds);
        }

        [Test]
        public void Then_When_You_Are_A_Sending_Employer_Your_Balance_First_Month_Does_not_Include_Transfer_Out_Payments_For_First_Month_For_Payment_Run()
        {
            //Arrange
            _account = new Account(1, 2000, 100, 0, 0);
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
                    StartDate = _projectionStartDate.AddMonths(-1),
                    PlannedEndDate = _projectionStartDate.GetStartOfMonth().AddMonths(5),
                    MonthlyInstallment = 100,
                    NumberOfInstallments = 6,
                    CompletionAmount = 400,
                    FundingSource = Models.Payments.FundingSource.Transfer
                }
            };

            var employerCommitments = new EmployerCommitments(1, _commitments);
            _accountProjection = new AccountProjection(_account, employerCommitments);

            _accountProjection.BuildPayrollPeriodEndTriggeredProjections(_projectionStartDate, 12, _currentDate);

            //Assert
            Assert.AreEqual(2100m, _accountProjection.Projections[0].FutureFunds);
        }


        [Test]
        public void Then_When_You_Are_A_Sending_Employer_Your_Balance_First_Month_Does_not_Include_Transfer_Out_Payments_For_First_Month_For_Levy_Run()
        {
            //Arrange
            _account = new Account(1, 2000, 100, 0, 0);
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
                    StartDate = _projectionStartDate.AddMonths(-1),
                    PlannedEndDate = _projectionStartDate.GetStartOfMonth().AddMonths(5),
                    MonthlyInstallment = 100,
                    NumberOfInstallments = 6,
                    CompletionAmount = 400,
                    FundingSource = Models.Payments.FundingSource.Transfer
                }
            };

            var employerCommitments = new EmployerCommitments(1, _commitments);
            _accountProjection = new AccountProjection(_account, employerCommitments);

            _accountProjection.BuildLevyTriggeredProjections(_projectionStartDate, 12, _currentDate);

            //Assert
            Assert.AreEqual(2000m, _accountProjection.Projections[0].FutureFunds);
        }

        [Test]
        public void Then_When_You_Are_A_Sending_Employer_Your_FutureFunds_Are_Updated()
        {
            //Arrange
            _account = new Account(1, 2000, 0, 0, 0);
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
                    StartDate = _projectionStartDate,
                    PlannedEndDate = _projectionStartDate.GetStartOfMonth().AddMonths(6),
                    MonthlyInstallment = 100,
                    NumberOfInstallments = 6,
                    CompletionAmount = 400,
                    FundingSource = Models.Payments.FundingSource.Transfer
                }
            };

            var employerCommitments = new EmployerCommitments(1, _commitments);
            
            _accountProjection = new AccountProjection(_account, employerCommitments);
            
            _accountProjection.BuildPayrollPeriodEndTriggeredProjections(_projectionStartDate, 12, _currentDate);

            //Assert
            Assert.AreEqual(1000m, _accountProjection.Projections[7].FutureFunds);
            Assert.AreEqual(1000m, _accountProjection.Projections.Last().FutureFunds);
        }


        [Test]
        public void Then_If_I_Am_A_Receiving_Employer_My_Balance_Is_Equal_To_My_Current_Balance_For_All_Months_For_A_Levy_Run()
        {
            //Arrange
            _account = new Account(1, 2000, 0, 0, 0);
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
                    StartDate = _projectionStartDate.AddMonths(-2),
                    PlannedEndDate = _projectionStartDate.GetStartOfMonth().AddMonths(6),
                    MonthlyInstallment = 2000,
                    NumberOfInstallments = 6,
                    CompletionAmount = 1200,
                    FundingSource = Models.Payments.FundingSource.Transfer
                }
            };

            var employerCommitments = new EmployerCommitments(1, _commitments);
            _accountProjection = new AccountProjection(_account, employerCommitments);
            
            //Act
            _accountProjection.BuildLevyTriggeredProjections(_projectionStartDate, 12, _currentDate);

            //Assert
            Assert.IsTrue(_accountProjection.Projections.All(c => c.FutureFunds.Equals(2000m)));
        }


        [Test]
        public void Then_If_I_Am_A_Receiving_Employer_My_Balance_Is_Equal_To_My_Current_Balance_For_All_Months_For_A_Payment_Run_And_I_Have_No_Coinvestment()
        {
            //Arrange
            _account = new Account(1, 400, 0, 0, 0);
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
                    StartDate = _projectionStartDate.AddMonths(-2),
                    PlannedEndDate = _projectionStartDate.GetStartOfMonth().AddMonths(6),
                    MonthlyInstallment = 2000,
                    NumberOfInstallments = 6,
                    CompletionAmount = 1200,
                    FundingSource = Models.Payments.FundingSource.Transfer
                }
            };

            var employerCommitments = new EmployerCommitments(1, _commitments);
            _accountProjection = new AccountProjection(_account, employerCommitments);

            //Act
            _accountProjection.BuildPayrollPeriodEndTriggeredProjections(_projectionStartDate, 12, _currentDate);

            //Assert
            Assert.IsTrue(_accountProjection.Projections.All(c => c.FutureFunds.Equals(400m)));
            Assert.IsTrue(_accountProjection.Projections.All(c => c.CoInvestmentEmployer.Equals(0m)));
            Assert.IsTrue(_accountProjection.Projections.All(c => c.CoInvestmentGovernment.Equals(0m)));
        }

        [Test]
        public void Then_If_I_Am_Calculating_The_First_Month_After_A_Levy_Run_It_Is_Calculated_Correctly_As_The_Accounts_Balance()
        {
            //Act
            _accountProjection.BuildLevyTriggeredProjections(new DateTime(_projectionStartDate.Year, _projectionStartDate.Month, 20), 12, _currentDate);

            //Assert
            Assert.AreEqual(12000m, _accountProjection.Projections.First().FutureFunds);
        }

        [Test]
        public void Then_If_I_Am_Calculating_The_First_Month_After_A_Payment_Run_It_Is_Calculated_As_The_Account_Balance_Plus_Levy()
        {
           //Act
            _accountProjection.BuildPayrollPeriodEndTriggeredProjections(new DateTime(_projectionStartDate.Year, _projectionStartDate.Month, 18), 12, _currentDate);

            //Assert
            Assert.AreEqual(12300m, _accountProjection.Projections.First().FutureFunds);
        }

        [Test]
        public void Then_If_I_Am_Calculating_The_First_Month_Following_A_Previous_Levy_Run_In_The_Next_Month_It_Is_Calculated_As_Balance_Plus_Levy_Minus_All_Costs()
        {
            //Act
            _accountProjection.BuildLevyTriggeredProjections(new DateTime(_projectionStartDate.AddMonths(1).Year, _projectionStartDate.AddMonths(1).Month, 18), 12, _currentDate);

            //Assert
            Assert.AreEqual(10200m, _accountProjection.Projections.First().FutureFunds);
        }


        [TestCase(800, 200, 400, true, 200)]
        [TestCase(400, 200, 400, true, 0)]
        [TestCase(-500, 200, 400, true, -500)]
        [TestCase(800, 200, 400, false, 800)]
        [TestCase(400, 200, 400, false, 400)]
        public void ShouldDetermineBalanceForCoInvestmentAfterTransferCosts(decimal lastBalance, decimal completionPaymentsTransferOut, decimal trainingCostTransferOut, bool isSendingEmployer, decimal expected)
        {
            //Arrange
            var employerCommitments = _employerCommitments;
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
                        StartDate = _projectionStartDate.AddMonths(-2),
                        PlannedEndDate = _projectionStartDate.GetStartOfMonth().AddMonths(6),
                        MonthlyInstallment = 2000,
                        NumberOfInstallments = 6,
                        CompletionAmount = 1200,
                        FundingSource = Models.Payments.FundingSource.Transfer
                    }
                };

                employerCommitments = new EmployerCommitments(1, _commitments);
            }

            _accountProjection = new AccountProjection(_account, employerCommitments);

            //Act
            var balance = _accountProjection.GetCurrentBalance(lastBalance, completionPaymentsTransferOut + trainingCostTransferOut, false);

            //Assert
            Assert.AreEqual(expected, balance);
        }

        [TestCase(500, 200, 0)]
        [TestCase(100, 200, 100)]
        [TestCase(0, 200, 200)]
        [TestCase(-100, 200, 200)]
        public void ShouldDetermineCoInvestedAmountBasedOnCurrentBalanceAndMoneyOut(decimal currentBalance, decimal moneyOut, decimal expected)
        {
            var coInvestmentAmount =
                _accountProjection.GetCoInvestmentAmountBasedOnCurrentBalanceAndTrainingCosts(currentBalance, moneyOut);

            Assert.AreEqual(expected, coInvestmentAmount);
        }

        [TestCase(500, 200, 400, 700)]
        [TestCase(100, 200, 400, 400)]
        [TestCase(0, 200, 400, 400)]
        [TestCase(-100, 200, 400, 300)]
        public void ShouldDetermineMonthEndBalance(decimal currentBalance, decimal moneyOut, decimal fundsIn, decimal expected)
        {
            var monthEndBalance = _accountProjection.GetMonthEndBalance(currentBalance, moneyOut, fundsIn,
                ProjectionGenerationType.PayrollPeriodEnd, false, 22);

            Assert.AreEqual(expected, monthEndBalance);
        }

        [Test]
        public void ShouldCalculateExpiredFunds_And_The_Original_Projection_Value_Is_Stored()
        {
            var expiredFunds = new Dictionary<CalendarPeriod, decimal>()
            {
                { new CalendarPeriod(DateTime.UtcNow.AddMonths(1).Year ,DateTime.UtcNow.AddMonths(1).Month), 15000  }
            };
            _accountProjection.BuildLevyTriggeredProjections(DateTime.UtcNow, 24, _currentDate);

            _accountProjection.UpdateProjectionsWithExpiredFunds(expiredFunds);

            Assert.IsTrue(_accountProjection.Projections.Sum(s => s.FutureFundsNoExpiry) != 0);
            Assert.IsTrue(_accountProjection.Projections.Sum(s => s.FutureFundsNoExpiry) != _accountProjection.Projections.Sum(s => s.FutureFunds));
            Assert.IsTrue(_accountProjection.Projections.Sum(s => s.ExpiredFunds) > 0);
        }

        private void CreateCommitmentModel(decimal monthlyInstallment, decimal completionAmount)
        {
            _commitment = new CommitmentModel
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = _projectionStartDate.AddMonths(-1),
                PlannedEndDate = _projectionStartDate.AddMonths(25),
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
            _employerCommitments = new EmployerCommitments(1, _commitments);
        }
    }
}