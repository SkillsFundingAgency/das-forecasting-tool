Feature: ProcessLevyEvent [CI-528]
	As an employer
	I want my levy credit to be forecast for the next 4 years
	So that I can effectively forecast my account balance

Scenario: AC1 - Store levy credit event data
Given levy credit events have been created
Then there are 3 levy credit events stored
And all of the levy declarations  stored is correct

Scenario: AC2 - Do not store levy credit event data when some data missing
Given levy credit events have been created
And all events with invalid data have been created
Then there are 4 levy credit events stored
And all the event with invalid data is not stored
