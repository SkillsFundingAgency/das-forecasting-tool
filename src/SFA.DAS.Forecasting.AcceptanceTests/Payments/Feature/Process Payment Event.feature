Feature: Process Payment Event
	As an employer
	I want my payments to be forecast for the next 4 years
	So that I can effectively forecast my account balance

Background:
	Given I have no existing payments
	And I have no existing commitments

Scenario: AC1: Store payment event data
	Given I have made the following payments
	| ApprenticeshipId | ProviderId |
	| 1234             | 7000       |
	| 5678             | 3000       |
	When the SFA Employer HMRC Payment service notifies the Forecasting service of the payment
	Then the Forecasting Payment service should store the payment declarations
	And the Forecasting Payment service should store the commitment declarations

Scenario: AC2: do not store invalid data
	Given I made some invalid payments
	When the SFA Employer HMRC Payment service notifies the Forecasting service of the payment
	Then the Forecasting Payment service should not store the payments	
	And the Forecasting Payment service should not store commitments	