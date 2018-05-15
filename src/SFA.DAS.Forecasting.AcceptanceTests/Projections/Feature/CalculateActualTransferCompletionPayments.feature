Feature: Calculate actual transfer completion payments (CI-620)
	As an employer
	I want to see my actual training completion costs for transfers that I'm funding
	So that they can be taken into account while forecasting against my transfer allowance

Background:
	Given I'm a levy paying employer
	And the payroll period is 
	| Payroll Year | Payroll Month |
	| 18-19        | 1             |
	And the following levy declarations have been recorded
	| Scheme   | Amount | Created Date |
	| ABC-1234 | 3000   | Today        |
	And the current balance is 5000

Scenario: AC1: Multiple transfer commitments
	Given the following commitments have been recorded
	| Apprentice Name   | Course Name   | Course Level | Provider Name   | Start Date | Installment Amount | Completion Amount | Number Of Installments | EmployerAccountId | SendingEmployerAccountId | FundingSource |
	| Test Apprentice   | Test Course   | 1            | Test Provider 1 | Yesterday  | 2000               | 1200              | 6                      | 999               | 12345                    | 2             |
	| Test Apprentice 1 | Test Course   | 1            | Test Provider 2 | Yesterday  | 2000               | 1200              | 6                      | 999               | 12345                    | 2             |
	| Test Apprentice 2 | Test Course 2 | 1            | Test Provider 3 | Yesterday  | 2000               | 1200              | 6                      | 999               | 12345                    | 2             |
	| Test Apprentice 3 | Test Course   | 1            | Test Provider 4 | Yesterday  | 2000               | 1200              | 6                      | 999               | 12345                    | 2             |
	| Test Apprentice 4 | Test Course 2 | 1            | Test Provider 5 | Next Year  | 2000               | 1200              | 6                      | 999               | 12345                    | 2             |

	When the account projection is triggered after a payment run
	Then the account projection should be generated
	And should have following projections from completion
	| MonthsFromNow | TotalCostOfTraining | TransferInTotalCostOfTraining | TransferOutTotalCostOfTraining | TransferOutCompletionPayments | TransferInCompletionPayments | CompletionPayments |
	| 6             | 8000                | 0                             | 8000                           | 0                             | 0                            | 0                  |
	| 7             | 0                   | 0                             | 0                              | 4800                          | 0                            | 4800               |
	| 8             | 0                   | 0                             | 0                              | 0                             | 0                            | 0                  |
	| 18            | 2000                | 0                             | 2000                           | 0                             | 0                            | 0                  |
	| 19            | 0                   | 0                             | 0                              | 1200                          | 0                            | 1200               |
	| 20            | 0                   | 0                             | 0                              | 0                             | 0                            | 0                  |
	

Scenario: AC3: Multiple transfer commitments and some with end dates after end of forecast period
	Given the following commitments have been recorded
	| Apprentice Name   | Course Name   | Course Level | Provider Name   | Start Date | Installment Amount | Completion Amount | Number Of Installments | EmployerAccountId | SendingEmployerAccountId | FundingSource |
	| Test Apprentice   | Test Course   | 1            | Test Provider 1 | Today      | 2000               | 1200              | 48                     | 999               | 12345                    | 2             |
	| Test Apprentice 4 | Test Course 2 | 1            | Test Provider 5 | Today      | 2000               | 1200              | 48                     | 999               | 12345                    | 2             |

	When the account projection is triggered after a payment run
	Then the account projection should be generated
	And should have no payments with TransferOutCompletionPayments