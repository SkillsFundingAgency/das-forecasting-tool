﻿Feature: Process Payment Event
	As an employer
	I want my payments to be forecast for the next 4 years
	So that I can effectively forecast my account balance

Background:
	Given I have no existing payments
	And I have no existing commitments

Scenario: AC1: Store payment event data
	Given I have made the following payments
	| Payment Amount | Apprentice Name   | Course Name | Course Level | Provider Name | Start Date       | Installment Amount | Completion Amount | Number Of Installments |
	| 133.33         | Test Apprentice   | Test Course | 1            | Test Provider | 16/04/2017 00:00 | 133.33             | 400.00            | 12                     |
	| 133.33         | Test Apprentice 2 | Test Course | 1            | Test Provider | 16/04/2017 00:00 | 133.33             | 400.00            | 12                     |
	When the SFA Employer HMRC Payment service notifies the Forecasting service of the payment
	Then the Forecasting Payment service should store the payment declarations
	And the Forecasting Payment service should store the commitment declarations

Scenario: AC2: Do not store invalid data
	Given I made some invalid payments
	| Payment Amount | Apprentice Name   | Course Name | Course Level | Provider Name | Start Date       | Installment Amount | Completion Amount | Number Of Installments |
	| 0              | Test Apprentice   | Test Course | 1            | Test Provider | 16/04/2017 00:00 | 133.33             | 400.00            | 12                     |
	| 133.33         |                   | Test Course | 1            | Test Provider | 16/04/2017 00:00 | 133.33             | 400.00            | 12                     |
	| 133.33         | Test Apprentice 3 |             | 1            | Test Provider | 16/04/2017 00:00 | 133.33             | 400.00            | 12                     |
	| 133.33         | Test Apprentice 4 | Test Course | 1            | Test Provider | 01/01/0001 00:00 | 133.33             | 400.00            | 12                     |
	| 133.33         | Test Apprentice 5 | Test Course | 1            | Test Provider | 16/04/2017 00:00 | 0                  | 400.00            | 12                     |
	| 133.33         | Test Apprentice 6 | Test Course | 1            | Test Provider | 16/04/2017 00:00 | 133.33             | 0                 | 12                     |
	| 133.33         | Test Apprentice 7 | Test Course | 1            | Test Provider | 29/05/2017 00:00 | 133.33             | 400.00            | 0                      |
	When the SFA Employer HMRC Payment service notifies the Forecasting service of the payment
	Then the Forecasting Payment service should not store the payments	
	And the Forecasting Payment service should not store commitments

Scenario: Ensure sending employer transfer payments are processed
	Given I have made the following payments
	| Payment Amount | Apprentice Name   | Course Name | Course Level | Provider Name | Start Date       | Installment Amount | Completion Amount | Number Of Installments | Sending Employer Account Id |
	| 133.33         | Test Apprentice   | Test Course | 1            | Test Provider | 16/04/2017 00:00 | 133.33             | 400.00            | 12                     | 100021                      |
	| 133.33         | Test Apprentice 2 | Test Course | 1            | Test Provider | 16/04/2017 00:00 | 133.33             | 400.00            | 12                     | 100022                      |
	| 133.33         | Test Apprentice 3 | Test Course | 1            | Test Provider | 16/04/2017 00:00 | 133.33             | 400.00            | 12                     | 100021                      |
	| 133.33         | Test Apprentice 4 | Test Course | 1            | Test Provider | 16/04/2017 00:00 | 133.33             | 400.00            | 12                     |                             |
	| 133.33         | Test Apprentice 5 | Test Course | 1            | Test Provider | 16/04/2017 00:00 | 133.33             | 400.00            | 12                     | 12345                       |
	When the SFA Employer HMRC Payment service notifies the Forecasting service of the payment
	Then the Forecasting Payment service should store the payment declarations
	And the Forecasting Payment service should store the commitment declarations

Scenario: Ensure receiving employer transfer payments are processed (CI-762)
	And I have made the following payments
	| Payment Amount | Apprentice Name   | Course Name | Course Level | Provider Name | Start Date       | Installment Amount | Completion Amount | Number Of Installments | Sending Employer Account Id | FundingSource |
	| 133.33         | Test Apprentice   | Test Course | 1            | Test Provider | 16/04/2015 00:00 | 133.33             | 400.00            | 12                     | 1                           | Transfer      |
	| 133.33         | Test Apprentice 2 | Test Course | 1            | Test Provider | 16/04/2015 00:00 | 133.33             | 400.00            | 12                     | 1                           | Transfer      |
	
	When the SFA Employer HMRC Payment service notifies the Forecasting service of the payment
	Then the Forecasting Payment service should store the payment declarations receiving employer 12345 from sending employer 1
	And the Forecasting Payment service should store the commitment declarations for receiving employer 12345 from sending employer 1
