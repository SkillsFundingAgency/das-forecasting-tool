CREATE TABLE [dbo].[CurrentBalanceAndLevy]
(
	[Id] INT NOT NULL PRIMARY KEY,
	EmployerAccountId BIGINT NOT NULL,
	LevyCredit DateTime NOT NULL,
	Balance decimal NOT NULL
)
