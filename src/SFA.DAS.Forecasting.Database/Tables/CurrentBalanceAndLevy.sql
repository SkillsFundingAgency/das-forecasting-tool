CREATE TABLE [dbo].[CurrentBalanceAndLevy]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY,
	EmployerAccountId BIGINT NOT NULL,
	LevyCredit DateTime NOT NULL,
	Balance decimal NOT NULL
)
