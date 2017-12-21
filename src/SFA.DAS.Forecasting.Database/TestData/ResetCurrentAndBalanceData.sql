GO
SET NOCOUNT ON

truncate table CurrentBalanceAndLevy
DECLARE @EmployerId int = 12345
IF NOT EXISTS (select * from CurrentBalanceAndLevy where EmployerAccountId = @EmployerId)
	Insert into CurrentBalanceAndLevy (EmployerAccountId, LevyCredit, Balance) VALUES (@EmployerId, 7000.00, 47000.00)


-- select * from CurrentBalanceAndLevy