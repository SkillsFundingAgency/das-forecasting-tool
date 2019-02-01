CREATE TABLE [dbo].[LevyDeclaration]
(
	Id BIGINT NOT NULL PRIMARY KEY IDENTITY,	
	EmployerAccountId BIGINT NOT NULL,
	Scheme NVARCHAR(50) NOT NULL,
	PayrollYear NVARCHAR(10) NOT NULL, 
	PayrollMonth TINYINT NOT NULL,	
	PayrollDate DATETIME NOT NULL,
	LevyAmountDeclared DECIMAL(18,5) NOT NULL,
	TransactionDate DATETIME NOT NULL,
	DateReceived DATETIME NOT NULL constraint DF_LevyDeclaration__DateReceived default(getdate())
)

GO
CREATE NONCLUSTERED INDEX [idx_commitment_employerAccountId] ON [dbo].[LevyDeclaration] ([EmployerAccountId]) INCLUDE ([scheme], [PayrollYear], [PayrollMonth], [PayrollDate], [LevyAmountDeclared], [TransactionDate], [DateReceived]) WITH (ONLINE = ON)
GO