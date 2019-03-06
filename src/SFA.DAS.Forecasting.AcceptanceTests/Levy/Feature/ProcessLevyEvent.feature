Feature: Process Levy Event [CI-528]
	As an employer with a pay bill over £3 million each year and therefore must now pay the apprenticeship levy
	I want my levy credit to be forecast for the next 4 years
	So that I can effectively forecast my account balance

Background:
	Given I'm a levy paying employer
	And the payroll period is 
	| Payroll Year | Payroll Month |
	| 18-19        | 1             |
	And I have no existing levy declarations for the payroll period
	
Scenario: AC1 - Store valid levy declarations
	Given I have made the following levy declarations
	| Scheme   | Amount | Created Date |
	| ABC-1234 | 7000   | Today        |
	| DEF-5678 | 3000   | Today        |
	| REF-5678 | -3000  | Today        |
	When the SFA Employer HMRC Levy service notifies the Forecasting service of the levy declarations
	Then the Forecasting Levy service should store the levy declarations	

Scenario: AC2 - Do not store invalid levy credit event data
	Given I made some invalid levy declarations
	When the SFA Employer HMRC Levy service notifies the Forecasting service of the levy declarations
	Then the Forecasting Levy service should not store the levy declarations

Scenario: AC3 - Store levy declarations with 0 amount
	Given I have made the following levy declarations
	| Scheme   | Amount | Created Date |
	| ABC-1234 | 0      | Today        |
	When the SFA Employer HMRC Levy service notifies the Forecasting service of the levy declarations
	Then the Forecasting Levy service should store the levy declarations	
