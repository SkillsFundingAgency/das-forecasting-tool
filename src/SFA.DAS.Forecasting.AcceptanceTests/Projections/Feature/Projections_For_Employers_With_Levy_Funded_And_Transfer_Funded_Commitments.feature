Feature: Projections For Employers With Levy Funded And Transfer Funded Commitments
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
	| ABC-1234 | 3000   | Today        |
	And the current balance is 5000

Scenario: Sending employer account has transfers in payment run
	Given I am a sending employer
	And the following commitments have been recorded
	| Apprentice Name   | Course Name   | Course Level | Provider Name   | Start Date | Installment Amount | Completion Amount | Number Of Installments | FundingSource |
	| Test Apprentice 1 | Test Course 1 | 1            | Test Provider 1 | Last Month  | 666.66             | 2000              | 12                     | Levy          |
	| Test Apprentice 2 | Test Course 2 | 1            | Test Provider 2 | Last Month  | 444.44             | 1000              | 18                     | Transfer      |
	When the account projection is triggered after a payment run
	Then the account projection should be generated
	And should have the following projected values
	| MonthsFromNow | TotalCostOfTraining | TransferOutTotalCostOfTraining | TransferInTotalCostOfTraining | TransferInCompletionPayments | CompletionPayments | TransferOutCompletionPayments | FutureFunds |
	| 0             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 8000.00     |
	| 1             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 9888.90     |
	| 2             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 11777.80    |
	| 3             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 13666.70    |
	| 4             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 15555.60    |
	| 5             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 17444.50    |
	| 6             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 19333.40    |
	| 7             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 21222.30    |
	| 8             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 23111.20    |
	| 9             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 25000.1     |
	| 10            | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 26889.00    |
	| 11            | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 28777.90    |
	| 12            | 0.00                | 444.44                         | 0.00                          | 0.00                         | 2000.00            | 0.00                          | 29333.46    |
	| 13            | 0.00                | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 31889.02    |
	| 14            | 0.00                | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 34444.58    |
	| 15            | 0.00                | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 37000.14    |
	| 16            | 0.00                | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 39555.70    |
	| 17            | 0.00                | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 42111.26    |
	| 18            | 0.00                | 0.00                           | 0.00                          | 0.00                         | 0.00               | 1000.00                       | 44111.26    |
	| 19            | 0.00                | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 47111.26    |
	| 20            | 0.00                | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 50111.26    |

	Scenario: Sending employer account has transfers in levy run
	Given I am a sending employer
	And the following commitments have been recorded
	| Apprentice Name   | Course Name   | Course Level | Provider Name   | Start Date | Installment Amount | Completion Amount | Number Of Installments | FundingSource |
	| Test Apprentice 1 | Test Course 1 | 1            | Test Provider 1 | Last Month  | 666.66             | 2000              | 12                     | Levy          |
	| Test Apprentice 2 | Test Course 2 | 1            | Test Provider 2 | Last Month  | 444.44             | 1000              | 18                     | Transfer      |
	When the account projection is triggered after a levy run
	Then the account projection should be generated
	And should have the following projected values
	| MonthsFromNow | TotalCostOfTraining | TransferOutTotalCostOfTraining | TransferInTotalCostOfTraining | TransferInCompletionPayments | CompletionPayments | TransferOutCompletionPayments | FutureFunds |
	| 0             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 5000.00     |
	| 1             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 6888.90     |
	| 2             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 8777.80    |
	| 3             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 10666.70    |
	| 4             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 12555.60    |
	| 5             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 14444.50    |
	| 6             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 16333.40    |
	| 7             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 18222.30    |
	| 8             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 20111.20    |
	| 9             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 22000.1     |
	| 10            | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 23889.00    |
	| 11            | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 25777.90    |
	| 12            | 0.00                | 444.44                         | 0.00                          | 0.00                         | 2000.00            | 0.00                          | 26333.46    |
	| 13            | 0.00                | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 28889.02    |
	| 14            | 0.00                | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 31444.58    |
	| 15            | 0.00                | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 34000.14    |
	| 16            | 0.00                | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 36555.70    |
	| 17            | 0.00                | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 39111.26    |
	| 18            | 0.00                | 0.00                           | 0.00                          | 0.00                         | 0.00               | 1000.00                       | 41111.26    |
	| 19            | 0.00                | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 44111.26    |
	| 20            | 0.00                | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 47111.26    |


	Scenario: Receiving employer account has transfers in
	Given I am a receiving employer
	And the following commitments have been recorded
	| Apprentice Name   | Course Name   | Course Level | Provider Name   | Start Date | Installment Amount | Completion Amount | Number Of Installments | FundingSource |
	| Test Apprentice 1 | Test Course 1 | 1            | Test Provider 1 | Yesterday  | 666.66             | 2000              | 12                     | Levy          |
	| Test Apprentice 2 | Test Course 2 | 1            | Test Provider 2 | Last Month  | 444.44             | 2000              | 18                     | Transfer      |
	When the account projection is triggered after a payment run
	Then the account projection should be generated
	And should have the following projected values
	| MonthsFromNow | TotalCostOfTraining | TransferOutTotalCostOfTraining | TransferInTotalCostOfTraining | TransferInCompletionPayments | CompletionPayments | TransferOutCompletionPayments | FutureFunds |
	| 0             | 0.00                | 444.44                         | 444.44                        | 0.00                         | 0.00               | 0.00                          | 8000.00     |
	| 1             | 666.66              | 444.44                         | 444.44                        | 0.00                         | 0.00               | 0.00                          | 10333.34    |
	| 2             | 666.66              | 444.44                         | 444.44                        | 0.00                         | 0.00               | 0.00                          | 12666.68    |
	| 3             | 666.66              | 444.44                         | 444.44                        | 0.00                         | 0.00               | 0.00                          | 15000.02    |
	| 4             | 666.66              | 444.44                         | 444.44                        | 0.00                         | 0.00               | 0.00                          | 17333.36    |
	| 5             | 666.66              | 444.44                         | 444.44                        | 0.00                         | 0.00               | 0.00                          | 19666.70    |
	| 6             | 666.66              | 444.44                         | 444.44                        | 0.00                         | 0.00               | 0.00                          | 22000.04    |
	| 7             | 666.66              | 444.44                         | 444.44                        | 0.00                         | 0.00               | 0.00                          | 24333.38    |
	| 8             | 666.66              | 444.44                         | 444.44                        | 0.00                         | 0.00               | 0.00                          | 26666.72    |
	| 9             | 666.66              | 444.44                         | 444.44                        | 0.00                         | 0.00               | 0.00                          | 29000.06    |
	| 10            | 666.66              | 444.44                         | 444.44                        | 0.00                         | 0.00               | 0.00                          | 31333.40    |
	| 11            | 666.66              | 444.44                         | 444.44                        | 0.00                         | 0.00               | 0.00                          | 33666.74    |
	| 12            | 666.66              | 444.44                         | 444.44                        | 0.00                         | 0.00               | 0.00                          | 36000.08    |
	| 13            | 0.00                | 444.44                         | 444.44                        | 0.00                         | 2000.00            | 0.00                          | 37000.08    |
	| 14            | 0.00                | 444.44                         | 444.44                        | 0.00                         | 0.00               | 0.00                          | 40000.08    |
	| 15            | 0.00                | 444.44                         | 444.44                        | 0.00                         | 0.00               | 0.00                          | 43000.08    |
	| 16            | 0.00                | 444.44                         | 444.44                        | 0.00                         | 0.00               | 0.00                          | 46000.08    |
	| 17            | 0.00                | 444.44                         | 444.44                        | 0.00                         | 0.00               | 0.00                          | 49000.08    |
	| 18            | 0.00                | 0.00                           | 0.00                          | 2000.00                      | 0.00               | 2000.00                       | 52000.08    |
	| 19            | 0.00                | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 55000.08    |
	| 20            | 0.00                | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 58000.08    |