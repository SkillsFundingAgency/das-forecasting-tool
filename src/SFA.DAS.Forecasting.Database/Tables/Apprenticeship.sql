CREATE TABLE [dbo].[Apprenticeship]
(
	Id INT NOT NULL PRIMARY KEY,
	EmployerAccountId BIGINT NOT NULL,
	FirstName NVARCHAR(100) NULL, 
    LastName NVARCHAR(100) NULL,
	StartDate DATETIME NOT NULL,
	MonthlyPayment decimal NOT NULL,
	TotalInstallments smallint NOT NULL,
	CompletionPayment decimal NOT NULL
)
