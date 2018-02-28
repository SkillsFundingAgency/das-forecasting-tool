Feature: PreLoadLevyEvents
	Pre load levy declarations
	
Scenario: Pre load levy events
	Given I trigger function for 3 employers to have their data loaded.
	When data have been processed
	Then there will be 3 records in the storage

Scenario: Pre load levy events with substitution id
	Given I trigger PreLoadEvent function for some employers with a substitution id 12345
	When data have been processed
	Then there will be a levy declaration for the employer 12345 and no sensitive data will have been stored in the database