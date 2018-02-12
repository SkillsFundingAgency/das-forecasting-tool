﻿CREATE TABLE [dbo].[AccountProjectionCommitment]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY (1,1),
	[AccountProjectionId] BIGINT NOT NULL CONSTRAINT FK_AccountProjectionCommitment__AccountProjection FOREIGN KEY REFERENCES AccountProjection(Id),
	[CommitmentId] BIGINT NOT NULL CONSTRAINT FK_AccountProjectionCommitment__Commitment FOREIGN KEY REFERENCES Commitment(Id)
)
