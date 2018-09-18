Feature: Projections For Employers With Transfer
	As an Employer
	I want my transferred funds in to be considered in my forecast
	So that the balance of my account can be accurately calculated

Background:
	Given I'm a levy paying employer
	And the payroll period is 
	| Payroll Year | Payroll Month |
	| 18-19        | 1             |
	And the following levy declarations have been recorded
	| Scheme   | Amount | Created Date |
	| ABC-1234 | 0      | Today        |
	And the current balance is 500

Scenario: Sending employer with transfer out after levy run
	Given I am a sending employer
	And the following commitments have been recorded
	| Apprentice Name   | Course Name   | Course Level | Provider Name   | Start Date | Installment Amount | Completion Amount | Number Of Installments | FundingSource |
	| Test Apprentice 2 | Test Course 2 | 1            | Test Provider 2 | last month  | 100                | 2000              | 6                      | Transfer      |
	When the account projection is triggered after a levy run
	Then the account projection should be generated
	And should have the following projected values
	| MonthsFromNow | TotalCostOfTraining | TransferOutTotalCostOfTraining | TransferInTotalCostOfTraining | TransferInCompletionPayments | CompletionPayments | TransferOutCompletionPayments | FutureFunds |
	| 0             | 0.00                | 100                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 500         |
	| 1             | 0.00                | 100                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 400         |
	| 2             | 0.00                | 100                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 300         |
	| 3             | 0.00                | 100                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 200         |
	| 4             | 0.00                | 100                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 100           |
	| 5             | 0.00                | 100                            | 0.00                          | 0.00                         | 0.00               | 00.0                          | 0           |
	| 6             | 0.00                | 0.00                           | 0.00                          | 0.00                         | 0.00               | 2000.00                       | 0           |
	| 7             | 0.00                | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 0           |

Scenario: Sending employer with transfer out after payment run
	Given I am a sending employer
	And the following commitments have been recorded
	| Apprentice Name   | Course Name   | Course Level | Provider Name   | Start Date | Installment Amount | Completion Amount | Number Of Installments | FundingSource |
	| Test Apprentice 2 | Test Course 2 | 1            | Test Provider 2 | last month  | 100.00             | 2000              | 6                     | Transfer      |
	When the account projection is triggered after a payment run
	Then the account projection should be generated
	And should have the following projected values
	| MonthsFromNow | TotalCostOfTraining | TransferOutTotalCostOfTraining | TransferInTotalCostOfTraining | TransferInCompletionPayments | CompletionPayments | TransferOutCompletionPayments | FutureFunds |
	| 0             | 0.00                | 100                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 500         |
	| 1             | 0.00                | 100                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 400         |
	| 2             | 0.00                | 100                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 300         |
	| 3             | 0.00                | 100                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 200         |
	| 4             | 0.00                | 100                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 100           |
	| 5             | 0.00                | 100                            | 0.00                          | 0.00                         | 0.00               | 00.0                          | 0           |
	| 6             | 0.00                | 0.00                           | 0.00                          | 0.00                         | 0.00               | 2000.00                       | 0           |
	| 7             | 0.00                | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 0           |

Scenario: Receiving employer account has transfers in after levy run
	Given I am a receiving employer
	And the following commitments have been recorded
	| Apprentice Name   | Course Name   | Course Level | Provider Name   | Start Date | Installment Amount | Completion Amount | Number Of Installments | FundingSource |
	| Test Apprentice 2 | Test Course 2 | 1            | Test Provider 2 | last month  | 100             | 2000              | 6                     | Transfer      |
	When the account projection is triggered after a levy run
	Then the account projection should be generated
	And should have the following projected values
	| MonthsFromNow | TotalCostOfTraining | TransferOutTotalCostOfTraining | TransferInTotalCostOfTraining | TransferInCompletionPayments | CompletionPayments | TransferOutCompletionPayments | FutureFunds |
	| 0             | 0.00                | 100                            | 100                           | 0.00                         | 0.00               | 0.00                          | 500         |
	| 1             | 0.00                | 100                            | 100                           | 0.00                         | 0.00               | 0.00                          | 500         |
	| 2             | 0.00                | 100                            | 100                           | 0.00                         | 0.00               | 0.00                          | 500         |
	| 3             | 0.00                | 100                            | 100                           | 0.00                         | 0.00               | 0.00                          | 500         |
	| 4             | 0.00                | 100                            | 100                           | 0.00                         | 0.00               | 0.00                          | 500         |
	| 5             | 0.00                | 100                            | 100                           | 0.00                         | 0.00               | 0.00                          | 500         |
	| 6             | 0.00                | 0.00                           | 0.00                          | 2000.00                      | 0.00               | 2000.00                       | 500        |
	| 7             | 0.00                | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 500        |

Scenario: Receiving employer account has transfers in after payment run
	Given I am a receiving employer
	And the following commitments have been recorded
	| Apprentice Name   | Course Name   | Course Level | Provider Name   | Start Date | Installment Amount | Completion Amount | Number Of Installments | FundingSource |
	| Test Apprentice 2 | Test Course 2 | 1            | Test Provider 2 | last month  | 100.00             | 2000              | 6                     | Transfer      |
	When the account projection is triggered after a payment run
	Then the account projection should be generated
	And should have the following projected values
	| MonthsFromNow | TotalCostOfTraining | TransferOutTotalCostOfTraining | TransferInTotalCostOfTraining | TransferInCompletionPayments | CompletionPayments | TransferOutCompletionPayments | FutureFunds |
	| 0             | 0.00                | 100                            | 100                           | 0.00                         | 0.00               | 0.00                          | 500         |
	| 1             | 0.00                | 100                            | 100                           | 0.00                         | 0.00               | 0.00                          | 500         |
	| 2             | 0.00                | 100                            | 100                           | 0.00                         | 0.00               | 0.00                          | 500         |
	| 3             | 0.00                | 100                            | 100                           | 0.00                         | 0.00               | 0.00                          | 500         |
	| 4             | 0.00                | 100                            | 100                           | 0.00                         | 0.00               | 0.00                          | 500         |
	| 5             | 0.00                | 100                            | 100                           | 0.00                         | 0.00               | 0.00                          | 500         |
	| 6             | 0.00                | 0.00                           | 0.00                          | 2000.00                      | 0.00               | 2000.00                       | 500        |
	| 7             | 0.00                | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 500        |