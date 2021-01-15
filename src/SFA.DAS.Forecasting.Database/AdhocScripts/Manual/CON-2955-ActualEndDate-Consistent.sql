BEGIN TRANSACTION [Tran1]

  BEGIN TRY	
	UPDATE [dbo].[Commitment] SET ActualEndDate = null WHERE (status = 0 or status is null) and (ActualEndDate is not null)
	COMMIT TRANSACTION [Tran1]
  END TRY

  BEGIN CATCH
      ROLLBACK TRANSACTION [Tran1]
	  SELECT   ERROR_NUMBER() AS ErrorNumber
        ,ERROR_SEVERITY() AS ErrorSeverity  
        ,ERROR_STATE() AS ErrorState  
        ,ERROR_PROCEDURE() AS ErrorProcedure  
        ,ERROR_LINE() AS ErrorLine  
        ,ERROR_MESSAGE() AS ErrorMessage;  
  END CATCH