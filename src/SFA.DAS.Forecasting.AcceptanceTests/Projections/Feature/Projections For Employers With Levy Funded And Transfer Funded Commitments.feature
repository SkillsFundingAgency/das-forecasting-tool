Feature: Projections For Employers With Levy Funded And Transfer Funded Commitments
	As an Employer
	I want my transferred funds in to be considered in my forecast
	So that the balance of my account can be accurately calculated

Background:
	Given I'm a levy paying employer
	And the payroll period is 
	| Payroll Year | Payroll Month |
	| 18-19        | 1             |
	And the following levy declarations have been recorded
	| Scheme   | Amount | Created Date |
	| ABC-1234 | 3000   | Today        |
	And the current balance is 5000

Scenario: Receiving employer account has transfers in
	Given I am a sending employer
	And the following commitments have been recorded
	| Apprentice Name   | Course Name   | Course Level | Provider Name   | Start Date | Installment Amount | Completion Amount | Number Of Installments | FundingSource |
	| Test Apprentice 1 | Test Course 1 | 1            | Test Provider 1 | Yesterday  | 666.66             | 2000              | 12                     | Levy          |
	| Test Apprentice 2 | Test Course 2 | 1            | Test Provider 2 | Yesterday  | 444.44             | 2000              | 18                     | Transfer      |
	When the account projection is triggered after a payment run
	Then the account projection should be generated
	And should have the following projected values
	| MonthsFromNow | TotalCostOfTraining | TransferOutTotalCostOfTraining | TransferInTotalCostOfTraining | TransferInCompletionPayments | CompletionPayments | TransferOutCompletionPayments | FutureFunds |
	| 0             | 0.00                | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 8000.00     |
	| 1             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 9888.90     |
	| 2             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 11777.80    |
	| 3             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 13666.70    |
	| 4             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 15555.60    |
	| 5             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 17444.50    |
	| 6             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 19333.40    |
	| 7             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 21222.30    |
	| 8             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 23111.20    |
	| 9             | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 25000.1     |
	| 10            | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 26889.00    |
	| 11            | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 28777.90    |
	| 12            | 666.66              | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 30666.80    |
	| 13            | 0.00                | 444.44                         | 0.00                          | 0.00                         | 2000.00            | 0.00                          | 31222.36    |
	| 14            | 0.00                | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 33777.92    |
	| 15            | 0.00                | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 36333.48    |
	| 16            | 0.00                | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 38889.04    |
	| 17            | 0.00                | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 41444.60    |
	| 18            | 0.00                | 444.44                         | 0.00                          | 0.00                         | 0.00               | 0.00                          | 44000.16    |
	| 19            | 0.00                | 0.00                           | 0.00                          | 0.00                         | 0.00               | 2000.00                       | 45000.16    |
	| 20            | 0.00                | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 48000.16    |