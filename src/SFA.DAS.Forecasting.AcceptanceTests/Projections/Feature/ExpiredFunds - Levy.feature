Feature: ExpiredFunds - [CI-893]
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

Background:
Given I'm a levy paying employer
And the payroll period is 
| Payroll Year | Payroll Month |
| 18-19        | 1             |
And the following commitments have been recorded
| Apprentice Name   | Course Name | Course Level | Provider Name | Start Date | Installment Amount | Completion Amount | Number Of Installments |
| Test Apprentice 1 | Test Course | 1            | Test Provider | Yesterday  | 300                | 3000              | 24                     |
And the current balance is 5000
And I have no existing levy declarations


Scenario: no expired funds in any month
When the account projection is triggered after levy has been declared
Then the account projection should be generated
And should have the following projected values
	| MonthsFromNow | TotalCostOfTraining | TransferOutTotalCostOfTraining	| TransferInTotalCostOfTraining | TransferInCompletionPayments | CompletionPayments | TransferOutCompletionPayments | FutureFunds | ExpiredFunds |
	| 0             | 0.00                | 0.00                            | 0.00                          | 0.00                         | 0.00               | 0.00                          | 5000        | 0            |

Scenario: expired funds in any month - Levy
Given the following levy declarations have been recorded
         | Scheme   | Amount | Created Date		| UseCreatedDateAsPayrollDate |
		 | abc-123	| 15,000 |   3 months ago	| true                    |
		 | abc-123	| 15,000 |   2 months ago	| true                    |
		 | abc-123	| 15,000 |   1 months ago	| true                    |
		 | abc-123	| 15,000 |   Today			| true                    |
And At least one levy declaration which has expired
When the account projection is triggered after a levy run
Then the account projection should be generated
And should have the following projected values
	| MonthsFromNow | TotalCostOfTraining | TransferOutTotalCostOfTraining | TransferInTotalCostOfTraining | TransferInCompletionPayments | CompletionPayments | TransferOutCompletionPayments | FutureFunds | ExpiredFunds |
	| 0             | 0.00                | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 5000.00     | 0            |
	| 1             | 300.00              | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 19700.00    | 0            |
	| 8             | 300.00              | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 109700.00   | 12900.00     |
	| 9             | 300.00              | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 122600.00   | 14700.00     |
	| 10            | 300.00              | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 137300.00   | 14700.00     |
	| 11            | 300.00              | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 152000.00   | 14700.00     |
	| 12            | 300.00              | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 181400.00   | 0.00         |

Scenario: expired funds in any month - Before Payment
Given the following levy declarations have been recorded
           | Scheme   | Amount | Created Date		| UseCreatedDateAsPayrollDate |
		 | abc-123	| 15,000 |   3 months ago	| true                    |
		 | abc-123	| 15,000 |   2 months ago	| true                    |
		 | abc-123	| 15,000 |   1 months ago	| true                    |
		 | abc-123	| 15,000 |   Today			| true                    |
And At least one levy declaration which has expired
When the account projection is triggered after levy has been declared
Then the account projection should be generated
And should have the following projected values
	| MonthsFromNow | TotalCostOfTraining | TransferOutTotalCostOfTraining | TransferInTotalCostOfTraining | TransferInCompletionPayments | CompletionPayments | TransferOutCompletionPayments | FutureFunds | ExpiredFunds |
	| 0             | 0.00                | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 5000.00     | 0            |
	| 1             | 300.00              | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 19700.00    | 0            |
	| 8             | 300.00              | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 109700.00   | 12900.00     |
	| 9             | 300.00              | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 122600.00   | 14700.00     |
	| 10            | 300.00              | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 137300.00   | 14700.00     |
	| 11            | 300.00              | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 152000.00   | 14700.00     |
	| 12            | 300.00              | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 181400.00   | 0.00         |


Scenario: expired funds in any month - After Payment
Given the following levy declarations have been recorded
           | Scheme   | Amount | Created Date	| UseCreatedDateAsPayrollDate |
		 | abc-123	| 15,000 |   3 months ago	| true                    |
		 | abc-123	| 15,000 |   2 months ago	| true                    |
		 | abc-123	| 15,000 |   1 months ago	| true                    |
		 | abc-123	| 15,000 |   Today			| true                    |
And At least one levy declaration which has expired
When the account projection is triggered after a payment run
Then the account projection should be generated
And should have the following projected values
	| MonthsFromNow | TotalCostOfTraining | TransferOutTotalCostOfTraining | TransferInTotalCostOfTraining | TransferInCompletionPayments | CompletionPayments | TransferOutCompletionPayments | FutureFunds | ExpiredFunds |
	| 0             | 0.00                | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 20000.00    | 0            |
	| 1             | 300.00              | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 34700.00    | 0            |
	| 8             | 300.00              | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 124700.00   | 12900.00     |
	| 9             | 300.00              | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 137600.00   | 14700.00     |
	| 10            | 300.00              | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 152300.00   | 14700.00     |
	| 11            | 300.00              | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 167000.00   | 14700.00     |
	| 12            | 300.00              | 0.00                           | 0.00                          | 0.00                         | 0.00               | 0.00                          | 196400.00   | 0.00         |

