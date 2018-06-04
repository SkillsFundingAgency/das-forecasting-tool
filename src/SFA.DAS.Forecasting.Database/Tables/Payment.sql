﻿CREATE TABLE [dbo].[Payment]
(
	[Id] BIGINT NOT NULL PRIMARY KEY Identity(1,1),
	[ExternalPaymentId] NVARCHAR (100) NOT NULL,
	[EmployerAccountId] BIGINT NOT NULL,
	[SendingEmployerAccountId] BIGINT NOT NULL Constraint DF_Payment__SendingEmployerAccountId default(0),
	[ProviderId] BIGINT NOT NULL,
	[ApprenticeshipId] BIGINT NOT NULL,
	[Amount] DECIMAL(18,2) NOT NULL,
	[ReceivedTime] DATETIME NOT NULL,
	[LearnerId] BIGINT NOT NULL, 
	[CollectionPeriodMonth] INT NOT NULL,
	[CollectionPeriodYear] INT NOT NULL,
	[DeliveryPeriodMonth] INT NOT NULL,
	[DeliveryPeriodYear] INT NOT NULL,
	[FundingSource] TINYINT NOT NULL CONSTRAINT FK_Payment__FundingSource FOREIGN KEY REFERENCES FundingSource(Id)
)
