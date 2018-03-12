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
	| Test Apprentice 3  | Test Course | 1            | Test Provider | 16/04/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice 4  | Test Course | 1            | Test Provider | 16/04/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice 5  | Test Course | 1            | Test Provider | 16/04/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice 6  | Test Course | 1            | Test Provider | 16/04/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice 7  | Test Course | 1            | Test Provider | 29/05/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice 8  | Test Course | 1            | Test Provider | 29/05/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice 9  | Test Course | 1            | Test Provider | 29/05/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice 10 | Test Course | 1            | Test Provider | 29/05/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice 11 | Test Course | 1            | Test Provider | 29/05/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice 12 | Test Course | 1            | Test Provider | 29/05/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice 13 | Test Course | 1            | Test Provider | 12/06/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice 14 | Test Course | 1            | Test Provider | 12/06/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice 15 | Test Course | 1            | Test Provider | 12/06/2017 00:00 | 133.33             | 400.00            | 12                     |
	When the account projection is triggered after a payment run
	Then the account projection should be generated
	And the training costs should be included in the correct months

Scenario: AC2: Training cost multiple apprenticeships with different numbers of instalments
	Given the following commitments have been recorded
	| Apprentice Name | Course Name | Course Level | Provider Name | Start Date       | Installment Amount | Completion Amount | Number Of Installments |
	| Test Apprentice | Test Course | 1            | Test Provider | 31/10/2017 00:00 | 1.00               | 0.00              | 5                      |
	| Test Apprentice | Test Course | 1            | Test Provider | 31/10/2017 00:00 | 1.00               | 0.00              | 5                      |
	| Test Apprentice | Test Course | 1            | Test Provider | 31/10/2017 00:00 | 1.00               | 0.00              | 5                      |
	| Test Apprentice | Test Course | 1            | Test Provider | 31/10/2017 00:00 | 1.00               | 0.00              | 5                      |
	| Test Apprentice | Test Course | 1            | Test Provider | 31/10/2017 00:00 | 1.00               | 0.00              | 5                      |
	| Test Apprentice | Test Course | 1            | Test Provider | 31/10/2017 00:00 | 1.00               | 0.00              | 5                      |
	| Test Apprentice | Test Course | 1            | Test Provider | 29/10/2017 00:00 | 1.00               | 0.00              | 6                      |
	| Test Apprentice | Test Course | 1            | Test Provider | 29/10/2017 00:00 | 1.00               | 0.00              | 6                      |
	| Test Apprentice | Test Course | 1            | Test Provider | 29/10/2017 00:00 | 1.00               | 0.00              | 6                      |
	| Test Apprentice | Test Course | 1            | Test Provider | 29/10/2017 00:00 | 1.00               | 0.00              | 6                      |
	| Test Apprentice | Test Course | 1            | Test Provider | 29/10/2017 00:00 | 1.00               | 0.00              | 6                      |
	| Test Apprentice | Test Course | 1            | Test Provider | 29/10/2017 00:00 | 1.00               | 0.00              | 6                      |
	| Test Apprentice | Test Course | 1            | Test Provider | 29/10/2017 00:00 | 1.00               | 0.00              | 6                      |
	| Test Apprentice | Test Course | 1            | Test Provider | 29/10/2017 00:00 | 1.00               | 0.00              | 6                      |
	| Test Apprentice | Test Course | 1            | Test Provider | 16/05/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 16/05/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 16/05/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 16/05/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 16/05/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 16/05/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 29/06/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 29/06/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 29/06/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 29/06/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 29/06/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 29/06/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 12/07/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 12/07/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 12/07/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 09/08/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 09/08/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 09/08/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 09/08/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 09/08/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 09/08/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 05/10/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 05/11/2017 00:00 | 1.00               | 0.00              | 11                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 05/10/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 05/10/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 05/10/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 05/10/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 05/11/2017 00:00 | 1.00               | 0.00              | 11                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 05/11/2017 00:00 | 1.00               | 0.00              | 11                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 05/11/2017 00:00 | 1.00               | 0.00              | 11                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 05/10/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 05/10/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 05/10/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 05/10/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 05/10/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 100.00             | 300.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/11/2017 00:00 | 1.00               | 0.00              | 11                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/11/2017 00:00 | 1.00               | 0.00              | 11                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/11/2017 00:00 | 1.00               | 0.00              | 11                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/11/2017 00:00 | 1.00               | 0.00              | 11                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 100.00             | 300.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/11/2017 00:00 | 1.00               | 0.00              | 11                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 100.00             | 300.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/11/2017 00:00 | 1.00               | 0.00              | 11                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 100.00             | 300.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 100.00             | 300.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 100.00             | 300.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 07/11/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 07/11/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 07/11/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 07/11/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 07/11/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 07/11/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 14/11/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 14/11/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 14/11/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 14/11/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 14/11/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 14/11/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 14/11/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 14/11/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 14/11/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 14/11/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 14/11/2017 00:00 | 133.33             | 400.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 14/11/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 15/11/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 15/11/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 15/11/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 15/11/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 15/11/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 15/11/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 15/11/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 15/11/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 15/11/2017 00:00 | 166.67             | 500.00            | 12                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 400.00             | 1800.00           | 18                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 400.00             | 1800.00           | 18                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 400.00             | 1800.00           | 18                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 400.00             | 1800.00           | 18                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 400.00             | 1800.00           | 18                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 400.00             | 1800.00           | 18                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 400.00             | 1800.00           | 18                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 26/10/2017 00:00 | 400.00             | 1800.00           | 18                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 31/10/2017 00:00 | 84.21              | 400.00            | 19                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 31/10/2017 00:00 | 84.21              | 400.00            | 19                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 31/10/2017 00:00 | 84.21              | 400.00            | 19                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 30/09/2017 00:00 | 240.00             | 1800.00           | 30                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 30/09/2017 00:00 | 240.00             | 1800.00           | 30                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 30/09/2017 00:00 | 240.00             | 1800.00           | 30                     |
	| Test Apprentice | Test Course | 1            | Test Provider | 30/09/2017 00:00 | 240.00             | 1800.00           | 30                     |
	When the account projection is triggered after a payment run
	Then the account projection should be generated
	And the training costs should be included in the correct months