Feature: RedirectToLoginPage
	In order to avoid people seeing private data
	As a admin
	I want the user to be logged in
	and only able to see his own data

Background: 
	Given that I am an employer

Scenario: AC1: User should be redirected to log in page if he is not logged in
	Given I am not logged into my Apprenticeship Account
	When the user tries to access his own forecasting data
	Then he should be redirected to the log in page

Scenario: AC2: User should not be redirected to log in page if he is logged in
	Given I have logged into my Apprenticeship Account
	When the user tries to access his own forecasting data
	Then he should not be redirected to the log in page