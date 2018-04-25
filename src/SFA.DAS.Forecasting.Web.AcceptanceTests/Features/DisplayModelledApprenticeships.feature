Feature: DisplayModelledApprenticeships
		As an employer
		I want to be able to see the apprenticeships that I have modelled
		So that I can understand the details behind the modelled costs

@mytag
Scenario: AC2: Check costs of modelled apprenticeships
Given that I have added apprenticeships
When the modelled apprenticeships page is displayed
Then the column headings are displayed
And each added apprenticeship is displayed
And each one is in a separate row
And the apprenticeship with the earliest start date is shown first
And the other apprenticeships are in order of start date
And the details against each apprenticeship match what was entered
