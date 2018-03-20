Feature: FundingProjectionPage _PendingCompletion
As a Levy Employer logged into my Apprenticeship Account
I want to be able to see any pending completion payments

Background:	
	Given that I am an employer
	And I have logged into my Apprenticeship Account

Scenario: AC1: Reading completion payment
	Given I have generated the following completion payments

	| Amount | CreatedOn  |
	| 11008  | 2018-10-01 |

	And I'm on the Funding projection page
	Then Pending completion payments should be £11,008