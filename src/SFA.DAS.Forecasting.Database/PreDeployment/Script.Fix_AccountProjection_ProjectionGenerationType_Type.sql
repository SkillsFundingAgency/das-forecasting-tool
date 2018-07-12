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

ALTER TABLE [dbo].[Balance] ALTER COLUMN Amount decimal(18,5) NOT NULL
ALTER TABLE [dbo].[Balance] ALTER COLUMN RemainingTransferBalance  decimal(18,5) NOT NULL
ALTER TABLE [dbo].[Balance] ALTER COLUMN TransferAllowance  decimal(18,5) NOT NULL
ALTER TABLE [dbo].[Balance] ALTER COLUMN UnallocatedCompletionPayments decimal(18,5) NOT NULL

ALTER TABLE [dbo].[Balance] ALTER COLUMN Amount decimal(18,5) NOT NULL
ALTER TABLE [dbo].[Balance] ALTER COLUMN RemainingTransferBalance  decimal(18,5) NOT NULL
ALTER TABLE [dbo].[Balance] ALTER COLUMN TransferAllowance  decimal(18,5) NOT NULL
ALTER TABLE [dbo].[Balance] ALTER COLUMN UnallocatedCompletionPayments decimal(18,5) NOT NULL


ALTER TABLE [dbo].[AccountProjection] ALTER COLUMN FundsIn decimal(18,5) NOT NULL
ALTER TABLE [dbo].[AccountProjection] ALTER COLUMN TotalCostOfTraining decimal(18,5) NOT NULL
ALTER TABLE [dbo].[AccountProjection] ALTER COLUMN TransferOutTotalCostOfTraining decimal(18,5) NOT NULL
ALTER TABLE [dbo].[AccountProjection] ALTER COLUMN TransferInTotalCostOfTraining decimal(18,5) NOT NULL
ALTER TABLE [dbo].[AccountProjection] ALTER COLUMN TransferInCompletionPayments decimal(18,5) NOT NULL
ALTER TABLE [dbo].[AccountProjection] ALTER COLUMN CompletionPayments decimal(18,5) NOT NULL
ALTER TABLE [dbo].[AccountProjection] ALTER COLUMN TransferOutCompletionPayments decimal(18,5) NOT NULL
ALTER TABLE [dbo].[AccountProjection] ALTER COLUMN FutureFunds decimal(18,5) NOT NULL
ALTER TABLE [dbo].[AccountProjection] ALTER COLUMN CoInvestmentEmployer decimal(18,5) NOT NULL
ALTER TABLE [dbo].[AccountProjection] ALTER COLUMN CoInvestmentGovernment decimal(18,5) NOT NULL



ALTER TABLE [dbo].[LevyDeclaration] ALTER COLUMN LevyAmountDeclared decimal(18,5) NOT NULL

ALTER TABLE [dbo].[Payment] ALTER COLUMN Amount decimal(18,5) NOT NULL


ALTER TABLE [dbo].[Commitment] ALTER COLUMN CompletionAmount decimal(18,5) NOT NULL
ALTER TABLE [dbo].[Commitment] ALTER COLUMN MonthlyInstallment decimal(18,5) NOT NULL