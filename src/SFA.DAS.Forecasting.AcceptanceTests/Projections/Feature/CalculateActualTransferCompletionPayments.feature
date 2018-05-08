Feature: Calculate actual transfer completion payments (CI-620)
	As an employer
	I want to see my actual training completion costs for transfers that I'm funding
	So that they can be taken into account while forecasting against my transfer allowance

Background:
	Given I'm a levy paying employer
	And the payroll period is 
	| Payroll Year | Payroll Month |
	| 18-19        | 1             |
	And the following levy declarations have been recorded
	| Scheme   | Amount | Created Date |
	| ABC-1234 | 3000   | Today        |
	And the current balance is 5000

Scenario: AC1: Multiple transfer commitments
	Given the following commitments have been recorded
	| Apprentice Name   | Course Name   | Course Level | Provider Name   | Start Date | Installment Amount | Completion Amount | Number Of Installments | EmployerAccountId | SendingEmployerAccountId | FundingSource |
	| Test Apprentice   | Test Course   | 1            | Test Provider 1 | Yesterday  | 2000               | 1200              | 6                      | 999               | 12345                    | 2             |
	| Test Apprentice 1 | Test Course   | 1            | Test Provider 2 | Yesterday  | 2000               | 1200              | 6                      | 999               | 12345                    | 2             |
	| Test Apprentice 2 | Test Course 2 | 1            | Test Provider 3 | Yesterday  | 2000               | 1200              | 6                      | 999               | 12345                    | 2             |
	| Test Apprentice 3 | Test Course   | 1            | Test Provider 4 | Yesterday  | 2000               | 1200              | 6                      | 999               | 12345                    | 2             |
	| Test Apprentice 4 | Test Course 2 | 1            | Test Provider 5 | Next Year  | 2000               | 1200              | 6                      | 999               | 12345                    | 2             |
	When the account projection is triggered after a payment run
	Then the account projection should be generated
	And should have following projections from completion
	| MonthsFromNow | TransferOutCompletionPayments |
	| 6             | 0                             |
	| 7             | 4800                          |
	| 8             | 0                             |
	| 18            | 0                             |
	| 19            | 1200                          |
	| 20            | 0                             |

#AC2: Multiple transfer commitments with end dates in same month
#	Given I have a number of transfer commitments that all end in the next 48 months
#	When many commitments end in the same month
#	Then my transfer completion costs are calculated for each month
#	And only completion payments with funding source of 'Transfer' are included
#	And the completion payments are allocated to the month after the planned end date
#	And all completion payments in the same month are summed
#	And the completion payments for each month are summed
#	And the monthly completion cost values are calculated correctly
#
#AC3: Multiple transfer commitments and some with end dates after end of forecast period
#	Given I have a number of transfer commitments
#	When some have a planned end date after 48 months
#	Then completion payments after 48 months are not included in monthly forecast completion costs

Scenario: AC3: Multiple transfer commitments and some with end dates after end of forecast period
	Given the following commitments have been recorded
	| Apprentice Name   | Course Name   | Course Level | Provider Name   | Start Date | Installment Amount | Completion Amount | Number Of Installments | EmployerAccountId | SendingEmployerAccountId | FundingSource |
	| Test Apprentice   | Test Course   | 1            | Test Provider 1 | Today      | 2000               | 1200              | 48                     | 999               | 12345                    | 2             |
	| Test Apprentice 4 | Test Course 2 | 1            | Test Provider 5 | Today      | 2000               | 1200              | 48                     | 999               | 12345                    | 2             |
	When the account projection is triggered after a payment run
	Then the account projection should be generated
	And should have no payments with TransferOutCompletionPayments

#AC4: Actual end dates populated
#	Given I have a number of transfer commitments
#	When some have the actual end date populated
#	Then transfer completion payments with the actual end date populated are not included in monthly completion cost totals
#
#AC5: Months with no completion payments
#	Given that I'm calculating transfer training cost
#	When I do not have any completion payments in a month
#	Then the transfer completion costs for that month will be zero