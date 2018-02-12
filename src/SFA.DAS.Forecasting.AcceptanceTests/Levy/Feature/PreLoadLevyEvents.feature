Feature: PreLoadLevyEvents
	In order to avoid silly mistakes
	I want to be told the sum of two numbers
	
Scenario: Pre load levy events
	Given I trigger function for 3 employers to have their data loaded.
	When data have been processed
	Then there will be 3 records in the storage
	And all records should have the latest data