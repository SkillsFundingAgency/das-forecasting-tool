/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/


IF EXISTS(select 1 from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'Balance' and COLUMN_NAME = 'Amount' and NUMERIC_SCALE = 2)
	BEGIN
		ALTER TABLE [dbo].[Balance] ALTER COLUMN Amount decimal(18,5) NOT NULL   
	END
IF EXISTS(select 1 from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'Balance' and COLUMN_NAME = 'RemainingTransferBalance' and NUMERIC_SCALE = 2)
BEGIN
	ALTER TABLE [dbo].[Balance] ALTER COLUMN RemainingTransferBalance decimal(18,5) NOT NULL
END
IF EXISTS(select 1 from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'Balance' and COLUMN_NAME = 'TransferAllowance' and NUMERIC_SCALE = 2)
BEGIN
	ALTER TABLE [dbo].[Balance] ALTER COLUMN TransferAllowance decimal(18,5) NOT NULL
END
IF EXISTS(select 1 from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'Balance' and COLUMN_NAME = 'UnallocatedCompletionPayments' and NUMERIC_SCALE = 2)
BEGIN
	ALTER TABLE [dbo].[Balance] ALTER COLUMN UnallocatedCompletionPayments decimal(18,5) NOT NULL
END

-- AccountProjection
IF EXISTS(SELECT * FROM sys.indexes WHERE name='idx_account_projection' AND object_id = OBJECT_ID('[dbo].[AccountProjection]'))
BEGIN
	drop index idx_account_projection ON [dbo].[AccountProjection]
END
IF EXISTS(SELECT * FROM sys.indexes WHERE name='IDX_Payment_EmployerAccountId' AND object_id = OBJECT_ID('[dbo].[AccountProjection]'))
BEGIN
	drop index IDX_Payment_EmployerAccountId ON [dbo].[AccountProjection]
END

IF EXISTS(select 1 from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'AccountProjection' and COLUMN_NAME = 'FundsIn' and NUMERIC_SCALE = 2)
BEGIN
	ALTER TABLE [dbo].[AccountProjection] ALTER COLUMN FundsIn decimal(18,5) NOT NULL
END
IF EXISTS(select 1 from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'AccountProjection' and COLUMN_NAME = 'TotalCostOfTraining' and NUMERIC_SCALE = 2)
BEGIN
	ALTER TABLE [dbo].[AccountProjection] ALTER COLUMN TotalCostOfTraining decimal(18,5) NOT NULL
END
IF EXISTS(select 1 from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'AccountProjection' and COLUMN_NAME = 'TransferOutTotalCostOfTraining' and NUMERIC_SCALE = 2)
BEGIN
	ALTER TABLE [dbo].[AccountProjection] ALTER COLUMN TransferOutTotalCostOfTraining decimal(18,5) NOT NULL
END
IF EXISTS(select 1 from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'AccountProjection' and COLUMN_NAME = 'TransferInTotalCostOfTraining' and NUMERIC_SCALE = 2)
BEGIN
	ALTER TABLE [dbo].[AccountProjection] ALTER COLUMN TransferInTotalCostOfTraining decimal(18,5) NOT NULL
END

IF EXISTS(select 1 from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'AccountProjection' and COLUMN_NAME = 'TransferInCompletionPayments' and NUMERIC_SCALE = 2)
BEGIN
	ALTER TABLE [dbo].[AccountProjection] ALTER COLUMN TransferInCompletionPayments decimal(18,5) NOT NULL
END
IF EXISTS(select 1 from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'AccountProjection' and COLUMN_NAME = 'CompletionPayments' and NUMERIC_SCALE = 2)
BEGIN
	ALTER TABLE [dbo].[AccountProjection] ALTER COLUMN CompletionPayments decimal(18,5) NOT NULL
END
IF EXISTS(select 1 from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'AccountProjection' and COLUMN_NAME = 'TransferOutCompletionPayments' and NUMERIC_SCALE = 2)
BEGIN
	ALTER TABLE [dbo].[AccountProjection] ALTER COLUMN TransferOutCompletionPayments decimal(18,5) NOT NULL
END
IF EXISTS(select 1 from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'AccountProjection' and COLUMN_NAME = 'FutureFunds' and NUMERIC_SCALE = 2)
BEGIN
	ALTER TABLE [dbo].[AccountProjection] ALTER COLUMN FutureFunds decimal(18,5) NOT NULL
END

IF EXISTS(select 1 from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'AccountProjection' and COLUMN_NAME = 'CoInvestmentEmployer' and NUMERIC_SCALE = 2)
BEGIN
	ALTER TABLE [dbo].[AccountProjection] ALTER COLUMN CoInvestmentEmployer decimal(18,5) NOT NULL
END
IF EXISTS(select 1 from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'AccountProjection' and COLUMN_NAME = 'CoInvestmentGovernment' and NUMERIC_SCALE = 2)
BEGIN
	ALTER TABLE [dbo].[AccountProjection] ALTER COLUMN CoInvestmentGovernment decimal(18,5) NOT NULL
END


-- LevyDeclaration
IF EXISTS(select 1 from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'LevyDeclaration' and COLUMN_NAME = 'LevyAmountDeclared' and NUMERIC_SCALE = 2)
BEGIN
	ALTER TABLE [dbo].[LevyDeclaration] ALTER COLUMN LevyAmountDeclared decimal(18,5) NOT NULL
END

-- Payment
IF EXISTS(SELECT * FROM sys.indexes WHERE name='IDX_Payment_EmployerAccountId' AND object_id = OBJECT_ID('[dbo].[AccountProjection]'))
BEGIN
	drop index IDX_Payment_EmployerAccountId ON [dbo].[Payment]
END
IF EXISTS(select 1 from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'Payment' and COLUMN_NAME = 'Amount' and NUMERIC_SCALE = 2)
BEGIN
	ALTER TABLE [dbo].[Payment] ALTER COLUMN Amount decimal(18,5) NOT NULL
END

-- Commitment
IF EXISTS(SELECT * FROM sys.indexes WHERE name='idx_commitment_employerAccountId' AND object_id = OBJECT_ID('[dbo].[Commitment]'))
BEGIN
	drop index idx_commitment_employerAccountId ON [dbo].[Commitment]
END
IF EXISTS(SELECT * FROM sys.indexes WHERE name='idx_commitment_employerAccountId' AND object_id = OBJECT_ID('[dbo].[Commitment]'))
BEGIN
	drop index idx_commitment_employerAccountId ON [dbo].[Commitment]
END
IF EXISTS(SELECT * FROM sys.indexes WHERE name='idx_commitment_sendingEmployerAccountId' AND object_id = OBJECT_ID('[dbo].[Commitment]'))
BEGIN
	drop index idx_commitment_sendingEmployerAccountId ON [dbo].[Commitment]
END
IF EXISTS(SELECT * FROM sys.indexes WHERE name='idx_commitment_actualendate' AND object_id = OBJECT_ID('[dbo].[Commitment]'))
BEGIN
	drop index idx_commitment_actualendate ON [dbo].[Commitment]
END
IF EXISTS(SELECT * FROM sys.indexes WHERE name='idx_commitment_employerAccountId_sending_endDate' AND object_id = OBJECT_ID('[dbo].[Commitment]'))
BEGIN
drop index idx_commitment_employerAccountId_sending_endDate ON [dbo].[Commitment]
END
IF EXISTS(SELECT * FROM sys.indexes WHERE name='idx_commitment_employerAccountId_fundingsource_endDate' AND object_id = OBJECT_ID('[dbo].[Commitment]'))
BEGIN
drop index idx_commitment_employerAccountId_fundingsource_endDate ON [dbo].[Commitment]
END
IF EXISTS(SELECT * FROM sys.indexes WHERE name='idx_commitment_sendingemployerAccountId_fundingsource_endDate' AND object_id = OBJECT_ID('[dbo].[Commitment]'))
BEGIN
	drop index idx_commitment_sendingemployerAccountId_fundingsource_endDate ON [dbo].[Commitment]
END

IF EXISTS(select 1 from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'Commitment' and COLUMN_NAME = 'CompletionAmount' and NUMERIC_SCALE = 2)
BEGIN
	ALTER TABLE [dbo].[Commitment] ALTER COLUMN CompletionAmount decimal(18,5) NOT NULL
END
IF EXISTS(select 1 from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'Commitment' and COLUMN_NAME = 'MonthlyInstallment' and NUMERIC_SCALE = 2)
BEGIN
	ALTER TABLE [dbo].[Commitment] ALTER COLUMN MonthlyInstallment decimal(18,5) NOT NULL
END