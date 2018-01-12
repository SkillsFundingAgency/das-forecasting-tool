GO
SET NOCOUNT ON
truncate table [Apprenticeship]
DECLARE @EmployerId int = 12345

DECLARE @StartDate DateTime = DateAdd(m,5,DateAdd(d,1, EOMonth(current_timestamp)))

IF NOT EXISTS (select * from Apprenticeship where EmployerAccountId=@EmployerId)
BEGIN
	INSERT INTO Apprenticeship(EmployerAccountId, [Name], DateOfBirth, TrainingName, TrainingLevel, TrainingProviderName, StartDate, MonthlyPayment, Instalments, CompletionPayment)
		VALUES (@EmployerId, 'John', '1985-12-17', 'Furniture Manufacturer', 4, 'Acme Ltd', '2017-01-01', 303.13, 18, 1.096)
	INSERT INTO Apprenticeship(EmployerAccountId, [Name], DateOfBirth, TrainingName, TrainingLevel, TrainingProviderName, StartDate, MonthlyPayment, Instalments, CompletionPayment)
		VALUES (@EmployerId, 'Abdul', '1986-03-28', 'Furniture Manufacturer', 4, 'Acme Ltd', '2017-01-01', 303.13, 18, 1.096)
END


-- select * from Apprenticeship