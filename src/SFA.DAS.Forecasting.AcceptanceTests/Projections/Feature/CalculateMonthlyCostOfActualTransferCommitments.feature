Feature: Calculate monthly cost of actual transfer commitments (Sending Employer - CI-619)
	As an employer
	I want to see my actual training costs for transfers that I'm funding
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


Scenario: AC1 Transfer training cost when some commitments duration exceeds forecast period
	Given the following commitments have been recorded
	| Apprentice Name   | Course Name   | Course Level | Provider Name   | Start Date | Installment Amount | Completion Amount | Number Of Installments | EmployerAccountId | SendingEmployerAccountId | FundingSource |
	| Test Apprentice   | Test Course   | 1            | Test Provider 1 | Yesterday  | 2000               | 1200              | 6                      | 999               | 12345                    | 2             |
	| Test Apprentice 1 | Test Course   | 1            | Test Provider 2 | Yesterday  | 2000               | 1200              | 6                      | 999               | 12345                    | 2             |
	| Test Apprentice 2 | Test Course 2 | 1            | Test Provider 3 | Yesterday  | 2000               | 1200              | 6                      | 999               | 12345                    | 2             |
	| Test Apprentice 3 | Test Course   | 1            | Test Provider 4 | Yesterday  | 2000               | 1200              | 6                      | 999               | 12345                    | 2             |
	| Test Apprentice 4 | Test Course 2 | 1            | Test Provider 5 | Next Year  | 2000               | 1200              | 6                      | 999               | 12345                    | 2             |
	When the account projection is triggered after a payment run
	Then the account projection should be generated
	And should have following projections
	| MonthsFromNow | TransferOutTotalCostOfTraining |
	| 1             | 8000                           |
	| 2             | 8000                           |
	| 3             | 8000                           |
	| 4             | 8000                           |
	| 5             | 8000                           |
	| 6             | 8000                           |
	| 7             | 0                              |
	| 12            | 0                              |
	| 13            | 2000                           |
	| 14            | 2000                           |
	| 15            | 2000                           |
	| 16            | 2000                           |
	| 17            | 2000                           |
	| 18            | 2000                           |
	| 19            | 0                              |