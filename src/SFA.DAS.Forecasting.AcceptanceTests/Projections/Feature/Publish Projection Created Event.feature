Feature: Publish Projection Created Event - CI-963
	In order to allow external system to update
	as an external system
	i need to subscribe to projection creted events

Background:
	Given I'm a levy paying employer
	And the payroll period is 
	| Payroll Year | Payroll Month |
	| 18-19        | 1             |
	And the following commitments have been recorded
	| Apprentice Name   | Course Name | Course Level | Provider Name | Start Date | Installment Amount | Completion Amount | Number Of Installments |
	| Test Apprentice 1 | Test Course | 1            | Test Provider | Yesterday  | 500                | 3000              | 24                     |
	| Test Apprentice 2 | Test Course | 1            | Test Provider | Last year  | 250                | 1500              | 48                     |
	And the current balance is 5000
	And  I have no existing levy declarations for the payroll period

	Scenario: levy
	Given the following levy declarations have been recorded
	| Scheme   | Amount | Created Date |
	| ABC-1234 | 3000   | Today        |
	| ABC-5678 | 3500   | Today        |
	| ABC-9012 | 8500   | Today        |
	When the account projection is triggered after levy has been declared
	Then the account projection should be generated
	And the Account Projection Event is published
