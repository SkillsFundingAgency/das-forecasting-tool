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
CREATE NONCLUSTERED INDEX [idx_commitment_sendingEmployerAccountId] ON [dbo].[Commitment] ([SendingEmployerAccountId]) INCLUDE ([ApprenticeName], [ApprenticeshipId], [CompletionAmount], [CourseLevel], [CourseName], [ActualEndDate], [FundingSource], [LearnerId], [MonthlyInstallment], [NumberOfInstallments], [PlannedEndDate], [ProviderId], [ProviderName], [EmployerAccountId], [StartDate]) WITH (ONLINE = ON)
GO
CREATE NONCLUSTERED INDEX [idx_commitment_actualendate] ON [dbo].[Commitment] ([ActualEndDate])INCLUDE ([EmployerAccountId],[SendingEmployerAccountId],[LearnerId],[ProviderId],[ProviderName],[ApprenticeshipId],[ApprenticeName],[CourseName],[CourseLevel],[StartDate],[PlannedEndDate],[CompletionAmount],[MonthlyInstallment],[FundingSource],   [NumberOfInstallments]   ) WITH (ONLINE = ON)
GO
CREATE NONCLUSTERED INDEX [idx_commitment_employerAccountId_sending_endDate] ON [dbo].[commitment] ([ActualEndDate],[EmployerAccountId], [SendingEmployerAccountId] ) INCLUDE ([id],[ApprenticeName], [ApprenticeshipId], [CompletionAmount], [CourseLevel], [CourseName], [FundingSource], [LearnerId], [MonthlyInstallment], [NumberOfInstallments], [PlannedEndDate], [ProviderId], [ProviderName], [StartDate]) WHERE(ActualEndDate IS NULL) WITH (ONLINE = ON)
GO
CREATE NONCLUSTERED INDEX [idx_commitment_employerAccountId_fundingsource_endDate] ON [dbo].[commitment] ([ActualEndDate],[EmployerAccountId], [fundingsource] ) INCLUDE ([id],[ApprenticeName], [ApprenticeshipId], [CompletionAmount], [CourseLevel], [CourseName], [SendingEmployerAccountId], [LearnerId], [MonthlyInstallment], [NumberOfInstallments], [PlannedEndDate], [ProviderId], [ProviderName], [StartDate]) WHERE(ActualEndDate IS NULL) WITH (ONLINE = ON)
GO
CREATE NONCLUSTERED INDEX [idx_commitment_sendingemployerAccountId_fundingsource_endDate] ON [dbo].[commitment] ([ActualEndDate],[SendingEmployerAccountId], [fundingsource] ) INCLUDE ([id],[ApprenticeName], [ApprenticeshipId], [CompletionAmount], [CourseLevel], [CourseName], [EmployerAccountId], [LearnerId], [MonthlyInstallment], [NumberOfInstallments], [PlannedEndDate], [ProviderId], [ProviderName], [StartDate]) WHERE(ActualEndDate IS NULL) WITH (ONLINE = ON)
GO