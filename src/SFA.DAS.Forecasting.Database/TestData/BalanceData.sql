DECLARE @EmployerId int = 12345
DECLARE @BalanceMonth DateTime = '01 Jan 2018'

IF NOT EXISTS (select * from Balance where EmployerAccountId=@EmployerId and datediff(dd,BalanceMonth, @BalanceMonth) = 0)
	INSERT INTO Balance (EmployerAccountId, BalanceMonth) VALUES (@EmployerId, @BalanceMonth)


set @BalanceMonth = '01 Feb 2018'
IF NOT EXISTS (select * from Balance where EmployerAccountId=@EmployerId and datediff(dd,BalanceMonth, @BalanceMonth) = 0)
	INSERT INTO Balance (EmployerAccountId, BalanceMonth) VALUES (@EmployerId, @BalanceMonth)



IF NOT EXISTS (select * from CurrentBalanceAndLevy where EmployerAccountId = @EmployerId)
	Insert into CurrentBalanceAndLevy (EmployerAccountId, LevyCredit, Balance) VALUES (@EmployerId, 7000.00, 47000.00)


		--select * from balance
		--select * from CurrentBalanceAndLevy


		--truncate table balance
		--truncate table CurrentBalanceAndLevy