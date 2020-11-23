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
    [CompletionAmount] DECIMAL(18,5) NOT NULL,
    [MonthlyInstallment] DECIMAL(18,5) NOT NULL,
    [NumberOfInstallments] SMALLINT NOT NULL,
	[FundingSource] TINYINT NOT NULL CONSTRAINT FK_Commitment__FundingSource FOREIGN KEY REFERENCES FundingSource(Id) default(1), 
    [UpdatedDateTime] DATETIME NOT NULL DEFAULT GETDATE(),
	[HasHadPayment] BIT NOT NULL default(1), 
    [Status] INT NULL DEFAULT 0
)

GO
CREATE NONCLUSTERED INDEX [idx_commitment_employerAccountId] ON [dbo].[Commitment] ([EmployerAccountId]) INCLUDE ([ApprenticeName], [ApprenticeshipId], [CompletionAmount], [CourseLevel], [CourseName], [ActualEndDate], [FundingSource], [LearnerId], [MonthlyInstallment], [NumberOfInstallments], [PlannedEndDate], [ProviderId], [ProviderName], [SendingEmployerAccountId], [StartDate],[UpdatedDateTime], [HasHadPayment]) WITH (ONLINE = ON)
GO
CREATE NONCLUSTERED INDEX [idx_commitment_sendingEmployerAccountId] ON [dbo].[Commitment] ([SendingEmployerAccountId]) INCLUDE ([ApprenticeName], [ApprenticeshipId], [CompletionAmount], [CourseLevel], [CourseName], [ActualEndDate], [FundingSource], [LearnerId], [MonthlyInstallment], [NumberOfInstallments], [PlannedEndDate], [ProviderId], [ProviderName], [EmployerAccountId],[UpdatedDateTime], [HasHadPayment]) WITH (ONLINE = ON)
GO
CREATE NONCLUSTERED INDEX [idx_commitment_actualendate] ON [dbo].[Commitment] ([ActualEndDate])INCLUDE ([EmployerAccountId],[SendingEmployerAccountId],[LearnerId],[ProviderId],[ProviderName],[ApprenticeshipId],[ApprenticeName],[CourseName],[CourseLevel],[StartDate],[PlannedEndDate],[CompletionAmount],[MonthlyInstallment],[FundingSource],   [NumberOfInstallments] ,[UpdatedDateTime], [HasHadPayment]  ) WITH (ONLINE = ON)
GO
CREATE NONCLUSTERED INDEX [idx_commitment_employerAccountId_fundingsource_endDate] ON [dbo].[Commitment] ([ActualEndDate],[EmployerAccountId], [FundingSource] , [StartDate]) INCLUDE ([Id],[ApprenticeName], [ApprenticeshipId], [CompletionAmount], [CourseLevel], [CourseName], [SendingEmployerAccountId], [LearnerId], [MonthlyInstallment], [NumberOfInstallments], [PlannedEndDate], [ProviderId], [ProviderName],[UpdatedDateTime], [HasHadPayment]) WHERE(ActualEndDate IS NULL) WITH (ONLINE = ON)
GO
CREATE NONCLUSTERED INDEX [idx_commitment_sendingemployerAccountId_fundingsource_endDate] ON [dbo].[Commitment] ([ActualEndDate],[SendingEmployerAccountId], [FundingSource], [StartDate] ) INCLUDE ([Id],[ApprenticeName], [ApprenticeshipId], [CompletionAmount], [CourseLevel], [CourseName], [EmployerAccountId], [LearnerId], [MonthlyInstallment], [NumberOfInstallments], [PlannedEndDate], [ProviderId], [ProviderName],[UpdatedDateTime], [HasHadPayment]) WHERE(ActualEndDate IS NULL) WITH (ONLINE = ON)
GO
CREATE NONCLUSTERED INDEX [idx_Commitment_employer_accountid_enddata_fundingsource] ON [dbo].[Commitment] ([EmployerAccountId], [ActualEndDate], [FundingSource]) INCLUDE ([ApprenticeName], [ApprenticeshipId], [CompletionAmount], [CourseLevel], [CourseName], [HasHadPayment], [LearnerId], [MonthlyInstallment], [NumberOfInstallments], [PlannedEndDate], [ProviderId], [ProviderName], [SendingEmployerAccountId], [StartDate], [UpdatedDateTime]) WITH (ONLINE = ON)
GO