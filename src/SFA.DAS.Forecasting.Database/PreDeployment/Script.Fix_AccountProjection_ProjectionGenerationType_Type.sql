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
--BEGIN TRANSACTION
--	BEGIN TRY

	if Exists (select * from sys.columns where object_id = OBJECT_ID('dbo.AccountProjection') and name = 'ProjectionGenerationType' and system_type_id = 52)
	begin 

		alter table AccountProjection add ProjectionGenerationType_tmp tinyint null;
		Exec ('update AccountProjection set ProjectionGenerationType_tmp = ProjectionGenerationType');
		alter table AccountProjection drop column ProjectionGenerationType;
		EXEC sp_rename 'dbo.AccountProjection.ProjectionGenerationType_tmp', 'ProjectionGenerationType', 'COLUMN';
		alter table AccountProjection alter column ProjectionGenerationType tinyint not null;

	end

 --   COMMIT
 --   END TRY
 --   BEGIN CATCH
 --       ROLLBACK
 --       --THROW; -- Only if you want reraise an exception (to determine the reason of the exception)
 --   END CATCH