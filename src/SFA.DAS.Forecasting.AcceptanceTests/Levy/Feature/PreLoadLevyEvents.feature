Feature: PreLoadLevyEvents
	Pre load levy declarations
	
Scenario: Pre load levy events
	Given I trigger function for 3 employers to have their data loaded.
	When data have been processed
	Then there will be a record in the storage for employer 497
	And there will be 1 records in the storage for employer 497

Scenario: Pre load levy events with substitution id
	Given I trigger PreLoadEvent function for some employers with a substitution id 12345
	When data have been processed
	Then there will be 0 records in the storage for employer 497
	Then there will be 1 records in the storage for employer 12345