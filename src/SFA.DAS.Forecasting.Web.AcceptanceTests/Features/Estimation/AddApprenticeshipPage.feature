Feature: AddApprenticeshipPage
	As an Employer
	I want to be abler to make sure that Add Apprenticeships Page works properly

@AddApprenticeshipPage
Scenario: AC1: Blank apprenticeship
	Given that I'm on the Add Apprenticeship page
	When I do not select an apprenticeship
	And I select 'Check if I can fund these'
	Then the 'You must choose 1 apprenticeship' error is displayed in line and at the top of the page

@AddApprenticeshipPage
Scenario: AC2: Blank number of apprentices
	Given that I'm on the Add Apprenticeship page
	When I do not enter the number of apprenticeship
	And I select 'Check if I can fund these' 
	Then the 'Make sure you have at least 1 or more apprentices' error is displayed in line and at the top of the page

@AddApprenticeshipPage
Scenario: AC3: Enter start date in past
	Given that I'm on the Add Apprenticeship page
	When I enter a start date before the current month
	And I select 'Check if I can fund these' 
	Then the 'The start date must be within the next 4 years' error is displayed in line and at the top of the page

@AddApprenticeshipPage
Scenario: AC4: Start date no month
	Given that I'm on the Add Apprenticeship page
	When I do not enter a start date month
	And I select 'Check if I can fund these' 
	Then the 'The start month was not entered' error is displayed in line and at the top of the page

@AddApprenticeshipPage
Scenario: AC5: Start date no year
	Given that I'm on the Add Apprenticeship page
	When I do not enter a start date year
	And I select 'Check if I can fund these' 
	Then the 'The start year was not entered' error is displayed in line and at the top of the page

@AddApprenticeshipPage
Scenario: AC6: No total cost
	Given that I'm on the Add Apprenticeship page
	When I do not enter a total cost
	And I select 'Check if I can fund these' 
	Then the 'The total training cost was not entered' error is displayed in line and at the top of the page

@AddApprenticeshipPage
Scenario: AC7: Add Standard Apprenticeship
	Given that I'm on the Add Apprenticeship page
	When i choose a standard apprenticeship
	And I select 'Check if I can fund these' 
	Then the apprenticeship is added

@AddApprenticeshipPage
Scenario: AC8: Add Framework Apprenticeship
	Given that I'm on the Add Apprenticeship page
	When i choose a Framework apprenticeship
	And I select 'Check if I can fund these' 
	Then the apprenticeship is added
