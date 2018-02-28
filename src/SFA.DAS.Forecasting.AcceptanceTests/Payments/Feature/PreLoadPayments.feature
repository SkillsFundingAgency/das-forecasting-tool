Feature: PreLoadPayments
	As a product owner 
	I want to pre populate the database for some employers
	So that I don't have to wait for the process to run.

@mytag
Scenario: Pre load payment events
	Given I trigger PreLoadPayment function some employers
	When data have been processed
	Then there will be payments for the employer and no sensitive data will have been stored in the database
	And there will be commitment for the employer and no sensitive data will have been stored in the database