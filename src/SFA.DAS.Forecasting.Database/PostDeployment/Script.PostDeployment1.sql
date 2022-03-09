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
	  (2, N'Transfer'),
	  (3, N'CoInvestedSfa'),
	  (4, N'CoInvestedEmployer'),
      (5, N'FullyFundedSfa')
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


if not exists( select * from sys.indexes where name = N'IX_LevyDeclaration__EmployerAccountId_PayrollYear_PayrollMonth_PayrollDate' ) 
begin
	CREATE NONCLUSTERED INDEX [IX_LevyDeclaration__EmployerAccountId_PayrollYear_PayrollMonth_PayrollDate] ON [dbo].[LevyDeclaration] ([EmployerAccountId],[PayrollYear],[PayrollMonth],[PayrollDate]) WITH (ONLINE = ON)
end
Go

update Payment
set SendingEmployerAccountId = EmployerAccountId
where SendingEmployerAccountId = 0

update Commitment
set SendingEmployerAccountId = EmployerAccountId
where SendingEmployerAccountId = 0

