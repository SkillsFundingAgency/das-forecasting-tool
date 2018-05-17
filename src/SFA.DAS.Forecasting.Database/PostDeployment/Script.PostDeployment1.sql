/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

--Seed data for Funding Source Table
MERGE INTO FundingSource AS Target 
USING (VALUES 
	  (1, N'Levy'), 
	  (2, N'Transfer')
	) 
AS Source (Id, Name) 
ON Target.Id = Source.Id 
-- update matched rows 
WHEN MATCHED THEN 
	UPDATE SET [Name] = Source.Name 
--insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
	INSERT (Id, [Name]) 
	VALUES (Id, [Name]) 
--delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
	DELETE;
GO

if not exists( select * from sys.indexes where name = N'IX_Commitment__EmployerAccountId_AppreticeshipId_ActualEndDate' ) 
begin
	CREATE NONCLUSTERED INDEX [IX_Commitment__EmployerAccountId_AppreticeshipId_ActualEndDate] ON [dbo].[Commitment] ([EmployerAccountId], [ApprenticeshipId], [ActualEndDate]) WITH (ONLINE = ON)
end
Go

if not exists( select * from sys.indexes where name = N'IX_Payment__EmployerAccountId_AppreticeshipId' ) 
begin
	CREATE NONCLUSTERED INDEX [IX_Payment__EmployerAccountId_AppreticeshipId] ON [dbo].[Payment] ([EmployerAccountId], [ApprenticeshipId]) WITH (ONLINE = ON)
end
Go

if not exists( select * from sys.indexes where name = N'IX_AccountProjection__EmployerAccountId' ) 
begin
	CREATE NONCLUSTERED INDEX [IX_AccountProjection__EmployerAccountId] ON [dbo].[AccountProjection] ([EmployerAccountId]) WITH (ONLINE = ON)
end
Go

if not exists( select * from sys.indexes where name = N'IX_AccountProjectionCommitment__AccountProjectionId' ) 
begin
	CREATE NONCLUSTERED INDEX [IX_AccountProjectionCommitment__AccountProjectionId] ON [dbo].[AccountProjectionCommitment] ([AccountProjectionId]) WITH (ONLINE = ON)
end
Go

if not exists( select * from sys.indexes where name = N'IX_LevyDeclaration__EmployerAccountId_PayrollYear_PayrollMonth_PayrollDate' ) 
begin
	CREATE NONCLUSTERED INDEX [IX_LevyDeclaration__EmployerAccountId_PayrollYear_PayrollMonth_PayrollDate] ON [dbo].[LevyDeclaration] ([EmployerAccountId],[PayrollYear],[PayrollMonth],[PayrollDate]) WITH (ONLINE = ON)
end
Go

