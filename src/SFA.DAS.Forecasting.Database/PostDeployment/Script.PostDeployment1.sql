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
	  (3, 'CoInvestedSfa')
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



DECLARE @paymentAcc as TABLE (
id BIGINT identity (1,1) not null,
employeraccountid BIGINT  not null
)

insert into @paymentAcc
select distinct employerAccountid from Payment

DECLARE @rowCount AS BIGINT 
select @rowCount = Count(1) from @paymentAcc

DECLARE @count as bigint = 1
WHILE(@count <= @rowCount)
BEGIN
	DECLARE @employerAccountid as BIGINT
	SELECT @employerAccountid = employerAccountid from @paymentAcc where id = @count

	insert into PaymentAggregation (EmployerAccountId,CollectionPeriodMonth,CollectionPeriodYear,Amount)
	SELECT 
			@employerAccountid as EmployerAccountId,
			[Extent1].[CollectionPeriodMonth] AS CollectionPeriodMonth, 
			[Extent1].[CollectionPeriodYear] AS CollectionPeriodYear, 
			SUM([Extent1].[Amount]) AS Amount
			FROM [dbo].[Payment] AS [Extent1]
			WHERE ((1 = [Extent1].[FundingSource]) AND ([Extent1].[EmployerAccountId] = @employerAccountid)) OR ((2 = [Extent1].[FundingSource]) AND ([Extent1].[SendingEmployerAccountId] = @employerAccountid))
			GROUP BY [Extent1].[CollectionPeriodMonth], [Extent1].[CollectionPeriodYear]
		

	SET @count = @count + 1
END