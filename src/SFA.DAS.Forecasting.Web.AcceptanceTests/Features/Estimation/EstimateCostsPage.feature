Feature: EstimateCostsPage
	As an employer
	I want to see how my modelled costs compare against my transfer allowance projection
	So that I can make a decision on whether to proceed with a transfer

Scenario: AC1: add more apprenticeships link
Given that I'm on the Estimated Costs page
When I click on the 'Add more apprenticeships...' link
Then I am taken to the Add Apprenticeships page

Scenario: AC2: tab to modelled apprenticeships
Given that I'm on the Estimated Costs page
When I click on the 'Apprenticeships added' link
Then I am taken to the Apprenticeships added tab

Scenario: AC3: check costs for modelled apprenticeships
Given that I'm on the Estimated Costs page
When I view the remaining transfer allowance tab
Then the first month in the table is the earliest month in which a modelled apprenticeship payment is made (the month after the earliest start date)
And the first month's transfer allowance value is correct
And the first month's modelled costs is correct
And each subsequent month's transfer allowance is correct
And each subsequent month's modelled costs is correct
And in the each April the transfer allowance value resets to its original value
And the last month is the month in which the last completion payment for a modelled apprenticeship will be made

#Repeat AC3 with several modelled apprenticeships each with different start dates

Scenario: AC4: what does the table show
Given that I'm on the Estimated Costs page
When I click on 'What does the table show'
Then the relevant text is displayed
And the text matches the design

Scenario: AC5: can fund apprenticeships
Given that I'm on the Estimated Costs page
When I have modelled apprenticeships
And I can afford those apprenticeships from my transfer allowance
Then the banner message says that I can fund the apprenticeships
And there is no highlighting of any rows in the remaining transfer allowance table

Scenario: AC6: cannot fund apprenticeships
Given that I'm on the Estimated Costs page
When I have modelled apprenticeships
And I cannot afford those apprenticeships from my transfer allowance
Then the banner message says that I can't fund the apprenticeships
And there is highlighting of any rows in the remaining transfer allowance table

Scenario: AC7: no modelled apprenticeships
Given that I'm on the Estimated Costs page
When I have removed my last modelled apprenticeships
Then the table is not displayed
And the message 'You have not selected any apprenticeships...' is displayed in their place
And the 'What does this table show' text is not displayed