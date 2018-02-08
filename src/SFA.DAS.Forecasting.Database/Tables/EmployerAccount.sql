CREATE TABLE [dbo].[EmployerAccount]
(
	[EmployerAccountId] BIGINT NOT NULL PRIMARY KEY,  
	[LevyDeclared] DECIMAL not null constraint DF_EmployerAccount__LevyDeclared default (0),
	[LevyPeriod] DATETIME null,
	[Balance] DECIMAL not null constraint DF_EmployerAccount__Balance default (0),
	[BalancePeriod] DATETIME null 
)
