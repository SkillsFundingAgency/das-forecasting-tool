Feature: ProcessLevyEvent [CI-528]
	As an employer
	I want my levy credit to be forecast for the next 4 years
	So that I can effectively forecast my account balance

Scenario: AC1 Store levy credit event data
Given that I'm the ESFA
And I have credited levy to employer accouns
And levy credit events have been created
Then the data for each levy credit event is stored
And all of the data stored is correct