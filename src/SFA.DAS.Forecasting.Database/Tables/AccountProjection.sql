﻿CREATE TABLE [dbo].[AccountProjection]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[EmployerAccountId] BIGINT NOT NULL,
    [ProjectionCreationDate] DATETIME NOT NULL,
    [ProjectionGenerationType] TINYINT NOT NULL,
    [Month] SMALLINT NOT NULL,
    [Year] INT NOT NULL,
    [FundsIn] DECIMAL(18,2) NOT NULL,
    [TotalCostOfTraining] DECIMAL(18,2) NOT NULL,
    [CompletionPayments] DECIMAL(18,2) NOT NULL,
    [FutureFunds] DECIMAL(18,2) NOT NULL,
	[CoInvestmentEmployer] DECIMAL(18,2) NOT NULL default(0),
	[CoInvestmentGovernment] DECIMAL(18,2) NOT NULL default(0)
)
