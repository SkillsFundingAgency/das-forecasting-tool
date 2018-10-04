Feature: DisplayModelledApprenticeships
		As an employer
		I want to be able to see the apprenticeships that I have modelled
		So that I can understand the details behind the modelled costs


Background:	
	Given that I am an employer with predefined projections
	And I have logged into my Apprenticeship Account
	And that I'm on the estimator start page

@DisplayModelledApprenticeships
Scenario: AC2: Check costs of modelled apprenticeships
	Given that I have added the following apprenticeships
	 | Apprenticeship						 | Number Of Apprentices | Number Of Months | Start Date Month | Start Date Year | Total Cost |
	 | Advanced butcher, Level: 3 (Standard) | 1                     | 12               | 3                | 2019            | 12000      |
	 | Baker, Level: 2 (Standard)            | 3                     | 15               | 12               | 2018            | 27000      |
	 | Network engineer, Level: 4 (Standard) | 2                     | 24               | 12               | 2020            | 36000      |
	When the modelled apprenticeships page is displayed
	Then the column headings are displayed
	And each added apprenticeship is displayed in a separate row
	And the apprenticeship with the earliest start date is shown first
	And the other apprenticeships are in order of start date
	And the details against each apprenticeship match what was entered
