﻿CREATE TABLE [dbo].[Payment]
(
	[Id] BIGINT NOT NULL PRIMARY KEY Identity(1,1),
	[ExternalPaymentId] NVARCHAR (100) NOT NULL,
	[EmployerAccountId] BIGINT NOT NULL,
	[ProviderId] BIGINT NOT NULL,
	[ApprenticeshipId] BIGINT NOT NULL,
	[Amount] DECIMAL NOT NULL,
	[ReceivedTime] DATETIME NOT NULL,
	[LearnerId] BIGINT NOT NULL, 
	[CollectionPeriodMonth] INT NOT NULL,
	[CollectionPeriodYear] INT NOT NULL,
	[FundingSource] TINYINT NOT NULL CONSTRAINT FK_Payment__FundingSource FOREIGN KEY REFERENCES FundingSource(Id)
)
