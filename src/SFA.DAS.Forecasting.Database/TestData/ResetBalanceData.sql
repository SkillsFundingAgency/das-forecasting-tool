GO
SET NOCOUNT ON
truncate table balance

DECLARE @EmployerId int = 12345
DECLARE @BalanceMonth DateTime = DateAdd(m,-1,DateAdd(d,1, EOMonth(current_timestamp)))


DECLARE @NoOfMonths int = 1

while (@NoOfMonths <= 48)
BEGIN
	IF NOT EXISTS (select * from Balance where EmployerAccountId=@EmployerId and datediff(dd,BalanceMonth, dateadd(mm,@NoOfMonths,@BalanceMonth)) = 0)
		INSERT INTO Balance (EmployerAccountId, BalanceMonth) VALUES (@EmployerId, dateadd(mm,@NoOfMonths,@BalanceMonth))
	set @NoOfMonths = @NoOfMonths+1
END


-- NOTE: you can pick a month and Employer to alter the CostOfTraining, CompletionPayments, ExpiredFunds as suits

--eg to update CostOfTraining for 5th month of this set use
/*
UPDATE Balance SET CostOfTraining = 10,
				   CompletionPayments = 0,
				   ExpiredFunds = 0
				   WHERE EmployerAccountId = @EmployerId AND BalanceMonth = DateAdd(mm,5,@BalanceMonth)
*/



-- select * from balance
	
