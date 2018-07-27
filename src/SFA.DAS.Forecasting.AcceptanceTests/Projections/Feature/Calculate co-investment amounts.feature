Feature: Calculate co-investment amounts [CI-570]
	As an employer with a pay bill over £3 million each year and therefore must now pay the apprenticeship levy
	I want my completion costs to be forecast for the next 4 years
	So that I can effectively forecast my account balance


Scenario: Calculate co-investment after payment run with positive balance
	Given I'm a levy paying employer
	And the payroll period is 
	| Payroll Year | Payroll Month |
	| 18-19        | 1             |
	And the following levy declarations have been recorded
	| Scheme   | Amount | Created Date |
	| ABC-1234 | 400   | Today        |
	And the current balance is 300
	And the following commitments have been recorded
	| Apprentice Name | Course Name | Course Level | Provider Name | Start Date | Installment Amount | Completion Amount | Number Of Installments | FundingSource |
	| Test Apprentice | Test Course | 1            | Test Provider | last month  | 600               | 700              | 6                      | CoInvestedSfa |
	When the account projection is triggered after a payment run
	Then the account projection should be generated
	And the balance should be 400
	And the employer co-investment amount is 10% of the remaining cost of training
	And the government co-investment amount is 90% of the remaining cost of training

Scenario: Calculate co-investment after payment run with positive balance and affordable commitment
	Given I'm a levy paying employer
	And the payroll period is 
	| Payroll Year | Payroll Month |
	| 18-19        | 1             |
	And the following levy declarations have been recorded
	| Scheme   | Amount | Created Date |
	| ABC-1234 | 400   | Today        |
	And the current balance is 300
	And the following commitments have been recorded
	| Apprentice Name | Course Name | Course Level | Provider Name | Start Date | Installment Amount | Completion Amount | Number Of Installments | FundingSource |
	| Test Apprentice | Test Course | 1            | Test Provider | last month  | 200               | 300              | 6                      | CoInvestedSfa |
	When the account projection is triggered after a payment run
	Then the account projection should be generated
	And the balance should be 700
	And the co-investment amount is zero


Scenario: Calculate co-investment after payment run with negative balance
	Given I'm a levy paying employer
	And the payroll period is 
	| Payroll Year | Payroll Month |
	| 18-19        | 1             |
	And the following levy declarations have been recorded
	| Scheme   | Amount | Created Date |
	| ABC-1234 | 400   | Today        |
	And the current balance is -300
	And the following commitments have been recorded
	| Apprentice Name | Course Name | Course Level | Provider Name | Start Date | Installment Amount | Completion Amount | Number Of Installments | FundingSource |
	| Test Apprentice | Test Course | 1            | Test Provider | last month  | 600               | 700              | 6                      | CoInvestedSfa |
	When the account projection is triggered after a payment run
	Then the account projection should be generated
	And the balance should be 100
	And the employer co-investment amount is 10% of the remaining cost of training
	And the government co-investment amount is 90% of the remaining cost of training


Scenario: Calculate co-investment after levy run with positive balance
	Given I'm a levy paying employer
	And the payroll period is 
	| Payroll Year | Payroll Month |
	| 18-19        | 1             |
	And the following levy declarations have been recorded
	| Scheme   | Amount | Created Date |
	| ABC-1234 | 400   | Today        |
	And the current balance is 300
	And the following commitments have been recorded
	| Apprentice Name | Course Name | Course Level | Provider Name | Start Date | Installment Amount | Completion Amount | Number Of Installments | FundingSource |
	| Test Apprentice | Test Course | 1            | Test Provider | last month  | 600               | 700              | 6                      | CoInvestedSfa |
	When the account projection is triggered after levy has been declared
	Then the account projection should be generated
	And the balance should be 300
	And the employer co-investment amount is 10% of the remaining cost of training
	And the government co-investment amount is 90% of the remaining cost of training

Scenario: Calculate co-investment after levy run with negative balance
	Given I'm a levy paying employer
	And the payroll period is 
	| Payroll Year | Payroll Month |
	| 18-19        | 1             |
	And the following levy declarations have been recorded
	| Scheme   | Amount | Created Date |
	| ABC-1234 | 400   | Today        |
	And the current balance is -300
	And the following commitments have been recorded
	| Apprentice Name | Course Name | Course Level | Provider Name | Start Date | Installment Amount | Completion Amount | Number Of Installments | FundingSource |
	| Test Apprentice | Test Course | 1            | Test Provider | last month  | 600               | 700              | 6                      | CoInvestedSfa |
	When the account projection is triggered after levy has been declared
	Then the account projection should be generated
	And the balance should be -300
	And the employer co-investment amount is 10% of the remaining cost of training
	And the government co-investment amount is 90% of the remaining cost of training

Scenario: Calculate co-investment after levy run with positive balance and affordable commitment
	Given I'm a levy paying employer
	And the payroll period is 
	| Payroll Year | Payroll Month |
	| 18-19        | 1             |
	And the following levy declarations have been recorded
	| Scheme   | Amount | Created Date |
	| ABC-1234 | 400   | Today        |
	And the current balance is 300
	And the following commitments have been recorded
	| Apprentice Name | Course Name | Course Level | Provider Name | Start Date | Installment Amount | Completion Amount | Number Of Installments | FundingSource |
	| Test Apprentice | Test Course | 1            | Test Provider | last month  | 200               | 300              | 6                      | CoInvestedSfa |
	When the account projection is triggered after levy has been declared
	Then the account projection should be generated
	And the balance should be 300
	And the co-investment amount is zero