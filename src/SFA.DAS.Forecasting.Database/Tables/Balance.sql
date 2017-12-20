CREATE TABLE [dbo].[Balance]
(
	Id BIGINT NOT NULL PRIMARY KEY  IDENTITY,
	EmployerAccountId BIGINT NOT NULL,
	BalanceMonth DateTime NOT NULL,
	CostOfTraining decimal NOT NULL DEFAULT 0,
	CompletionPayments decimal NOT NULL DEFAULT 0,
	ExpiredFunds decimal NOT NULL DEFAULT 0,
)
