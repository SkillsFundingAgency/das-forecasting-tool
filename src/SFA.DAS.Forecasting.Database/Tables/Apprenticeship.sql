CREATE TABLE [dbo].[Apprenticeship]
(
	Id BIGINT NOT NULL PRIMARY KEY IDENTITY,
	EmployerAccountId BIGINT NOT NULL,
	[Name] NVARCHAR(200) NULL, 
	DateOfBirth DATETIME NOT NULL,
	TrainingName NVARCHAR(200) NOT NULL,
	TrainingLevel INT NOT NULL,
	TrainingProviderName NVARCHAR(200) NOT NULL,
	StartDate DATETIME NOT NULL,
	MonthlyPayment decimal NOT NULL,
	Instalments smallint NOT NULL,
	CompletionPayment decimal NOT NULL
)
