DECLARE @BalanceMonth DateTime = '01 Jan 2018'

IF NOT EXISTS (select * from Balance where EmployerAccountId=12345 and datediff(dd,BalanceMonth, @BalanceMonth) = 0)
	INSERT INTO Balance (EmployerAccountId, BalanceMonth) VALUES (12345, @BalanceMonth)


set @BalanceMonth = '01 Feb 2018'
IF NOT EXISTS (select * from Balance where EmployerAccountId=12345 and datediff(dd,BalanceMonth, @BalanceMonth) = 0)
	INSERT INTO Balance (EmployerAccountId, BalanceMonth) VALUES (12345, @BalanceMonth)
