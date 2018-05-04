Feature: Calculate co-investment amounts [CI-570]
	As an employer with a pay bill over £3 million each year and therefore must now pay the apprenticeship levy
	I want my completion costs to be forecast for the next 4 years
	So that I can effectively forecast my account balance

Background:
	Given I'm a levy paying employer
	And the payroll period is 
	| Payroll Year | Payroll Month |
	| 18-19        | 1             |
	And the following levy declarations have been recorded
	| Scheme   | Amount | Created Date |
	| ABC-1234 | 3000   | Today        |
	And the current balance is 5000

Scenario: Calculate Co-investment 
	Given the following commitments have been recorded
	| Apprentice Name   | Course Name   | Course Level | Provider Name | Start Date | Installment Amount | Completion Amount | Number Of Installments |
	| Test Apprentice   | Test Course   | 1            | Test Provider | Yesterday  | 2000               | 1200              | 6                      |
	| Test Apprentice 1 | Test Course   | 1            | Test Provider | Yesterday  | 2000               | 1200              | 6                      |
	| Test Apprentice 2 | Test Course 2 | 1            | Test Provider | Yesterday  | 2000               | 1200              | 6                      |
	| Test Apprentice 3 | Test Course   | 1            | Test Provider | Yesterday  | 2000               | 1200              | 6                      |
	| Test Apprentice 4 | Test Course 2 | 1            | Test Provider | Yesterday  | 2000               | 1200              | 6                      |
	When the account projection is triggered after a payment run
	Then the account projection should be generated
	And the balance should be 0
	And the employer co-investment amount is 10% of the negative balance
	And the government co-investment amount is 90% of the negative value


# Move to separate file

Scenario: AC1 Transfer training cost when some commitments duration exceeds forecast period
	Given the following commitments have been recorded
	| Apprentice Name   | Course Name   | Course Level | Provider Name   | Start Date | Installment Amount | Completion Amount | Number Of Installments | EmployerAccountId | SendingEmployerAccountId | FundingSource |
	| Test Apprentice   | Test Course   | 1            | Test Provider 1 | Yesterday  | 2000               | 1200              | 6                      | 999                      | 12345                    | 2             |
	| Test Apprentice 1 | Test Course   | 1            | Test Provider 2 | Yesterday  | 2000               | 1200              | 6                      | 999                      | 12345                    | 2             |
	| Test Apprentice 2 | Test Course 2 | 1            | Test Provider 3 | Yesterday  | 2000               | 1200              | 6                      | 999                      | 12345                    | 2             |
	| Test Apprentice 3 | Test Course   | 1            | Test Provider 4 | Yesterday  | 2000               | 1200              | 6                      | 999                      | 12345                    | 2             |
	| Test Apprentice 4 | Test Course 2 | 1            | Test Provider 5 | Next Year  | 2000               | 1200              | 6                      | 999                      | 12345                    | 2             |
	When the account projection is triggered after a payment run
	Then the account projection should be generated
	And transfer out should have 8000 month 1 to 6
	And transfer out should have 2000 month 13 to 19