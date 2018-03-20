CREATE TABLE [dbo].[AccountPendingCompletionPayment]
(
	EmployerAccountId BIGINT NOT NULL,
	Amount DECIMAL(18,2) NOT NULL CONSTRAINT DF_Account__Amount DEFAULT (0),
	CreadedOn DATETIME NOT NULL CONSTRAINT DF_Account__CreatedOn DEFAULT (GETDATE()),
)