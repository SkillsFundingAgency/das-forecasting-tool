CREATE TABLE [dbo].[Balance]
(
	Id INT NOT NULL PRIMARY KEY,
	EmployerAccountId BIGINT NOT NULL,
	BalanceMonth DateTime NOT NULL,
	CostOfTraining decimal NOT NULL DEFAULT 0,
	CompletionPayments decimal NOT NULL DEFAULT 0,
	ExpiredFunds decimal NOT NULL DEFAULT 0,
)
