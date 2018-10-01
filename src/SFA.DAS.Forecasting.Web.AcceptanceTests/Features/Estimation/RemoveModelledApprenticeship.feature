Feature: RemoveModelledApprenticeship
	As an employer
	I want to be able to remove apprenticeships from my estimate
	So thaft I can only see an estimate for the desired apprenticeships

@RemoveModelledApprenticeship
Scenario: AC1: remove modelled apprenticeship
Given that I'm on the modelled apprenticeships tab
When I select remove for one apprenticeship
Then the remove apprenticeship page is displayed

@RemoveModelledApprenticeship
Scenario: AC2: confirm remove modelled apprenticeship
Given that I'm on the remove apprenticeship page
When I confirm remove for the apprenticeship
Then the Estimated Costs page is displayed
And the Remaining Transfer Allowance tab is displayed
And the removed apprenticeship is not in the list
And the list of apprenticeships is in start date order
And the banner message 'Apprenticeship removed' is displayed

@RemoveModelledApprenticeship
Scenario: AC3: confirm costs deducted for removed modelled apprenticeship
Given that I have removed an apprenticeship
When I'm on the Estimated costs page
And the 'Remaining transfer allowance' tab
Then the monthly costs of the removed apprenticeships have been deducted from the correct months
And the completion costs of the removed apprenticeships have been removed from the correct month

@RemoveModelledApprenticeship
Scenario: AC4: continue with default radio button selected
Given that I'm on the remove apprenticeship page
And the No radio button is defaulted as selected
When I click continue
Then the Estimated Costs is displayed