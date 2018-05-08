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
	| Date       | TransferOutTotalCostOfTraining |
	| 2018-06-01 | 8000                           |
	| 2018-07-01 | 8000                           |
	| 2018-08-01 | 8000                           |
	| 2018-09-01 | 8000                           |
	| 2018-10-01 | 8000                           |
	| 2018-11-01 | 8000                           |
	| 2018-12-01 | 0                              |
	| 2019-05-01 | 0                              |
	| 2019-06-01 | 2000                           |
	| 2019-07-01 | 2000                           |
	| 2019-08-01 | 2000                           |
	| 2019-09-01 | 2000                           |
	| 2019-10-01 | 2000                           |
	| 2019-11-01 | 2000                           |
	| 2019-12-01 | 0                              |