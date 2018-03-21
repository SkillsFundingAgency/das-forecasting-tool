Feature: FundingProjectionPage _PendingCompletion
As a Levy Employer logged into my Apprenticeship Account
I want to be able to see any pending completion payments

Background:	
	Given that I am an employer
	And I have logged into my Apprenticeship Account

Scenario: AC1: Reading completion payment
	Given I have generated the following commitments
	
	| EmployerAccountId | LearnerId | ProviderId | ProviderName    | ApprenticeshipId | ApprenticeName    | CourseName                  | CourseLevel | StartDate  | PlannedEndDate | ActualEndDate | CompletionAmount | MonthlyInstallment | NumberOfInstallments |
	| 12345             | 1         | 2          | Test Provider 2 | 222              | Apprentice Name 1 | Test Provider	Test Course 2 | 1         | 2018-03-20 | 2020-03-20     | NULL          | 3000.00          | 500.00             | 24                   |
	| 12345             | 2         | 3          | Test Provider 3 | 333              | Apprentice Name 2 | Test Provider	Test Course 3 | 1         | 2016-03-21 | 2018-03-21     | NULL          | 2000.00          | 250.00             | 24                   |

	And I have generated the following projections
  
	| Date   | Funds in | Cost Of Training	   | Completion Payments | Your Contribution | Government Contribution | Future Funds |
	| Apr 18 | 14000    | 880                    | 32200               | 31000			 | 31000				   | 31000        |
	| May 18 | 15000    | 880                    | 32200               | 17000			 | 17000				   | 17000        |
	| Jun 18 | 91000    | 1800                   | 10000               | 23000			 | 23000				   | 23000        |

	And I'm on the Funding projection page
	Then Pending completion payments should be £2,000