Feature: Build Projections From Levy Declarations
	As an employer with a pay bill over £3 million each year and therefore must now pay the apprenticeship levy
	I want my levy credit to be forecast for the next 4 years
	So that I can effectively forecast my account balance

Background:
	Given I'm a levy paying employer
	And the payroll period is 
	| Payroll Year | Payroll Month |
	| 18-19        | 1             |
	And I have no existing levy declarations for the payroll period
	And no account projections have been generated
	
Scenario: AC3 - Levy Declaration triggers build of projections
	Given the following commitments have been recorded
	| Apprentice Name   | Course Name | Course Level | Provider Name | Start Date | Installment Amount | Completion Amount | Number Of Installments |
	| Test Apprentice 1 | Test Course | 1            | Test Provider | Yesterday  | 500                | 3000              | 24                     |
	| Test Apprentice 2 | Test Course | 1            | Test Provider | Last year  | 250                | 1500              | 48                     |
	And the current balance is 5000
	And I have made the following levy declarations
	| Scheme   | Amount | Created Date |
	| ABC-1234 | 7000   | Today        |
	| DEF-5678 | 3000   | Today        |
	When the SFA Employer HMRC Levy service notifies the Forecasting service of the levy declarations
	Then the account projection should be generated