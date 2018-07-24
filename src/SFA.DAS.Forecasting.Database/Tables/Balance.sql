CREATE TABLE [dbo].[Balance]
(
	EmployerAccountId BIGINT NOT NULL CONSTRAINT PK_Balance PRIMARY KEY,
	Amount DECIMAL(18,5) NOT NULL CONSTRAINT DF_Balance__Amount DEFAULT (0),
	TransferAllowance DECIMAL (18,5) NOT NULL CONSTRAINT DF_Balance__TransferAllowance DEFAULT(0),
	RemainingTransferBalance DECIMAL (18,5) NOT NULL CONSTRAINT DF_Dalance__RemainingTransferBalance DEFAULT(0),
	BalancePeriod DATETIME NOT NULL,
	ReceivedDate DATETIME NOT NULL CONSTRAINT DF_Balance__ReceivedDate DEFAULT (GETDATE()),
	UnallocatedCompletionPayments DECIMAL (18,5) NOT NULL CONSTRAINT DF_Balance__UnallocatedCompletionPayments DEFAULT(0),
)