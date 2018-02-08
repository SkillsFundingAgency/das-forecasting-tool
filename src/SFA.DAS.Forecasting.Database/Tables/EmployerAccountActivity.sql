CREATE TABLE [dbo].[EmployerAccount]
(
	[Id] BIGINT NOT NULL PRIMARY KEY,  
	[LevyDeclared] DECIMAL not null constraint DF_EmployerAccount__LevyDeclared default (0),
	[TotalCostOfTraining] DECIMAL not null constraint DF_EmployerAccount__TotalCostOfTraining default (0),
	[Balance] DECIMAL not null constraint DF_EmployerAccount__Balance default (0),
	[Period] DATETIME not null constraint DF_EmployerAccount__Period default (getdate())
)
