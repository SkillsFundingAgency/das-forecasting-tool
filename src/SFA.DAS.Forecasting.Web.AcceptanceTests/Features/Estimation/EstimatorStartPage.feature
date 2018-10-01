Feature: EstimatorStartPage
	As an employer
	I want the solution to know if I already have modelled apprenticeships
	So that it routes me to the most relevant page

Scenario: AC1: no current modelled apprenticeships
	Given that I'm on the estimator start page
	When I have no current modelled apprenticeships
	Then by clicking the Start button I am taken to the Add apprenticeship page

Scenario: AC2: current modelled apprenticeships
	Given that I'm on the estimator start page
	When I have current modelled apprenticeships
	Then by clicking the Start button I am taken to the Estimated costs page
	And the Remaining transfers allowance tab is displayed
	And the previously modelled apprenticeships costs are displayed