CREATE TABLE [dbo].[PaymentAggregation]
(
	[Id] BIGINT NOT NULL PRIMARY KEY Identity(1,1),
    [EmployerAccountId] BIGINT NOT NULL,
	[CollectionPeriodYear] INT NOT NULL,
	[CollectionPeriodMonth] INT not null,
	[Amount] DECIMAL(18,5) NOT NULL
)
GO

CREATE NONCLUSTERED INDEX [IDX_EmployerAccountId] ON [dbo].[PaymentAggregation] ([EmployerAccountId]) INCLUDE ([CollectionPeriodYear],	[CollectionPeriodMonth],[Amount]) WITH (ONLINE = ON)
GO


