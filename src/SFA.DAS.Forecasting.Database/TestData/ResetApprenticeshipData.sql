GO
SET NOCOUNT ON
truncate table [Apprenticeship]
DECLARE @EmployerId int = 12345

DECLARE @StartDate DateTime = DateAdd(m,5,DateAdd(d,1, EOMonth(current_timestamp)))

IF NOT EXISTS (select * from Apprenticeship where EmployerAccountId=@EmployerId)
BEGIN
	INSERT INTO Apprenticeship (EmployerAccountId,FirstName, LastName ,StartDate,MonthlyPayment ,TotalInstallments, CompletionPayment) 
		VALUES (@EmployerId, 'Peter' ,'Parker', @StartDate,313.40,18, 313.40)
	INSERT INTO Apprenticeship (EmployerAccountId,FirstName, LastName ,StartDate,MonthlyPayment ,TotalInstallments, CompletionPayment) 
		VALUES (@EmployerId, 'Cletus' ,'Kasady', @StartDate,313.40,18, 313.40)
	INSERT INTO Apprenticeship (EmployerAccountId,FirstName, LastName ,StartDate,MonthlyPayment ,TotalInstallments, CompletionPayment) 
		VALUES (@EmployerId, 'Edward' ,'Brock', @StartDate,313.40,18, 313.40)
	INSERT INTO Apprenticeship (EmployerAccountId,FirstName, LastName ,StartDate,MonthlyPayment ,TotalInstallments, CompletionPayment) 
		VALUES (@EmployerId, 'Miles' ,'Morales', @StartDate,313.40,18, 313.40)
END


-- select * from Apprenticeship