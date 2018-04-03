Feature: Pre-Load Payments
	As a product owner 
	I want to pre populate the database for some employers
	So that I don't have to wait for the process to run.

Background:
	Given my employer account id "12345"
	And I have no existing payments recorded in the forecasting service
	And I have no existing commitments recorded in the forecasting service
	And the collection period is
	| Id       | Month | Year |
	| 1718-R07 | 2     | 2018 |
	And the funding source for the payments is "Levy"
	
Scenario: Pre load payments	
	Given payments for the following apprenticeships have been recorded in the Payments service
	| Payment Amount | Apprentice Name   | Course Name   | Course Level | Provider Name   | Start Date | Installment Amount | Completion Amount | Number Of Installments | Delivery Period Month | Delivery Period Year |
	| 166.66667      | Test Apprentice 1 | Test Course 1 | 1            | Test Provider 1 | 01/01/2018 | 166.66667          | 500.00            | 12                     | 2                     | 2018                 |
	| 83.33333       | Test Apprentice 2 | Test Course 2 | 2            | Test Provider 2 | 01/01/2018 | 83.33333           | 250.00            | 24                     | 2                     | 2018                 |
	And the payments have also been recorded in the Employer Accounts Service
	When I trigger the pre-load of the payment events
	Then the funding projections payments service should record the payments
	And the funding projections commitments service should record the commitments

#Scenario: Payments for multiple delivery periods collected in same period
#	Given payments for the following apprenticeships have been recorded in the Payments service
#	| Payment Amount | Apprentice Name   | Course Name   | Course Level | Provider Name   | Start Date | Installment Amount | Completion Amount | Number Of Installments | Delivery Period Month | Delivery Period Year |
#	| 166.66667      | Test Apprentice 1 | Test Course 1 | 1            | Test Provider 1 | 01/01/2018 | 166.66667          | 500.00            | 12                     | 2                     | 2018                 |
#	When I trigger the pre-load of the payment events
#	Then the funding projections payments service should record the payments
#	And the funding projections commitments service should record the commitments

Scenario: Pre load anonymised payments	
	Given payments for the following apprenticeships have been recorded in the Payments service
	| Payment Amount | Apprentice Name   | Course Name   | Course Level | Provider Name   | Start Date | Installment Amount | Completion Amount | Number Of Installments | Delivery Period Month | Delivery Period Year |
	| 166.66667      | Test Apprentice 1 | Test Course 1 | 1            | Test Provider 1 | 01/01/2018 | 166.66667          | 500.00            | 12                     | 2                     | 2018                 |
	| 83.33333       | Test Apprentice 2 | Test Course 2 | 2            | Test Provider 2 | 01/01/2018 | 83.33333           | 250.00            | 24                     | 2                     | 2018                 |
	And the payments have also been recorded in the Employer Accounts Service
	When I trigger the pre-load of anonymised payment events
	Then the funding projections payments service should record the anonymised payments
	And the Payment Id should be anonymised
	And the Apprenticeship Id should be anonymised	
	And the funding projections commitments service should record the anonymised commitments

Scenario: Pre load multiple anonymised payments for a single apprenticeship
	Given payments for the following apprenticeship have been recorded in the Payments service
	| Payment Amount | Apprentice Name   | Course Name   | Course Level | Provider Name   | Start Date | Installment Amount | Completion Amount | Number Of Installments | Delivery Period Month | Delivery Period Year |
	| 166.66667      | Test Apprentice 1 | Test Course 1 | 1            | Test Provider 1 | 01/01/2018 | 166.66667          | 500.00            | 12                     | 2                     | 2018                 |
	| 83.33333       | Test Apprentice 1 | Test Course 1 | 1            | Test Provider 1 | 01/01/2018 | 83.33333           | 250.00            | 24                     | 1                     | 2018                 |
	| 333.333334     | Test Apprentice 1 | Test Course 1 | 1            | Test Provider 1 | 01/01/2018 | 333.333334         | 1000.00           | 6                      | 12                    | 2017                 |
	And the payments have also been recorded in the Employer Accounts Service
	When I trigger the pre-load of anonymised payment events
	Then the funding projections payments service should record the anonymised payments
	And the Payment Id should be anonymised
	And the Apprenticeship Id should be anonymised	
	And the funding projections commitments service should record the anonymised commitment