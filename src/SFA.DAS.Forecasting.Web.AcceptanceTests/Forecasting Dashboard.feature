Feature: Forecasting Dashboard
As a Levy Employer   
I want to be able to see my current levy balance, credit and apprenticeship commitments displayed as a forecast across multiple future periods. 
So that I can explore a variety of possible future scenarios and better plan my future levy spend and apprenticeships 

Background:	
	Given that I am an employer
	And I have logged into my Apprenticeship Account

Scenario: Navigate to Employer Forecasting Dashboard
	When I navigate to the Landing page of the Forecasting dashboard
	Then the dashboard should be displayed 