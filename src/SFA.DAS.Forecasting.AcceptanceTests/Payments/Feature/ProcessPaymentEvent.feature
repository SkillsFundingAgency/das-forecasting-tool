Feature: ProcessPaymentEvent
	As an employer
	I want my payments to be forecast for the next 4 years
	So that I can effectively forecast my account balance

Scenario: AC1: Store payment event data
	Given payment events have been created
	Then there are 3 payment events stored
	And all of the data stored is correct
	And the aggregation for the total cost of training has been created properly

Scenario: AC2: do not store invalid data
	Given payment events have been created
	And events with invalid data have been created
	Then there are 3 payment events stored
	And the event with invalid data is not stored