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

Scenario: Co inverstment 
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