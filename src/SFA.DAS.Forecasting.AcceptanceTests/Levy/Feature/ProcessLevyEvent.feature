Feature: ProcessLevyEvent [CI-528]
	As an employer
	I want my levy credit to be forecast for the next 4 years
	So that I can effectively forecast my account balance

Background:
	Given that I'm the ESFA
	And I have credited levy to employer accounts

Scenario: AC1 - Store levy credit event data
	When the employer services notifies the Forecasting service of the Levy Credits
	Then there should be 3 levy credit events stored
	And all of the levy declarations stored should be correct

Scenario: AC2 - Do not store invalid levy credit event data
	When the employer service notifies the Forecasting service of the invalid Levy Credits	
	Then all the event with invalid data is not stored