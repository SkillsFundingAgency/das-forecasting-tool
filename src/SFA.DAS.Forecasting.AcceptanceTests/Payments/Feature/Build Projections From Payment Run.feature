Feature: Build Projections From Payment Run
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
	
Scenario: Payment run triggers build of projections
	Given the following levy declarations have been recorded
	| Scheme   | Amount   | Created Date |
	| ABC-1234 | 64569.55 | Today        |
	And the current balance is 5000
	And I have made the following payments
	| Payment Amount | Apprentice Name   | Course Name | Course Level | Provider Name | Start Date       | Installment Amount | Completion Amount | Number Of Installments |
	| 133.33         | Test Apprentice   | Test Course | 1            | Test Provider | 16/04/2017 00:00 | 133.33             | 400.00            | 12                     |
	| 133.33         | Test Apprentice 2 | Test Course | 1            | Test Provider | 16/04/2017 00:00 | 133.33             | 400.00            | 12                     |
	| 133.33         | Test Apprentice 3 | Test Course | 1            | Test Provider | 16/04/2017 00:00 | 133.33             | 400.00            | 12                     |
	| 133.33         | Test Apprentice 4 | Test Course | 1            | Test Provider | 16/04/2017 00:00 | 133.33             | 400.00            | 12                     |
	| 133.33         | Test Apprentice 5 | Test Course | 1            | Test Provider | 16/04/2017 00:00 | 133.33             | 400.00            | 12                     |
	| 133.33         | Test Apprentice 6 | Test Course | 1            | Test Provider | 16/04/2017 00:00 | 133.33             | 400.00            | 12                     |
	| 133.33         | Test Apprentice 7 | Test Course | 1            | Test Provider | 29/05/2017 00:00 | 133.33             | 400.00            | 12                     |
	| 133.33         | Test Apprentice 8 | Test Course | 1            | Test Provider | 29/05/2017 00:00 | 133.33             | 400.00            | 12                     |
	| 133.33         | Test Apprentice 9 | Test Course | 1            | Test Provider | 29/05/2017 00:00 | 133.33             | 400.00            | 12                     |
	When the SFA Employer HMRC Payment service notifies the Forecasting service of the payment
	Then the account projection should be generated