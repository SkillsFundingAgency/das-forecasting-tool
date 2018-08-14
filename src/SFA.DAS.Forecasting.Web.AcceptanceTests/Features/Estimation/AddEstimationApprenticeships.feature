Feature: AddEstimationApprenticeships
	As an Employer
	I want to be abler to add an Apprenticeships record

Scenario: Add apprenticeship with multiple funding periods
	Given that I am an employer
	And I have a standard with multiple funding periods
	When I navigate to the Estimated costs page
	And I click on the Add link
	Then I am on the add apprenticeship page
	When I select 'Rockstar Developer' from drop down
	And I add start date for April next year
	And I edit number of apprenticeship to be 3
	Then total cost will be 15,000
	When I change the start date to be one year later
	Then total cost will be 30,000 