Feature: LoadApprenticeships
	As an employer 
	I want all my not started and not paid commitments to be considered in my projection
	So that my projection is as complete as possible

Scenario: Load apprenticeships
    Given I have no existing commitments
	And there is following apprenticehsips in the commitments API
	| EmployerAccountId | TransferSenderId | Id | FirstName | StartDateString | EndDateString | Cost |
	| 12345             | 12346            | 1  | Abba      | in 1 years      | in 2 years    | 1500 |
	| 12345             |                  | 2  | Abba      | in 0 years      | in 2 years    | 48000 |
	When I trigger load of apprenticeships
	Then there should be following commitments stored
	| ApprenticeshipId | ApprenticeName | CompletionAmount | InstallmentAmount | NumberOfInstallments | CourseLevel | FundingSource |
	| 1                | Abba           | 300              | 100               | 12                   | 6           | Transfer      |
	| 2                | Abba           | 9600             | 1600              | 24                   | 6           | Levy          |