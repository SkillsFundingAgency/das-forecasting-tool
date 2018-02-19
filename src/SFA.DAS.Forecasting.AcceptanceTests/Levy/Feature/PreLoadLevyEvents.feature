Feature: PreLoadLevyEvents
	Pre load levy declarations
	
Scenario: Pre load levy events
	Given I trigger function for 3 employers to have their data loaded.
	When data have been processed
	Then there will be 3 records in the storage