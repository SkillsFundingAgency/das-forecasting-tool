Feature: CoInvestmentFundedPayments

Scenario: Co-Invested payments
	Given I am a sending employer
	And the following commitments have been recorded
	| Apprentice Name   | Course Name   | Course Level | Provider Name   | Start Date | Installment Amount | Completion Amount | Number Of Installments | FundingSource |
	| Test Apprentice 1 | Test Course 1 | 1            | Test Provider 1 | Yesterday  | 666.66             | 2000              | 12                     | Levy          |
	| Test Apprentice 2 | Test Course 2 | 1            | Test Provider 2 | Yesterday  | 444.44             | 2000              | 12                     | CoInvestedEmployer |
	| Test Apprentice 2 | Test Course 2 | 1            | Test Provider 2 | Yesterday  | 444.44             | 2000              | 12                     | CoInvestedSfa |
	| Test Apprentice 2 | Test Course 2 | 1            | Test Provider 2 | Yesterday  | 444.44             | 2000              | 12                     | FullyFundedSfa |
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