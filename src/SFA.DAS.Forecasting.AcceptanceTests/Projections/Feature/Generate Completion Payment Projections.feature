Feature: Generate Completion Payment Projections [CI-506] 
	As an employer with a pay bill over £3 million each year and therefore must now pay the apprenticeship levy
	I want my levy credit to be forecast for the next 4 years
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

Scenario: AC1: Commitments with end dates in forecast period
	Given the following commitments have been recorded
	| Apprentice Name   | Course Name | Course Level | Provider Name | Start Date | Installment Amount | Completion Amount | Number Of Installments |
	| Test Apprentice   | Test Course | 1            | Test Provider | Yesterday  | 500                | 3000              | 24                     |
	When the account projection is triggered after a payment run
	Then the account projection should be generated
	Then the completion payments should be included in the correct month

Scenario: AC2: Multiple commitments with end dates in forecast period
	Given the following commitments have been recorded
	| Apprentice Name   | Course Name   | Course Level | Provider Name | Start Date | Installment Amount | Completion Amount | Number Of Installments |
	| Test Apprentice 1 | Test Course   | 1            | Test Provider | Yesterday  | 500                | 3000              | 24                     |
	| Test Apprentice 2 | Test Course 2 | 1            | Test Provider | Yesterday  | 500                | 3000              | 24                     |
	| Test Apprentice 3 | Test Course 3 | 1            | Test Provider | Last year  | 250                | 2000              | 24                     |
	| Test Apprentice 4 | Test Course 3 | 1            | Test Provider | Last year  | 250                | 2000              | 24                     |
	| Test Apprentice 5 | Test Course 4 | 1            | Test Provider | Next year  | 100                | 2000              | 24                     |
	When the account projection is triggered after a payment run
	Then the account projection should be generated
	Then the completion payments should be included in the correct month	

Scenario: AC3: Multiple commitments and some with end dates after end of forecast period
	Given the following commitments have been recorded
	| Apprentice Name   | Course Name | Course Level | Provider Name | Start Date | Installment Amount | Completion Amount | Number Of Installments |
	| Test Apprentice 1 | Test Course | 1            | Test Provider | Next Year  | 1000               | 6000              | 48                     |
	When the account projection is triggered after a payment run
	Then the account projection should be generated
	Then the completion payments should not be included in the projection