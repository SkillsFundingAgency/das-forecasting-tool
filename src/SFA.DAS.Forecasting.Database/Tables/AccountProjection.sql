﻿CREATE TABLE [dbo].[AccountProjection]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[EmployerAccountId] BIGINT NOT NULL,
    [ProjectionCreationDate] DATETIME NOT NULL,
    [ProjectionGenerationType] TINYINT NOT NULL,
    [Month] SMALLINT NOT NULL,
    [Year] INT NOT NULL,
    [FundsIn] DECIMAL(18,5) NOT NULL,
    [TotalCostOfTraining] DECIMAL(18,5) NOT NULL,
	[TransferOutTotalCostOfTraining] DECIMAL(18,5) NOT NULL default(0),
	[TransferInTotalCostOfTraining] DECIMAL(18,5) NOT NULL default(0),
	[TransferInCompletionPayments] DECIMAL(18,5) NOT NULL default(0),
    [CompletionPayments] DECIMAL(18,5) NOT NULL,
	[TransferOutCompletionPayments] DECIMAL(18,5) NOT NULL default(0),
    [FutureFunds] DECIMAL(18,5) NOT NULL,
	[CoInvestmentEmployer] DECIMAL(18,5) NOT NULL default(0),
	[CoInvestmentGovernment] DECIMAL(18,5) NOT NULL default(0), 
    [ExpiredFunds] DECIMAL(18, 5) NOT NULL DEFAULT (0), 
    [FutureFundsNoExpiry] DECIMAL(18, 5) NOT NULL DEFAULT 0,
    [ApprovedPledgeApplicationCost] DECIMAL(18, 5) NOT NULL DEFAULT 0,
    [AcceptedPledgeApplicationCost] DECIMAL(18, 5) NOT NULL DEFAULT 0,
    [PledgeOriginatedCommitmentCost] DECIMAL(18, 5) NOT NULL DEFAULT 0
)
GO

CREATE NONCLUSTERED INDEX [idx_account_projection] ON [dbo].[AccountProjection] ([EmployerAccountId]) 
	INCLUDE ([CoInvestmentEmployer], [CoInvestmentGovernment], [CompletionPayments], [FundsIn], [FutureFunds], [Month], [ProjectionCreationDate], [ProjectionGenerationType], [TotalCostOfTraining], [TransferInCompletionPayments], [TransferInTotalCostOfTraining], [TransferOutCompletionPayments], [TransferOutTotalCostOfTraining], [Year], [FutureFundsNoExpiry]) 
WITH (ONLINE = ON)
GO
CREATE NONCLUSTERED INDEX [idx_account_projection_account_date] ON [dbo].[AccountProjection] ([EmployerAccountId],[Month],[Year]) 
	INCLUDE ([CoInvestmentEmployer], [CoInvestmentGovernment], [CompletionPayments], [FundsIn], [FutureFunds], [ProjectionCreationDate],[ProjectionGenerationType], [TotalCostOfTraining], [TransferInCompletionPayments], [TransferInTotalCostOfTraining], [TransferOutCompletionPayments], [TransferOutTotalCostOfTraining], [FutureFundsNoExpiry]) 
WITH (ONLINE = ON)