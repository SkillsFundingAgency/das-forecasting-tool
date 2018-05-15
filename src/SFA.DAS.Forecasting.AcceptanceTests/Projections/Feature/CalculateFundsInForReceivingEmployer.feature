Feature: Calculate funds in for receiving employer (CI-644)
	As a receiving employer
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

Scenario: AC1: receiving employer account has transfers in
	Given the following commitments have been recorded
	| Apprentice Name   | Course Name   | Course Level | Provider Name   | Start Date | Installment Amount | Completion Amount | Number Of Installments | EmployerAccountId | SendingEmployerAccountId | FundingSource |
	| Test Apprentice   | Test Course   | 1            | Test Provider 1 | Yesterday  | 2000               | 1200              | 6                      | 12345             | 999                      | 1             |
	| Test Apprentice 1 | Test Course   | 1            | Test Provider 2 | Yesterday  | 2000               | 1200              | 6                      | 12345             | 999                      | 1             |
	| Test Apprentice 2 | Test Course 2 | 1            | Test Provider 3 | Yesterday  | 2000               | 1200              | 6                      | 12345             | 999                      | 2             |
	| Test Apprentice 3 | Test Course   | 1            | Test Provider 4 | Yesterday  | 2000               | 1200              | 6                      | 999               | 12345                    | 2             |
	| Test Apprentice 4 | Test Course 2 | 1            | Test Provider 5 | Next Year  | 2000               | 1200              | 6                      | 12345             | 999                      | 2             |
	When the account projection is triggered after a payment run
	Then the account projection should be generated
	And should have following projections from completion
	| MonthsFromNow | TotalCostOfTraining | TransferInTotalCostOfTraining | TransferOutTotalCostOfTraining | TransferOutCompletionPayments | TransferInCompletionPayments | CompletionPayments |
	| 5             | 8000                | 2000                          | 4000                           | 0                             | 0                            | 0                  |
	| 6             | 8000                | 2000                          | 4000                           | 0                             | 0                            | 0                  |
	| 7             | 0                   | 0                             | 0                              | 2400                          | 1200                         | 4800               |
	| 8             | 0                   | 0                             | 0                              | 0                             | 0                            | 0                  |