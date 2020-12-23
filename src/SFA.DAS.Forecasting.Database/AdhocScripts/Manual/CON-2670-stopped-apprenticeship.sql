/*
Script to populate Commitment data for FPT  from Commitments(Approvals) source data

Instructions for use:
1. Turn on SQL CMD mode and Results to Text and max characters in text output to 8192 (query->query options->results->text->Maximum number of characters displayed in each column)
2. Execute this script against Commitments database
3. Execute the resulting script against Forecasting database

Expectations:
~45 minutes on commitments database
~10 minutes on forecasting db
*/

SET NOCOUNT ON

declare @MAXINSERT int = 1000

--table var declarations
print 'declare @TempStoppedCommitments table ([ApprenticeshipId] bigint, [StopDate] date, [PaymentStatus] smallint)'

BEGIN TRY

	--commitments
	SELECT
	CASE (ROW_NUMBER() OVER (ORDER BY a.Id DESC) % @MAXINSERT) 
	WHEN 1 
	THEN 'insert into @TempStoppedCommitments ([ApprenticeshipId], [StopDate], [PaymentStatus]) values' + char(13) + char(10) else '' end +
	' (' + convert(varchar,[Id]) +', ' + '''' + convert(varchar,[StopDate],121) + ''''+', ' +'''' + convert(varchar,PaymentStatus) + '''' + ')' + 	
	CASE 
	WHEN ((ROW_NUMBER() OVER (ORDER BY a.Id DESC) % @MAXINSERT = 0)
	OR (ROW_NUMBER() OVER (ORDER BY a.Id DESC) = (select count(1) from dbo.Apprenticeship where PaymentStatus = 3 and a.StopDate is not null )))
	THEN ''
	ELSE ',' END
	FROM  [dbo].[Apprenticeship] a
	WHERE PaymentStatus = 3 and a.StopDate is not null	
	ORDER BY a.Id desc
	
	PRINT '	BEGIN TRANSACTION '

	PRINT '	

	UPDATE [dbo].[Commitment] 
	SET ActualEndDate = TC.StopDate , Status = 1
	FROM Commitment C 
	INNER JOIN
	@TempStoppedCommitments TC 
	ON C.ApprenticeshipId = TC.ApprenticeshipId AND TC.PaymentStatus = 3

	print ''updated '' + convert(varchar,@@ROWCOUNT) + '' Stopped Commitments''
	print ''Completed''

	COMMIT TRANSACTION
	'

END TRY
BEGIN CATCH
	SELECT  
		 ERROR_NUMBER() AS ErrorNumber  
        ,ERROR_SEVERITY() AS ErrorSeverity  
        ,ERROR_STATE() AS ErrorState  
        ,ERROR_PROCEDURE() AS ErrorProcedure  
        ,ERROR_LINE() AS ErrorLine  
        ,ERROR_MESSAGE() AS ErrorMessage;    
END CATCH
