CREATE TABLE [dbo].[Commitment]
(
	[Id] BIGINT NOT NULL PRIMARY KEY Identity(1,1),
	[EmployerAccountId] BIGINT NOT NULL,
	[SendingEmployerAccountId] BIGINT NOT NULL Constraint DF_Commitment__SendingEmployerAccountId default(0),
	[LearnerId] BIGINT NOT NULL,
	[ProviderId] BIGINT NOT NULL,
	[ProviderName] NVARCHAR(200) NOT NULL,	
	[ApprenticeshipId] BIGINT NOT NULL,
    [ApprenticeName] NVARCHAR(200) NOT NULL,
    [CourseName] NVARCHAR(200)  NOT NULL,
    [CourseLevel] INT NULL,
	[StartDate] DATETIME NOT NULL,
    [PlannedEndDate] DATETIME NOT NULL,
    [ActualEndDate] DATETIME NULL,
    [CompletionAmount] DECIMAL(18,2) NOT NULL,
    [MonthlyInstallment] DECIMAL(18,2) NOT NULL,
    [NumberOfInstallments] SMALLINT NOT NULL,
	[FundingSource] TINYINT NOT NULL CONSTRAINT FK_Commitment__FundingSource FOREIGN KEY REFERENCES FundingSource(Id) default(1)
)

GO
CREATE NONCLUSTERED INDEX [idx_commitment_employerAccountId] ON [dbo].[Commitment] ([EmployerAccountId]) INCLUDE ([ApprenticeName], [ApprenticeshipId], [CompletionAmount], [CourseLevel], [CourseName], [ActualEndDate], [FundingSource], [LearnerId], [MonthlyInstallment], [NumberOfInstallments], [PlannedEndDate], [ProviderId], [ProviderName], [SendingEmployerAccountId], [StartDate]) WITH (ONLINE = ON)
GO
CREATE NONCLUSTERED INDEX [idx_commitment_actualendate] ON [dbo].[Commitment] ([ActualEndDate]) INCLUDE ([ApprenticeName], [ApprenticeshipId], [CompletionAmount], [CourseLevel], [CourseName], [EmployerAccountId], [FundingSource], [LearnerId], [MonthlyInstallment], [NumberOfInstallments], [PlannedEndDate], [ProviderId], [ProviderName], [SendingEmployerAccountId], [StartDate]) WITH (ONLINE = ON)
GO