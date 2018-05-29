Feature: Generate Training Cost Projections [CI-499]
	As an employer with a pay bill over £3 million each year and therefore must now pay the apprenticeship levy
	I want my trainging costs to be forecast for the next 4 years
	So that I can effectively forecast my account balance

Background:
	Given I'm a levy paying employer
	And the payroll period is 
	| Payroll Year | Payroll Month |
	| 18-19        | 1             |
	And the following levy declarations have been recorded
	| Scheme   | Amount   | Created Date |
	| ABC-1234 | 64569.55 | Today        |
	And the current balance is 623104.60

Scenario: AC1: Training cost for commitments included in the projection
	Given the following commitments have been recorded
	| Apprentice Name    | Course Name | Course Level | Provider Name | Start Date       | Installment Amount | Completion Amount | Number Of Installments |
	| Test Apprentice    | Test Course | 1            | Test Provider | 16/04/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice 2  | Test Course | 1            | Test Provider | 16/04/2017 00:00 | 133.33             | 400.00            | 12                     |
	When the account projection is triggered after a payment run
	Then the account projection should be generated
	And the training costs should be included in the correct months

Scenario: Sending employer transfer training costs
	Given I am a sending employer
	And the following commitments have been recorded
	| Apprentice Name   | Course Name | Course Level | Provider Name   | Start Date | Installment Amount | Completion Amount | Number Of Installments | FundingSource |
	| Test Apprentice 1 | Test Course | 1            | Test Provider 1 | Last Year  | 333.33             | 2000              | 24                     | Transfer      |
	| Test Apprentice 2 | Test Course | 1            | Test Provider 2 | Yesterday  | 444.44             | 2000              | 18                     | Transfer      |
	| Test Apprentice 3 | Test Course | 1            | Test Provider 3 | Next Year  | 666.66             | 2000              | 12                     | Transfer      |
	When the account projection is triggered after a payment run
	Then the account projection should be generated
	And should have following projections
	| Months From Now | Transfer Out Total Cost Of Training |
	| 0               | 333.33                              |
	| 1               | 777.77                              |
	| 2               | 777.77                              |
	| 3               | 777.77                              |
	| 4               | 777.77                              |
	| 5               | 777.77                              |
	| 6               | 777.77                              |
	| 7               | 777.77                              |
	| 8               | 777.77                              |
	| 9               | 777.77                              |
	| 10              | 777.77                              |
	| 11              | 777.77                              |
	| 12              | 777.77                              |
	| 13              | 1111.10                             |
	| 14              | 1111.10                             |
	| 15              | 1111.10                             |
	| 16              | 1111.10                             |
	| 17              | 1111.10                             |
	| 18              | 1111.10                             |
	| 19              | 666.66                              |

Scenario: Receiving employer transfer training costs
	Given I am a receiving employer
	And the following commitments have been recorded
	| Apprentice Name   | Course Name | Course Level | Provider Name   | Start Date | Installment Amount | Completion Amount | Number Of Installments | FundingSource |
	| Test Apprentice 1 | Test Course | 1            | Test Provider 1 | Last Year  | 333.33             | 2000              | 24                     | Transfer      |
	| Test Apprentice 2 | Test Course | 1            | Test Provider 2 | Yesterday  | 444.44             | 2000              | 18                     | Transfer      |
	| Test Apprentice 3 | Test Course | 1            | Test Provider 3 | Next Year  | 666.66             | 2000              | 12                     | Transfer      |
	When the account projection is triggered after a payment run
	Then the account projection should be generated
	And should have following projections
	| Months From Now | Transfer In Total Cost Of Training | Transfer Out Total Cost Of Training |
	| 0               | 333.33                             | 333.33                              |
	| 1               | 777.77                             | 777.77                              |
	| 2               | 777.77                             | 777.77                              |
	| 3               | 777.77                             | 777.77                              |
	| 4               | 777.77                             | 777.77                              |
	| 5               | 777.77                             | 777.77                              |
	| 6               | 777.77                             | 777.77                              |
	| 7               | 777.77                             | 777.77                              |
	| 8               | 777.77                             | 777.77                              |
	| 9               | 777.77                             | 777.77                              |
	| 10              | 777.77                             | 777.77                              |
	| 11              | 777.77                             | 777.77                              |
	| 12              | 777.77                             | 777.77                              |
	| 13              | 1111.10                            | 1111.10                             |
	| 14              | 1111.10                            | 1111.10                             |
	| 15              | 1111.10                            | 1111.10                             |
	| 16              | 1111.10                            | 1111.10                             |
	| 17              | 1111.10                            | 1111.10                             |
	| 18              | 1111.10                            | 1111.10                             |
	| 19              | 666.66                             | 666.66                              |