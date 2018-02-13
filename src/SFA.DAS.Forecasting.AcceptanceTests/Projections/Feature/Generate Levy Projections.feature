Feature: Generate Levy Projections [CI-498]
	As an employer with a pay bill over £3 million each year and therefore must now pay the apprenticeship levy
	I want my levy credit to be forecast for the next 4 years
	So that I can effectively forecast my account balance

Background:
	Given I'm a levy paying employer
	And the payroll period is 
	| Payroll Year | Payroll Month |
	| 18-19        | 1             |
	And I have no existing levy declarations for the payroll period

Scenario: AC1: Calculate forecast levy credit value when single linked PAYE scheme
	Given The following levy declaration has been recorded
	| Scheme   | Amount | Created Date |
	| ABC-1234 | 7000   | Today        |
	When the account projection is generated
	Then calculated levy credit value should be the amount declared for the single linked PAYE scheme
	And each future month's forecast levy credit is the same

#Scenario: AC2: Calculate forecast levy credit value when multiple linked PAYE schemes
#Given I'm calculating the levy credit value to use in a forecast
#When I have many levy credits for each employer account
#Then the calculated levy credit value for each account is the sum of the last levy credits
#And each future month's forecast levy credit is the same