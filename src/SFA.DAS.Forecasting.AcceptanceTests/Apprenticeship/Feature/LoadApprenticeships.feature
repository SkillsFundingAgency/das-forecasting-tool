Feature: LoadApprenticeships
	As an employer 
	I want all my not started and not paid commitments to be considered in my projection
	So that my projection is as complete as possible

Scenario: Load apprenticeships
    Given I have no existing commitments
	And there is following apprenticehsips in the commitments API
	| EmployerAccountId | TransferSenderId | Id | FirstName | StartDateString | EndDateString | Cost |
	| 12345             | 12346            | 1  | Abba      | 2 years ago     | 1 year ago    | 1000 |
	| 12345             |                  | 2  | Abba      | 3 years ago     | 1 year ago    | 1000 |
	When I trigger load of apprenticeships
	Then there should be following commitments stored
	| ApprenticeshipId | ApprenticeName | CompletionAmount | InstallmentAmount | NumberOfInstallments | CourseLevel | FundingSource |
	| 1                | Abba           | 200              | 66.66667          | 12                   | 6           | Transfer      |
	| 2                | Abba           | 200              | 33.33333          | 24                   | 6           | Levy          |