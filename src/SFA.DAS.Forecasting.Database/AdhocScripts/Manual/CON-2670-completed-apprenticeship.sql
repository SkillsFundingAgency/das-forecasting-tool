/*
Script to populate Commitment data for FPT  from Commitments(Approvals) source data

Instructions for use:
1. Turn on SQL CMD mode and Results to Text and max characters in text output to 8192 (query->query options->results->text->Maximum number of characters displayed in each column)
2. Execute this script against Commitments database
3. Execute the resulting script against Forecasting database

Expectations:
~? minutes on commitments database
~? minutes on forecasting db
*/

SET NOCOUNT ON

declare @MAXINSERT int = 1000

--table var declarations
print 'declare @TempCompletedCommitments table ([ApprenticeshipId] bigint, [CompletionDate] datetime, [PaymentStatus] smallint)'

BEGIN TRY

	--commitments
	SELECT
	CASE (ROW_NUMBER() OVER (ORDER BY a.Id) % @MAXINSERT) 
	WHEN 1 
	THEN 'insert into @TempCompletedCommitments ([ApprenticeshipId], [CompletionDate], [PaymentStatus]) values' + char(13) + char(10) else '' end +
	' (' + convert(varchar,[Id]) +', ' + '''' + convert(varchar,[CompletionDate],121) + ''''+', ' +'''' + convert(varchar,PaymentStatus) + '''' + ')' 	+ 	
	CASE 
	WHEN ((ROW_NUMBER() OVER (ORDER BY a.Id) % @MAXINSERT = 0)
	OR (ROW_NUMBER() OVER (ORDER BY a.Id) = (select count(1) from dbo.Apprenticeship where PaymentStatus = 4 and a.CompletionDate is not null )))
	THEN ''
	ELSE ',' END
	FROM  [dbo].[Apprenticeship] a
	WHERE PaymentStatus = 4 and a.CompletionDate is not null
	
	PRINT '	BEGIN TRANSACTION '

	PRINT '	

	UPDATE [dbo].[Commitment] 
	SET ActualEndDate = TC.CompletionDate , Status = 2
	FROM Commitment C 
	INNER JOIN
	@TempCompletedCommitments TC 
	ON C.ApprenticeshipId = TC.ApprenticeshipId AND TC.PaymentStatus = 4

	print ''updated '' + convert(varchar,@@ROWCOUNT) + '' Completed Commitments''
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
