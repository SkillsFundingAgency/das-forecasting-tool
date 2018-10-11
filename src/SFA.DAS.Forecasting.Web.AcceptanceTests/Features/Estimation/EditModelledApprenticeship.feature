Feature: EditModelledApprenticeship
	As an employer
	I want to be able to edit my estimate for apprenticeships funded through transfers
	So that I can see the impact of different values on the estimate

@EditModelledApprenticeship
Scenario: AC1: Edit apprenticeship
	Given that I'm on the Estimated Costs - Apprenticeships Added page
	When I select edit
	Then the Edit Apprenticeship page is displayed
	And the default details are those of the apprenticeship I wish to edit

@EditModelledApprenticeship
Scenario: AC2: Enter start date in past
	Given that I'm on the Edit apprenticeships page
	When I edit a start date before the current month
	Then the error message 'The start date cannot be in the past' is displayed

@EditModelledApprenticeship
Scenario: AC3: Enter start date before 05/2018
	Given that I'm on the Edit apprenticeships page
	When I edit a start date before the May Twenty Eighteen
	Then the error message 'The start date cannot be in the past' is displayed

@EditModelledApprenticeship
Scenario: AC4: Enter start date in the past and after 05/2018
	Given that I'm on the Edit apprenticeships page
	And the current date is after 05/2018
	When I edit a start date before the current month
	Then the error message 'The start date cannot be in the past' is displayed

@EditModelledApprenticeship
Scenario: AC5: Enter training cost more than funding cap
	Given that I'm on the Edit apprenticeships page
	When I enter a training cost more than the funding cap
	Then the error message 'The total cost cant be higher than the government funding cap for this apprenticeship' is not displayed

@EditModelledApprenticeship
Scenario:  AC6: Enter valid apprenticeship details
	Given that I'm on the Edit apprenticeships page
	When I update the details (choose any editable fields)
	And I select the 'Check if I can fund this' button
	Then the 'Estimated cost - Apprenticeships added' page is displayed
	And the apprenticeship has been updated correctly

@EditModelledApprenticeship
Scenario: AC7: Change number of apprentices
	Given that I'm on the Edit apprenticeships page
	When I change the number of apprentices
	Then the government funding cap is updated
	And the total cost value is updated

@EditModelledApprenticeship
Scenario: AC8: cancel
	Given that I'm on the Edit apprenticeships page
	When I click on cancel
	Then the 'Estimated cost - Apprenticeships added' page is displayed
	And the details have not been updated

@EditModelledApprenticeship
Scenario: AC9: back link
	Given that I'm on the Edit apprenticeships page
	When I click on the back link
	Then the 'Estimated cost - Apprenticeships added' page is displayed
	And the details have not been updated
