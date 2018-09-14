Feature: Projections For Employers With Non Paid Apprenticeships
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

Scenario: Sending employer account has transfers in with non paid apprenticeships
	Given I am a sending employer
	And the following commitments have been recorded
	| Apprentice Name   | Course Name   | Course Level | Provider Name   | Start Date | Installment Amount | Completion Amount | Number Of Installments | FundingSource | HasHadPayment |
	| Test Apprentice 1 | Test Course 1 | 1            | Test Provider 1 | Yesterday  | 600                | 2000              | 12                     | Levy          | 0             |
	| Test Apprentice 2 | Test Course 2 | 1            | Test Provider 2 | Next Month | 400                | 2000              | 18                     | Transfer      | 0             |
	When the account projection is triggered after a payment run
	Then the account projection should be generated
	And should have the following projected values
	| MonthsFromNow | TotalCostOfTraining | TransferOutTotalCostOfTraining | TransferInTotalCostOfTraining | TransferInCompletionPayments | CompletionPayments | TransferOutCompletionPayments | FutureFunds |
	| 0             | 0.00                | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 8000        |
	| 1             | 600                 | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 10400       |
	| 2             | 600                 | 400                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 12400       |
	| 3             | 600                 | 400                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 14400       |
	| 4             | 600                 | 400                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 16400       |
	| 5             | 600                 | 400                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 18400       |
	| 6             | 600                 | 400                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 20400       |
	| 7             | 600                 | 400                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 22400       |
	| 8             | 600                 | 400                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 24400       |
	| 9             | 600                 | 400                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 26400       |
	| 10            | 600                 | 400                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 28400       |
	| 11            | 600                 | 400                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 30400       |
	| 12            | 600                 | 400                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 32400       |
	| 13            | 0.00                | 400                            | 0.00                          | 0.00                         | 2000.00            | 0.00                          | 33000       |
	| 14            | 0.00                | 400                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 35600       |
	| 15            | 0.00                | 400                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 38200       |
	| 16            | 0.00                | 400                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 40800       |
	| 17            | 0.00                | 400                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 43400       |
	| 18            | 0.00                | 400                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 46000       |
	| 19            | 0.00                | 400                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 48600       |
	| 20            | 0.00                | 0.00                           | 0.00                          | 0.00                         | 0.00               | 2000.00                       | 49600       |

	Scenario: Receiving employer account has transfers in with non paid apprenticeships
	Given I am a receiving employer
	And the following commitments have been recorded
	| Apprentice Name   | Course Name   | Course Level | Provider Name   | Start Date | Installment Amount | Completion Amount | Number Of Installments | FundingSource | HasHadPayment |
	| Test Apprentice 1 | Test Course 1 | 1            | Test Provider 1 | Yesterday  | 3500               | 2000              | 12                     | Levy          | 0             |
	| Test Apprentice 2 | Test Course 2 | 1            | Test Provider 2 | Next Month | 400                | 2000              | 18                     | Transfer      | 0             |
	When the account projection is triggered after a payment run
	Then the account projection should be generated
	And should have the following projected values
	| MonthsFromNow | TotalCostOfTraining | TransferOutTotalCostOfTraining | TransferInTotalCostOfTraining | TransferInCompletionPayments | CompletionPayments | TransferOutCompletionPayments | FutureFunds |
	| 0             | 0.00                | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 8000        |
	| 1             | 3500                | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 7500        |
	| 2             | 3500                | 400                            | 400                           | 0.00                         | 0.00               | 0.00                          | 7000        |
	| 3             | 3500                | 400                            | 400                           | 0.00                         | 0.00               | 0.00                          | 6500        |
	| 4             | 3500                | 400                            | 400                           | 0.00                         | 0.00               | 0.00                          | 6000        |
	| 5             | 3500                | 400                            | 400                           | 0.00                         | 0.00               | 0.00                          | 5500        |
	| 6             | 3500                | 400                            | 400                           | 0.00                         | 0.00               | 0.00                          | 5000        |
	| 7             | 3500                | 400                            | 400                           | 0.00                         | 0.00               | 0.00                          | 4500        |
	| 8             | 3500                | 400                            | 400                           | 0.00                         | 0.00               | 0.00                          | 4000        |
	| 9             | 3500                | 400                            | 400                           | 0.00                         | 0.00               | 0.00                          | 3500        |
	| 10            | 3500                | 400                            | 400                           | 0.00                         | 0.00               | 0.00                          | 3400        |
	| 11            | 3500                | 400                            | 400                           | 0.00                         | 0.00               | 0.00                          | 3400        |
	| 12            | 3500                | 400                            | 400                           | 0.00                         | 0.00               | 0.00                          | 3400        |
	| 13            | 0.00                | 400                            | 400                           | 0.00                         | 2000.00            | 0.00                          | 4400        |
	| 14            | 0.00                | 400                            | 400                           | 0.00                         | 0.00               | 0.00                          | 7400        |
	| 15            | 0.00                | 400                            | 400                           | 0.00                         | 0.00               | 0.00                          | 10400       |
	| 16            | 0.00                | 400                            | 400                           | 0.00                         | 0.00               | 0.00                          | 13400        |
	| 17            | 0.00                | 400                            | 400                           | 0.00                         | 0.00               | 0.00                          | 16400        |
	| 18            | 0.00                | 400                            | 400                           | 0.00                         | 0.00               | 0.00                          | 19400        |
	| 19            | 0.00                | 400                            | 400                           | 0.00                         | 0.00               | 0.00                          | 22400        |
	| 20            | 0.00                | 0.00                           | 0.00                          | 2000.00                      | 0.00               | 2000.00                       | 25400        |