Feature: EditEstimationApprenticeships
	As an Employer
	I want to be abler to edit an Apprenticeships record

Scenario: Edit apprenticeship
	Given that I am an employer
	And I have an estimated apprenticeships record
	When I navigate to the Estimated costs page
	And I click on the Edit link
	Then I am on the edit apprenticeship page
	When I edit number of apprenticeship to be 3
	Then total cost will be 15,000
	When I change the start date to be one year later
	Then total cost will be 30,000 