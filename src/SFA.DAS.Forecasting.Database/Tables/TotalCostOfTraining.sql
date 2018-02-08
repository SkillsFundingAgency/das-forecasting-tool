CREATE TABLE [dbo].[TotalCostOfTraining]
(
	[Id] BIGINT NOT NULL PRIMARY KEY Identity(1,1),
	[EmployerAccountId] BIGINT NOT NULL,
	[TotalCostOfTraining] DECIMAL NOT NULL,
	[PeriodYear] SMALLINT NOT NULL,
	[PeriodMonth] SMALLINT NOT NULL
)
