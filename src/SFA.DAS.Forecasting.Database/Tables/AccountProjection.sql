CREATE TABLE [dbo].[AccountProjection]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[EmployerAccountId] BIGINT,
    [ProjectionCreationDate] DateTime,
    [ProjectionGenerationType] SMALLINT,
    [Month] SMALLINT,
    [Year] INT,
    [FundsIn] DECIMAL,
    [TotalCostOfTraning] DECIMAL,
    [CompletionPayments] DECIMAL,
    [FutureFunds] DECIMAL
)
